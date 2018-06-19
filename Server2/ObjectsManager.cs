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
    public class ObjectsManager : AbstractActor, IObjectsManager
    {
        private readonly ILog _logger;
        private readonly IConnection _conn;
        private readonly IDatabaseAgent _database;
        private Dictionary<Guid, ISceneActor> loadedScenes;

        public ObjectsManager(IDatabaseAgent database, IConnection conn) : base(
            GenerateGuids.GetActorGuid(TYPEACTOR.OBJECTMANAGER), GenerateGuids.GetActorGuid(TYPEACTOR.EMPTY))
        {
            _logger = LogManager.GetLogger("[Objects Manager]");
            _database = database;
            _conn = conn;

            _logger.Info("is online");
        }

        async Task IObjectsManager.LoadObjects()
        {
        }

        async Task IObjectsManager.LoadAllScenes()
        {
            var scenesInfo = await _database.GetScenes();
            _logger.Info("Total scenes loaded:" + scenesInfo.Count);

            loadedScenes = new Dictionary<Guid, ISceneActor>();

            foreach (var scene in scenesInfo)
            {
                var sceneAgent = Context.ActorOf(Props.Create(() => new ServerSceneActor(scene, _database, _conn)),
                        scene.LocalId.ToString())
                    .Cast<SceneActorRef>();
                loadedScenes.Add(scene.LocalId, sceneAgent);


                _logger.Info("Scene name: " + scene.SceneName);


                await _conn.AddLoadedScene(scene.LocalId, sceneAgent);

                await sceneAgent.LoadSceneObjects();
            }
        }

        async Task IObjectsManager.SaveObjectsToDB()
        {
            foreach (var scene in loadedScenes) await scene.Value.SaveObjectsToDB();
        }

        #region Stats

        async Task<List<Tuple<Guid, string>>> IObjectsManager.GetLoadedScenesList()
        {
            List<Tuple<Guid, string>> ret = new List<Tuple<Guid, string>>();

            foreach (var scene in loadedScenes)
                ret.Add(new Tuple<Guid, string>(scene.Key, scene.Value.GetSceneName().Result));

            return ret;
        }

        async Task<List<Tuple<Guid, string>>> IObjectsManager.GetSceneObjects(Guid scene)
        {
            if (loadedScenes.ContainsKey(scene)) return await loadedScenes[scene].GetSceneObjects();

            return null;
        }

        #endregion
    }
}