using System;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    /// <summary>
    /// Abstract actor interface. Implemented in AbstractActor.
    /// </summary>
    public interface IAbstractActor : IInterfacedActor
    {
        Task<Guid> GetGuid();
        Task<Guid> GetOwnerId();
    }
}