using System.Threading.Tasks;
using Akka.Interfaced;

namespace DomainInternal
{
    public interface IBasicServerPlugin : IInterfacedActor
    {
        Task PluginMessage(string msg);

        Task<PluginInfo> GetInfo();
    }

    public struct PluginInfo
    {
        public string Name;
        public int Version;
    }
}