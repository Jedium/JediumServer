using Domain;
using Domain.BehaviourMessages;
using Jedium.Behaviours.Shared;
using TestComponentLibrary.Shared;
using UnityEngine;

namespace TestComponentLibrary.Unity3D
{

    //Unity side
    public class JediumTestBehaviour : JediumBehaviour
    {
        public override bool ProcessUpdate(JediumBehaviourMessage message)
        {
            if (message == null) //empty update
                return false;

            if (message.GetBehaviourType() != TYPEBEHAVIOUR.GetTypeIndex(GetComponentType()))
            {
                Debug.Log("WRONG MESSAGE TYPE:" + message.GetBehaviourType() + ";" + message.GetType());
                return false;
            }

            JediumTestBehaviourMessage tmsg = (JediumTestBehaviourMessage) message;

            Debug.Log("__GOT TEST MESSAGE:" + tmsg.SomeTestMessage);
            return true;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("___SENDING TEST MESSAGE");
                JediumTestBehaviourMessage msg = new JediumTestBehaviourMessage("Testing:" + Time.deltaTime);
                _updater.AddUpdate(msg);
            }
        }

        void Start()
        {
            Debug.Log("______PLUGIN COMPONENT STARTED");
        }

        public override string GetComponentType()
        {
            return "TestBehaviour";
        }
    }
}