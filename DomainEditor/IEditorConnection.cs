using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Interfaced;
using Domain;

namespace DomainEditor
{
    public interface IEditorConnection:IInterfacedActor
    {
        Task<List<Tuple<Guid, string, string>>> GetSceneList();
        Task<ServerInfo> GetServerInfo();        Task<List<EditorPrefabInfo>> GetPrefabs();    }

    public class EditorPrefabInfo
    {
        public Guid ObjectId;
        public string Name;
        public string Prefab;
        public Guid BundleId;
        public string Metadata;

        public override string ToString()
        {
            return Name;
        }
    }
}
