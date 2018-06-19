using System;

namespace Domain.BehaviourMessages
{
    //snapshot-> class. Used rarely, and class is more useful here
    public abstract class JediumBehaviourSnapshot
    {
        public string BehaviourType;
        public Guid LocalId;

        public JediumBehaviourSnapshot(string type, Guid localId)
        {
            BehaviourType = type;
            LocalId = localId;
        }

        public virtual string GetBehaviourType()
        {
            return BehaviourType;
        }
    }

   
    public interface JediumBehaviourMessage
    {
        int GetBehaviourType();
    }
}