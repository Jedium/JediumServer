using System;
using Domain.BehaviourMessages;

namespace DomainInternal.Behaviours
{
    public abstract class JediumBehaviour
    {
        protected IJediumGameObject _parent;

        public JediumBehaviour(IJediumGameObject parent)
        {
            _parent = parent;
        }

        public abstract JediumBehaviourSnapshot GetSnapshot();
        public abstract void FromSnapshot(JediumBehaviourSnapshot snap);

        public abstract void ProcessMessage(Guid clientId, JediumBehaviourMessage message);

        public abstract string GetBehaviourType();
        public abstract int GetBehaviourIndex();

        public abstract void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages);

        public abstract JediumBehaviourDBSnapshot GetDbSnapshot();

        public abstract void FromDBSnapshot(JediumBehaviourDBSnapshot snap);
    }

    //database
    public abstract class JediumBehaviourDBSnapshot
    {
        public Guid LocalId;
        public string Type;
    }
}