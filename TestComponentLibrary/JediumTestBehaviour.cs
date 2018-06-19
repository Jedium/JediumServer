using System;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using TestComponentLibrary.Shared;

namespace TestComponentLibrary
{
    //server-side
    public class JediumTestBehaviour : JediumBehaviour
    {
        public JediumTestBehaviour(IJediumGameObject parent) : base(parent)
        {
        }

        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumTestBehaviourSnapshot(_parent.LocalId);
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
        }

        public override void ProcessMessage(Guid clientId, JediumBehaviourMessage message)
        {
        }

        public override string GetBehaviourType()
        {
            return "TestBehaviour";
        }

        public override int GetBehaviourIndex()
        {
            return TYPEBEHAVIOUR.GetTypeIndex("TestBehaviour");
        }

        public override void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (messages[0].GetBehaviourType() != GetBehaviourIndex()) return;

            Console.WriteLine("_______GOT TEST MESSAGE");

            _parent.Actor.SendMessagePackToRegisteredClients(Guid.Empty, messages);
        }

        public override JediumBehaviourDBSnapshot GetDbSnapshot()
        {
            return new JediumTestBehaviourDBSnapshot
            {
                LocalId = _parent.LocalId,
                Type = "TestBehaviour"
            };
        }

        public override void FromDBSnapshot(JediumBehaviourDBSnapshot snap)
        {
            //empty
        }
    }

    public class JediumTestBehaviourDBSnapshot : JediumBehaviourDBSnapshot
    {
    }
}