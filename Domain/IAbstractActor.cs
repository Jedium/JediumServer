using System;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    public interface IAbstractActor : IInterfacedActor
    {
        Task<Guid> GetGuid();
        Task<Guid> GetOwnerId();
    }
}