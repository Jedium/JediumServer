using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace JBehaviours
{
    /// <summary>
    /// для упаковки и передачи данных
    /// </summary>
    ///     
    public class JediumTransform :JediumBehaviour
    {
      //  public JediumTransform(JediumGameObject parent, float posX, float posY, float posZ, float quatX, float quatY, float quatZ, float quatW,
      //      float scaleX, float scaleY, float scaleZ):base(parent)
        public JediumTransform(JediumGameObject parent):base(parent)
        {
           _posX = 0;
           _posY =0;
           _posZ = 0;
           _quatX = 0;
           _quatY = 0;
           _quatZ = 0;
            _quatW = 1;
           _scaleX = 1;
           _scaleY = 1;
           _scaleZ = 1;
        }

        public JediumTransform(JediumGameObject parent, JediumTransformSnapshot snap):base(parent)
        {
            this.FromSnapshot(snap);
        }

        

        public string NamePrefab;

        // Postion 
        public float _posX;
        public float _posY;
        public float _posZ;

        // Rotation 
        public float _quatX;
        public float _quatY;
        public float _quatZ;
        public float _quatW;

        // Scale 
        public float _scaleX;
        public float _scaleY;
        public float _scaleZ;

        public override string ToString()
        {
            return $"Pos: {_posX},{_posY},{_posZ}";
        }

        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumTransformSnapshot
            {
                X=_posX,
                Y=_posY,
                Z=_posZ,
                RotX = _quatX,
                RotY = _quatY,
                RotZ = _quatZ,
                RotW = _quatW,
                ScaleX = _scaleX,
                ScaleY = _scaleY,
                ScaleZ = _scaleZ
            };

            
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetType() != typeof(JediumTransformSnapshot))
            {
                //warn
                return;
            }

            JediumTransformSnapshot tsnap = (JediumTransformSnapshot) snap;

            _posX = tsnap.X;
            _posY = tsnap.Y;
            _posZ = tsnap.Z;
            _quatX = tsnap.RotX;
            _quatY = tsnap.RotY;
            _quatZ = tsnap.RotZ;
            _quatW = tsnap.RotW;
            _scaleX = tsnap.ScaleX;
            _scaleY = tsnap.ScaleY;
            _scaleZ = tsnap.ScaleZ;
        }

        public override void ProcessMessage(Guid clientId,JediumBehaviourMessage message)
        {
            if (message.GetType() != typeof(JediumTransformMessage))
                return;

            Console.WriteLine("______PROCESSING TRANSFORM MESSAGE");

            JediumTransformMessage amsg = (JediumTransformMessage) message;


            _posX = amsg.X;
            _posY = amsg.Y;
            _posZ = amsg.Z;
            _quatX = amsg.RotX;
            _quatY = amsg.RotY;
            _quatZ = amsg.RotZ;
            _quatW = amsg.RotW;
            _scaleX = amsg.ScaleX;
            _scaleY = amsg.ScaleY;
            _scaleZ = amsg.ScaleZ;

            //CRASH
          // if(_parent==null)
          //     Console.WriteLine("_______NULL PARENT");
          // if(_parent.Actor==null)
          //     Console.WriteLine("___NULL ACTOR");

            _parent.Actor.SendMessageToRegisteredClients(Guid.Empty, message).Wait();

            Console.WriteLine("______END PROCESSING TRANSFORM MESSAGE");
        }
    }

    public class JediumTransformMessage : JediumBehaviourMessage
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

        public JediumTransformMessage(float x, float y, float z,
            float rotx, float roty, float rotz,float rotw,
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

        }
    }

    public class JediumTransformSnapshot : JediumBehaviourSnapshot
    {
        //TODO - pack it
        public float X;
        public float Y;
        public float Z;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;
        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;

        public JediumTransformSnapshot()
        {
            X = 0;
            Y = 0;
            Z = 0;
            RotX = 0;
            RotY = 0;
            RotZ = 0;
            RotW = 1;
            ScaleX = 1;
            ScaleY = 1;
            ScaleZ = 1;
        }
    }
}