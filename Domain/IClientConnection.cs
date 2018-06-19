using System;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    public interface IClientConnection : IInterfacedActor
    {
        Task RegisterConnection(Guid clientID, string scene);
        Task KillConnection();
    }
}