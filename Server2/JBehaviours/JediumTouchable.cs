using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBehaviours
{
   public class JediumTouchable :JediumBehaviour
    {
        public JediumTouchable(JediumGameObject parent) : base(parent)
        {

        }

        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumToucheableSnapshot();
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetType() != typeof(JediumToucheableSnapshot))
            {
                //warn
            }
        }

        public override void ProcessMessage(Guid clientId,JediumBehaviourMessage message)
        {
            //stub
            if (message.GetType() != typeof(JediumTouchMessage))
                return;

            _parent.Actor.SendMessageToRegisteredClients(clientId, message);
        }
    }

    public class JediumTouchMessage:JediumBehaviourMessage
    {
        public readonly Guid ClientId;
        public readonly float U;
        public readonly float V;

        public JediumTouchMessage(Guid clientId, float u, float v)
        {
            ClientId = clientId;
            U = u;
            V = v;
        }

    }

    public class JediumToucheableSnapshot : JediumBehaviourSnapshot
    {

    }
}
