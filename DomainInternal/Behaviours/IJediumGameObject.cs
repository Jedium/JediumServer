using System;
using Domain;

namespace DomainInternal.Behaviours
{
    public interface IJediumGameObject
    {
        Guid LocalId { get; }
        Guid OwnerId { get; }
        IGameObjectSelfAccessor Actor { get; }
    }
}