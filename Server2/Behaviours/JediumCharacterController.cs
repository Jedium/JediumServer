using System;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using NLog;

namespace Server2.Behaviours
{
    public class JediumCharacterController : JediumBehaviour
    {
        private readonly ILogger _log;

        public JediumCharacterController(JediumGameObject parent) : base(parent)
        {
            _log = LogManager.GetLogger("CharacterController: " + parent.LocalId);
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetBehaviourType() != "CharacterController")
                _log.Warn($"Wrong snapshot type: {snap.GetBehaviourType()}");
        }

        public override string GetBehaviourType()
        {
            return "CharacterController";
        }

        public override int GetBehaviourIndex()
        {
            return TYPEBEHAVIOUR.GetTypeIndex("CharacterController");
        }

        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumCharacterControllerSnapshot(_parent.LocalId);
        }

        //unused
        public override void ProcessMessage(Guid clientId, JediumBehaviourMessage message)
        {
          // if (message.GetBehaviourType() != GetBehaviourIndex()) return;
          //
          // _parent.Actor.SendMessageToRegisteredClients(Guid.Empty, message);
        }

        public override void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (messages[0].GetBehaviourType() != GetBehaviourIndex())
            {
                _log.Warn($"Wrong message type:" + messages[0].GetBehaviourType());
                return;
            }

            _parent.Actor.SendMessagePackToRegisteredClients(Guid.Empty, messages);
        }

        public override JediumBehaviourDBSnapshot GetDbSnapshot()
        {
            return new JediumCharacterControllerDBSnapshot
            {
                LocalId = _parent.LocalId,
                Type = "CharacterController"
            };
        }

        public override void FromDBSnapshot(JediumBehaviourDBSnapshot snap)
        {
            //empty
        }
    }


    public class JediumCharacterControllerDBSnapshot : JediumBehaviourDBSnapshot
    {
    }
}