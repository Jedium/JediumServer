using Domain.BehaviourMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BehaviourMessages
{
   public struct JediumTakeMessage : JediumBehaviourMessage
    {

        public readonly Guid ClientId;
        public readonly Guid LocalId;
        public bool IsTaken;


        private readonly int _behType;

        public JediumTakeMessage(Guid clientId, Guid localId, bool IsTaken)
        {
            ClientId = clientId;
            LocalId = localId;
            _behType = TYPEBEHAVIOUR.GetTypeIndex("Take");
            this.IsTaken = IsTaken;
        }

        public int GetBehaviourType()
        {
            return _behType;
        }

    }


    public class JediumTakeSnapshot: JediumBehaviourSnapshot
    {

        public float X;
        public float Y;
        public float Z;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;

        public JediumTakeSnapshot(Guid localId, float X, float Y, float Z, float rotX, float rotY, float rotZ, float rotW):base("Take", localId)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.RotX = rotX;
            this.RotY = rotY;
            this.RotZ = rotZ;
            this.RotW = rotW;
        }






    }
}
