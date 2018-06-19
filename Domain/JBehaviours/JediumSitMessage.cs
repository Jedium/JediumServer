using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BehaviourMessages
{
    public class JediumSitMessage : JediumBehaviourMessage
    {

        public readonly Guid ClientId;
        public readonly Guid LocalId;
        public bool IsOccupied;

        private readonly int _behType;

        public JediumSitMessage(Guid _clientId, Guid _localId, bool isOccupied)
        {
            ClientId = _clientId;
            LocalId = _localId;
            IsOccupied = isOccupied;
            _behType = TYPEBEHAVIOUR.GetTypeIndex("Sit");

        }


        public int GetBehaviourType()
        {
            return _behType;
        }
    }


    public class JediumSitSnapshot: JediumBehaviourSnapshot
    {
        public float X;
        public float Y;
        public float Z;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;

        public JediumSitSnapshot(Guid localId, float X, float Y, float Z, float rotX, float rotY, float rotZ, float rotW) : base("Sit", localId)
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
