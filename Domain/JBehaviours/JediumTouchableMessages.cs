using System;

namespace Domain.BehaviourMessages
{
    public struct JediumTouchMessage : JediumBehaviourMessage
    {
        public readonly Guid ClientId;
        public readonly float U;
        public readonly float V;

        private readonly int _behType;

        public JediumTouchMessage(Guid clientId, float u, float v)
        {
            ClientId = clientId;
            U = u;
            V = v;

            _behType = TYPEBEHAVIOUR.GetTypeIndex("Touch");
        }

        public int GetBehaviourType()
        {
            return _behType;
        }
    }

    public class JediumToucheableSnapshot : JediumBehaviourSnapshot
    {
        public JediumToucheableSnapshot(Guid localId) : base("Touch", localId)
        {
        }
    }
}