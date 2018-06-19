using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBehaviours
{
    /// <summary>
    /// Realization Monobehaviour AnimatorClass
    /// </summary>
    public class JediumAnimator:JediumBehaviour
    {
        private List<JediumAnimatorMessage> _animatorParams;

        public JediumAnimator(JediumGameObject parent):base(parent)
        {
            _animatorParams = new List<JediumAnimatorMessage>();
        }

        public void AddAnimatorParametr(JediumAnimatorMessage p)
        {
            _animatorParams.Add(p);
        }

        public void RemoveAnimatorParametr(JediumAnimatorMessage p)
        {
            _animatorParams.Remove(p);
        }

        public bool FindAnimatorParametr(JediumAnimatorMessage p)
        {
            return _animatorParams.Contains(p);
        }

        public int CountAnimationParametr()
        {
            return _animatorParams.Count;
        }

        public void ClearAnimationParametr()
        {
            _animatorParams.Clear();
        }

        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumAnimatorSnapshot();
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetType() != typeof(JediumAnimatorSnapshot))
            {
                //warn
            }
        }

        public override void ProcessMessage(Guid clientId,JediumBehaviourMessage message)
        {
            //stub
            if (message.GetType() != typeof(JediumAnimatorMessage))
                return;

            _parent.Actor.SendMessageToRegisteredClients(clientId, message);
        }
    }


    //shoud be nullable
    //and make it immutable
    public class JediumAnimatorMessage:JediumBehaviourMessage
    {
        public readonly JEDIUM_TYPE_ANIMATOR type;
        public readonly string nameParametr;

        public readonly object _value;

        public JediumAnimatorMessage(JEDIUM_TYPE_ANIMATOR type, string nameParametr, object value)
        {
            this.type = type;
            this.nameParametr = nameParametr;
            _value = value;
        }


    }

    public class JediumAnimatorSnapshot : JediumBehaviourSnapshot
    {

    }
}
