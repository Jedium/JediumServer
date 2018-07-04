using System;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using NLog;

namespace Server2.Behaviours
{
    public class JediumTouchable : JediumBehaviour
    {
        
        private readonly ILogger _log;

        public JediumTouchable(JediumGameObject parent) : base(parent)
        {
            _log = LogManager.GetLogger("Touchable: " + parent.LocalId);
        }

        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumToucheableSnapshot(_parent.LocalId);
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetBehaviourType() != "Touch") _log.Warn($"Wrong snapshot type: {snap.GetBehaviourType()}");
        }

        //unused
        public override void ProcessMessage(Guid clientId, JediumBehaviourMessage message)
        {
          // //stub
          // if (message.GetType() != typeof(JediumTouchMessage))
          //     return;
          //
          // _parent.Actor.SendMessageToRegisteredClients(clientId, message);
        }

        public override int GetBehaviourIndex()
        {
            return TYPEBEHAVIOUR.GetTypeIndex("Touch");
        }

        public override void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (messages[0].GetBehaviourType() != GetBehaviourIndex()) return;

            _parent.Actor.SendMessagePackToRegisteredClients(clientId, messages);
        }

        public override string GetBehaviourType()
        {
            return "Touch";
        }

        public override JediumBehaviourDBSnapshot GetDbSnapshot()
        {
            return new JediumTouchableDBSnapshot
            {
                LocalId = _parent.LocalId,
                Type = "Touch"
            };
        }

        public override void FromDBSnapshot(JediumBehaviourDBSnapshot snap)
        {
            //empty
        }
    }

    public class JediumTouchableDBSnapshot : JediumBehaviourDBSnapshot
    {
    }
}