using System;
using System.Collections.Generic;
using System.Web.Http;
using Common.Logging;
using DomainInternal;
using Server2.Behaviours;

namespace Server2.Web
{
    public class FrameworkStatsController : ApiController
    {
        private readonly ILog _logger;

        private readonly IObjectsManager _manager;

        public FrameworkStatsController(IObjectsManager manager)
        {
            _logger = LogManager.GetLogger("[Stats Controller]");

            _manager = manager;
        }

        [HttpGet]
        public int ScenesCount()
        {
            return _manager.GetLoadedScenesList().Result.Count;
        }

        [HttpGet]
        public List<Tuple<Guid, string,string>> LoadedScenes()
        {
            return _manager.GetLoadedScenesList().Result;
        }

        [HttpGet]
        public string ServerHash()
        {
            return MainSettings.ServerHash;
        }

        [HttpGet]
        public List<string> LoadedBehaviours()
        {
            List<string> ret = new List<string>();

            foreach (var beh in BehaviourTypeRegistry.BehaviourTypes) ret.Add(beh.Key);

            return ret;
        }

        [HttpGet]
        public List<Tuple<Guid, string>> SceneActors(string id)
        {
            Guid sid;

            if (Guid.TryParse(id, out sid)) return _manager.GetSceneObjects(sid).Result;

            return null;
        }
    }
}