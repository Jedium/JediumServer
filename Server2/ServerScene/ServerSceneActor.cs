using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Interfaced;
using Akka.Remote;
using Akka.Util.Internal;
using Common.Logging;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal;

namespace Server2
{
   
    class ServerSceneActor :InterfacedActor, ISceneActor//,IAbstractActor
    {
        private readonly ILog _logger;
        private readonly Guid _bundleId;
        private readonly IConnection _conn;
        private readonly DatabaseAgentRef _database;
        private readonly string _sceneName;
        private readonly string _serverName;
        private readonly Guid _localId;
        private readonly Guid _ownerId;
        private Dictionary<Guid, Tuple<DatabaseObject, IGameObject>> sceneObjects;

     

        public ServerSceneActor(DatabaseScene dscene, DatabaseAgentRef database, IConnection conn)
           
        {
            _logger = LogManager.GetLogger($"[Scene: {dscene.ServerName}]");
            _serverName = dscene.ServerName;
            _sceneName = dscene.SceneName;
            _bundleId = dscene.BundleId;
            _database = database;
            _conn = conn;
            _localId = dscene.LocalId;
            _ownerId = Guid.Empty;


        }

        Task ISceneActor.TestConnection()
        {
            Console.WriteLine("___CONNECTED");
            return Task.FromResult(true);
        }

        Task<string> ISceneActor.GetActorAddress()
        {
            return Task.FromResult(GetRemoteAddress());
        }

        protected string GetRemoteAddress()
        {
            Address addr = Context.System.AsInstanceOf<ExtendedActorSystem>().Provider
                .AsInstanceOf<RemoteActorRefProvider>().DefaultAddress;

            return Self.Path.ToStringWithAddress(addr);
        }

        

        async Task<Tuple<Guid,ObjectSnapshot>> ISceneActor.GetObjectInfo(Guid localId)
        {
            if (!sceneObjects.ContainsKey(localId))
                return null;

            return Tuple.Create(sceneObjects[localId].Item2.GetBundleId().Result,sceneObjects[localId].Item2.GetSnapshot().Result);
        }

        public Task SetObjectBehaviour(Guid localId, JediumBehaviourSnapshot snap)
        {
            if (!sceneObjects.ContainsKey(localId))
            {
                _logger.Warn($"Object {localId} not found while trying to set behaviour");
                return Task.FromResult(false);
            }

            return sceneObjects[localId].Item2.SetBehaviourSnapshot(snap);

        }
    

        Task<string> ISceneActor.GetServerName()
        {
            return Task.FromResult(_serverName);


        }

        Task<string> ISceneActor.GetSceneName()
        {
            return Task.FromResult(_sceneName);
        }

        Task<Guid> ISceneActor.GetBundleId()
        {
            return Task.FromResult(_bundleId);
        }


        Task ISceneActor.AddSceneObject(Guid localId, Guid prefabId, List<JediumBehaviourSnapshot> snapshots)
        {
            DatabaseObject dbObj = _database.GetObjectServer(prefabId).Result;

            IGameObject sceneAgent = Context
                .ActorOf(
                    Props.Create(() =>
                        new ServerGameObject(_conn, _database, dbObj.Prefab,
                            dbObj.Prefab, localId, Guid.Empty, dbObj.BundleId, Guid.Empty, Guid.Empty, "", null,
                            null)),
                    localId.ToString())
                .Cast<GameObjectRef>();


            sceneObjects.Add(localId, Tuple.Create(dbObj, sceneAgent));

            _database.AddSceneObject(new DatabaseSceneObject()
            {
                LocalId = localId,
                ObjectId = prefabId,
                SceneId = _localId
            }).Wait();

            return Task.FromResult(true);
        }

        Task ISceneActor.DeleteSceneObject(Guid localId)
        {
            if (!sceneObjects.ContainsKey(localId))
            {
                _logger.Warn($"Object {localId} not found, can't delete");
                return Task.FromResult(true);
            }

            sceneObjects[localId].Item2.DestroyObject().Wait();
            sceneObjects.Remove(localId);

            _database.DeleteSceneObject(localId).Wait();
            _database.DeleteObjectBehaviours(localId).Wait();
            return Task.FromResult(true);
        }

        //possible
        async Task ISceneActor.LoadSceneObjects()
        {
              var objects = await _database.GetObjectsScene(_localId);

            foreach (var obj in objects)
            {
                Console.WriteLine("___OBJECT:"+obj.LocalId);
            }


            sceneObjects = new Dictionary<Guid, Tuple<DatabaseObject, IGameObject>>();

            foreach (var obj in objects)
            {
                DatabaseObject dbObj = _database.GetObjectServer(obj.ObjectId).Result;


                IGameObject sceneAgent = Context
                    .ActorOf(
                        Props.Create(() =>
                            new ServerGameObject(_conn, _database, dbObj.Prefab,
                                dbObj.Prefab, obj.LocalId, Guid.Empty, dbObj.BundleId, Guid.Empty, Guid.Empty, "", null,
                                null)),
                        obj.LocalId.ToString())
                    .Cast<GameObjectRef>();


                sceneObjects.Add(obj.LocalId, Tuple.Create(dbObj, sceneAgent));


                _logger.Info("sceneObjects: " + dbObj.Name);
            }

            _logger.Info("is online");
        }

        

        #region Комната: вызов спавна каждого обьекта через IConnectionObserver

        Task ISceneActor.PushObjectsToClient(Guid clientId, IConnectionObserver client)
        {
            foreach (var obj in sceneObjects)
            {
                _logger.Info($"Spawning object {obj.Key} on client {clientId}");

               
                client.OnSpawnedGameObject(obj.Value.Item1.Prefab, obj.Value.Item1.Prefab, obj.Key,
                    obj.Value.Item2.GetOwnerId().Result, obj.Value.Item2.GetBundleId().Result,
                    obj.Value.Item2.GetAvatarId().Result, obj.Value.Item2.GetServerAddress().Result,
                    obj.Value.Item2.GetSnapshot().Result);
            }

            return Task.FromResult(true);
        }

        #endregion

        Task ISceneActor.LogoutClient(Guid clientId)
        {
            foreach (var obj in sceneObjects) obj.Value.Item2.UnregisterClient(clientId).Wait();

            return Task.FromResult(true);
        }

        #region Stats

        Task<List<Tuple<Guid, string>>> ISceneActor.GetSceneObjects()
        {
            List<Tuple<Guid, string>> ret = new List<Tuple<Guid, string>>();

            foreach (var obj in sceneObjects)
                ret.Add(new Tuple<Guid, string>(obj.Key, obj.Value.Item2.GetNameOfPrefab().Result));

            return Task.FromResult(ret);
        }

        #endregion

        //TODO - async
        async Task ISceneActor.SaveObjectsToDB()
        {
            foreach (var obj in sceneObjects) await obj.Value.Item2.SaveToDB();
        }

     // Task<Guid> IAbstractActor.GetGuid()
     // {
     //     return Task.FromResult(_localId);
     // }
     //
     // Task<Guid> IAbstractActor.GetOwnerId()
     // {
     //     return Task.FromResult(_ownerId);
     // }
    }
}