using System;
using Domain;
using Domain.BehaviourMessages;

namespace TestComponentLibrary.Shared
{
    //shared: message protocol


    public class JediumTestBehaviourSnapshot : JediumBehaviourSnapshot
    {
        //we need this for plugin loading
        public JediumTestBehaviourSnapshot() : base("TestBehaviour", Guid.Empty)
        {
        }

        public JediumTestBehaviourSnapshot(Guid localId) : base("TestBehaviour", localId)
        {
        }
    }

    public struct JediumTestBehaviourMessage : JediumBehaviourMessage
    {
        public readonly string SomeTestMessage;


        public JediumTestBehaviourMessage(string message)
        {
            SomeTestMessage = message;
        }

        public int GetBehaviourType()
        {
            return TYPEBEHAVIOUR.GetTypeIndex("TestBehaviour");
        }
    }
}