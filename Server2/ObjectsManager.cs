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
    public class ObjectsManager : InterfacedActor, IObjectsManager
    {
        private readonly ILog _logger;
        private readonly IConnection _conn;
        private readonly DatabaseAgentRef _database;
        private Dictionary<Guid, SceneActorRef> loadedScenes;

        public ObjectsManager(DatabaseAgentRef database, IConnection conn) 
        {
            _logger = LogManager.GetLogger("[Objects Manager]");
            _database = database;
            _conn = conn;

            _logger.Info("is online");
        }

        Task IObjectsManager.LoadObjects()
        {
            return Task.FromResult(false);
        }

        //possible
        async Task IObjectsManager.LoadAllScenes()
        {
            var scenesInfo = await _database.GetScenes();
            _logger.Info("Total scenes loaded:" + scenesInfo.Count);

            loadedScenes = new Dictionary<Guid, SceneActorRef>();

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

        //possible
        async Task IObjectsManager.SaveObjectsToDB()
        {
            foreach (var scene in loadedScenes) await scene.Value.SaveObjectsToDB();
        }

        #region Stats

         Task<List<Tuple<Guid, string,string>>> IObjectsManager.GetLoadedScenesList()
        {
            List<Tuple<Guid, string,string>> ret = new List<Tuple<Guid, string,string>>();

            foreach (var scene in loadedScenes)
                ret.Add(new Tuple<Guid, string,string>(scene.Key, scene.Value.GetSceneName().Result,scene.Value.GetActorAddress().Result));

            return Task.FromResult(ret);
        }

        //TODO - async problem
        async Task<List<Tuple<Guid, string>>> IObjectsManager.GetSceneObjects(Guid scene)
        {
            if (loadedScenes.ContainsKey(scene)) return await loadedScenes[scene].GetSceneObjects();

            return null;
        }

        #endregion
    }
}