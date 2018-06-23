using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Interfaced;
using Common.Logging;
using Domain;
using DomainInternal;

namespace Server2
{
   
    class ServerSceneActor :InterfacedActor, ISceneActor,IAbstractActor
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


        //possible
        async Task ISceneActor.LoadSceneObjects()
        {
              var objects = await _database.GetObjectsScene(_localId);


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

        Task<Guid> IAbstractActor.GetGuid()
        {
            return Task.FromResult(_localId);
        }

        Task<Guid> IAbstractActor.GetOwnerId()
        {
            return Task.FromResult(_ownerId);
        }
    }
}