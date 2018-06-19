using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBehaviours
{
    public abstract class JediumBehaviour
    {

        protected JediumGameObject _parent;

        public JediumBehaviour(JediumGameObject parent)
        {
            _parent = parent;
        }

        public abstract JediumBehaviourSnapshot GetSnapshot();
        public abstract void FromSnapshot(JediumBehaviourSnapshot snap);

        public abstract void ProcessMessage(Guid clientId,JediumBehaviourMessage message);

    }

    public abstract class JediumBehaviourSnapshot
    {

    }

    public abstract class JediumBehaviourMessage
    {

    }
}
