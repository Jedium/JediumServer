using System;
using System.Threading.Tasks;
using Akka.Interfaced;
using Domain;

namespace Server2
{
    public abstract class AbstractActor : InterfacedActor, IAbstractActor
    {
        protected AbstractActor(Guid localID, Guid ownerID)
        {
            _localID = localID;
            _ownerID = ownerID;
        }

        protected Guid _localID { get; }
        protected Guid _ownerID { get; }


        async Task<Guid> IAbstractActor.GetGuid()
        {
            return _localID;
        }

        async Task<Guid> IAbstractActor.GetOwnerId()
        {
            return _ownerID;
        }
    }
}