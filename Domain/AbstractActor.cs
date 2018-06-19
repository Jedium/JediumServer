using System;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    public abstract class AbstractActor : InterfacedActor, IAbstractActor
    {
        protected AbstractActor(Guid localID, Guid ownerID)
        {
            _localID = localID;
            _ownerID = ownerID;
        }

        public Guid _localID { get; }
        public Guid _ownerID { get; }


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