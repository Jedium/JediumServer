using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Interfaced;
using Domain;
using DomainEditor;
using DomainInternal;

namespace Server2.Editor
{
    class EditorConnection:InterfacedActor,IEditorConnection
    {
        private ObjectsManagerRef _objectsManager;

        private DatabaseAgentRef _database;

        public EditorConnection(ObjectsManagerRef omanager,DatabaseAgentRef database)
        {
            _objectsManager = omanager;
            _database = database;
        }

        async Task<ServerInfo> IEditorConnection.GetServerInfo()
        {
            ServerInfo info = new ServerInfo();
            info.AdditionalRegisteredBehaviours = TYPEBEHAVIOUR.AdditionalBehaviours;
            return info;
        }

        Task<List<Tuple<Guid, string, string>>> IEditorConnection.GetSceneList()
        {
            return _objectsManager.GetLoadedScenesList();
        }

        async Task<List<EditorPrefabInfo>> IEditorConnection.GetPrefabs()
        {
            var prfbs = await _database.GetAllServerObjects();

            List<EditorPrefabInfo> ret=new List<EditorPrefabInfo>();

            foreach (var prfb in prfbs)
            {
                EditorPrefabInfo pi = new EditorPrefabInfo()
                {
                    BundleId = prfb.BundleId,
                    Metadata = "",
                    Name = prfb.Name,
                    ObjectId = prfb.ObjectId,
                    Prefab = prfb.Prefab
                };
                ret.Add(pi);
            }

            return ret;
        }
    }
}
