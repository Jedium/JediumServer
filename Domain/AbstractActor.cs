using System;
using System.Threading.Tasks;
using Akka.Interfaced;

namespace Domain
{
    /// <summary>
    /// Basic class for almost all actors.
    /// Implements LocalID and OwnerID properties.
    /// </summary>
    public abstract class AbstractActor : InterfacedActor, IAbstractActor
    {
        protected AbstractActor(Guid localID, Guid ownerID)
        {
            _localID = localID;
            _ownerID = ownerID;
        }

        public Guid _localID { get; }
        public Guid _ownerID { get; }


        /// <summary>
        /// Get LocalID
        /// </summary>
        /// <returns></returns>
        async Task<Guid> IAbstractActor.GetGuid()
        {
            return _localID;
        }

        /// <summary>
        /// Get OwnerID
        /// </summary>
        /// <returns></returns>
        async Task<Guid> IAbstractActor.GetOwnerId()
        {
            return _ownerID;
        }
    }
}