using System;

namespace Domain.BehaviourMessages
{
    public class JediumCharacterControllerSnapshot : JediumBehaviourSnapshot
    {
        public JediumCharacterControllerSnapshot(Guid localId) : base("CharacterController", localId)
        {
        }
    }

    public struct JediumCharacterControllerMessage : JediumBehaviourMessage
    {
        public readonly float V;
        public readonly float H;
        public readonly bool Jump;

        private readonly int _behType;

        public int GetBehaviourType()
        {
            return _behType;
        }

        public JediumCharacterControllerMessage(float v, float h, bool jump)
        {
            V = v;
            H = h;
            Jump = jump;
            _behType = TYPEBEHAVIOUR.GetTypeIndex("CharacterController");
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(JediumCharacterControllerMessage))
                return false;
            JediumCharacterControllerMessage mobj = (JediumCharacterControllerMessage) obj;

            return Math.Abs(V - mobj.V) < 0.1f && Math.Abs(H - mobj.H) < 0.1f && Jump == mobj.Jump;
        }

        public static bool operator ==(JediumCharacterControllerMessage obj1, JediumCharacterControllerMessage obj2)
        {
            return Math.Abs(obj1.V - obj2.V) < 0.1f && Math.Abs(obj1.H - obj2.H) < 0.1f && obj1.Jump == obj2.Jump;
        }

        public static bool operator !=(JediumCharacterControllerMessage obj1, JediumCharacterControllerMessage obj2)
        {
            return Math.Abs(obj1.V - obj2.V) > 0.1f || Math.Abs(obj1.H - obj2.H) > 0.1f || obj1.Jump != obj2.Jump;
        }
    }
}