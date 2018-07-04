using System;
using ProtoBuf;

namespace Domain.BehaviourMessages
{
    
    public struct JediumTransformMessage : JediumBehaviourMessage
    {
      
        public readonly float X;
        
        public readonly float Y;
      
        public readonly float Z;
        public readonly float RotX;
        public readonly float RotY;
        public readonly float RotZ;
        public readonly float RotW;
        public readonly float ScaleX;
        public readonly float ScaleY;
        public readonly float ScaleZ;

        private readonly int _behType;

        public JediumTransformMessage(float x, float y, float z,
            float rotx, float roty, float rotz, float rotw,
            float scalex, float scaley, float scalez)
        {
            X = x;
            Y = y;
            Z = z;
            RotX = rotx;
            RotY = roty;
            RotZ = rotz;
            RotW = rotw;
            ScaleX = scalex;
            ScaleY = scaley;
            ScaleZ = scalez;

            _behType = TYPEBEHAVIOUR.GetTypeIndex("Transform");
        }

        public int GetBehaviourType()
        {
            return _behType;
        }
    }

    public class JediumTransformSnapshot : JediumBehaviourSnapshot
    {
        private static readonly JediumTransformSnapshot _identity =
            new JediumTransformSnapshot(Guid.Empty, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f);

        public float RotW;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float ScaleX;
        public float ScaleY;

        public float ScaleZ;

      
        public float X;
        public float Y;
        public float Z;

        public JediumTransformSnapshot(Guid localId, float x, float y, float z,
            float rotx, float roty, float rotz, float rotw,
            float scalex, float scaley, float scalez) : base("Transform", localId)
        {
            X = x;
            Y = y;
            Z = z;
            RotX = rotx;
            RotY = roty;
            RotZ = rotz;
            RotW = rotw;
            ScaleX = scalex;
            ScaleY = scaley;
            ScaleZ = scalez;
        }

        public static JediumTransformSnapshot Identity => _identity;
    }
}