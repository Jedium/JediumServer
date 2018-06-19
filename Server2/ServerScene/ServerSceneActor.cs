using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Interfaced;
using Common.Logging;
using Domain;
using DomainInternal;

namespace Server2
{
   
    class ServerSceneActor : AbstractActor, ISceneActor
    {
        private readonly ILog _logger;
        private readonly Guid _bundleId;
        private readonly IConnection _conn;
        private readonly IDatabaseAgent _database;
        private readonly string _sceneName;
        private readonly string _serverName;
        private Dictionary<Guid, Tuple<DatabaseObject, IGameObject>> sceneObjects;

        public ServerSceneActor(DatabaseScene dscene, IDatabaseAgent database, IConnection conn)
            : base(dscene.LocalId, GenerateGuids.GetActorGuid(TYPEACTOR.EMPTY))
        {
            _logger = LogManager.GetLogger($"[Scene: {dscene.ServerName}]");
            _serverName = dscene.ServerName;
            _sceneName = dscene.SceneName;
            _bundleId = dscene.BundleId;
            _database = database;
            _conn = conn;

            _logger.Info("is online");
        }

        async Task<string> ISceneActor.GetServerName()
        {
            return _serverName;
        }

        async Task<string> ISceneActor.GetSceneName()
        {
            return _sceneName;
        }

        async Task<Guid> ISceneActor.GetBundleId()
        {
            return _bundleId;
        }


        async Task ISceneActor.LoadSceneObjects()
        {
            var objects = await _database.GetObjectsScene(_localID);


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
        }

        #region Комната: вызов спавна каждого обьекта через IConnectionObserver

        async Task ISceneActor.PushObjectsToClient(Guid clientId, IConnectionObserver client)
        {
            foreach (var obj in sceneObjects)
            {
                _logger.Info($"Spawning object {obj.Key} on client {clientId}");

               
                client.OnSpawnedGameObject(obj.Value.Item1.Prefab, obj.Value.Item1.Prefab, obj.Key,
                    obj.Value.Item2.GetOwnerId().Result, obj.Value.Item2.GetBundleId().Result,
                    obj.Value.Item2.GetAvatarId().Result, obj.Value.Item2.GetServerAddress().Result,
                    obj.Value.Item2.GetSnapshot().Result);
            }
        }

        #endregion

        async Task ISceneActor.LogoutClient(Guid clientId)
        {
            foreach (var obj in sceneObjects) obj.Value.Item2.UnregisterClient(clientId).Wait();
        }

        #region Stats

        async Task<List<Tuple<Guid, string>>> ISceneActor.GetSceneObjects()
        {
            List<Tuple<Guid, string>> ret = new List<Tuple<Guid, string>>();

            foreach (var obj in sceneObjects)
                ret.Add(new Tuple<Guid, string>(obj.Key, obj.Value.Item2.GetNameOfPrefab().Result));

            return ret;
        }

        #endregion

        async Task ISceneActor.SaveObjectsToDB()
        {
            foreach (var obj in sceneObjects) await obj.Value.Item2.SaveToDB();
        }
    }
}