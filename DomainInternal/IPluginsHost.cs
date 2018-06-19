using System.Threading.Tasks;
using Akka.Interfaced;

namespace DomainInternal
{
    public interface IPluginsHost : IInterfacedActor
    {
        Task LoadPlugins();
    }
}