using System;
using System.Collections.Generic;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using NLog;

namespace Server2.Behaviours
{
    /// <summary>
    ///     Realization Monobehaviour AnimatorClass
    /// </summary>
    public class JediumAnimator : JediumBehaviour
    {
        private readonly List<JediumAnimatorMessage> _animatorParams;
        private readonly ILogger _log;

        public JediumAnimator(JediumGameObject parent) : base(parent)
        {
            _log = LogManager.GetLogger("Animator: " + parent.LocalId);
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
            return new JediumAnimatorSnapshot(_parent.LocalId);
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetBehaviourType() != "Animation") _log.Warn($"Wrong snapshot type: {snap.GetBehaviourType()}");
        }

        //unused
        public override void ProcessMessage(Guid clientId, JediumBehaviourMessage message)
        {
           
          // if (message.GetType() != typeof(JediumAnimatorMessage))
          //     return;
          //
          // JediumAnimatorMessage msg = (JediumAnimatorMessage) message;
          //
          // if (!msg.IsDirectUpdate)
          //     _parent.Actor.SendMessageToRegisteredClients(clientId, message);
          // else
          //     _parent.Actor.SendMessageToRegisteredClients(Guid.Empty, message);
        }

        public override int GetBehaviourIndex()
        {
            return TYPEBEHAVIOUR.GetTypeIndex("Animation");
        }

        public override void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (messages[0].GetBehaviourType() != GetBehaviourIndex()) return;

            //we assume direct messages
            _parent.Actor.SendMessagePackToRegisteredClients(Guid.Empty, messages);
        }

        public override JediumBehaviourDBSnapshot GetDbSnapshot()
        {
            return new JediumAnimatorDBSnapshot
            {
                LocalId = _parent.LocalId,
                Type = "Animation"
            };
        }

        public override void FromDBSnapshot(JediumBehaviourDBSnapshot snap)
        {
            //empty
        }

        public override string GetBehaviourType()
        {
            return "Animation";
        }
    }

    public class JediumAnimatorDBSnapshot : JediumBehaviourDBSnapshot
    {
    }
}