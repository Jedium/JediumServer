using System;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NLog;

namespace Server2.Behaviours
{
    /// <summary>
    ///     для упаковки и передачи данных
    /// </summary>
    public class JediumTransform : JediumBehaviour
    {
        private readonly ILogger _log;

        // Postion 
        public float _posX;
        public float _posY;
        public float _posZ;
        public float _quatW;

        // Rotation 
        public float _quatX;
        public float _quatY;
        public float _quatZ;

        // Scale 
        public float _scaleX;
        public float _scaleY;
        public float _scaleZ;


        public string NamePrefab;

       
        public JediumTransform(JediumGameObject parent) : base(parent)
        {
            _log = LogManager.GetLogger("Transform: " + parent.LocalId);
            _posX = 0;
            _posY = 0;
            _posZ = 0;
            _quatX = 0;
            _quatY = 0;
            _quatZ = 0;
            _quatW = 1;
            _scaleX = 1;
            _scaleY = 1;
            _scaleZ = 1;
        }

        public JediumTransform(JediumGameObject parent, JediumTransformSnapshot snap) : base(parent)
        {
            _log = LogManager.GetLogger("Transform: " + parent.LocalId);
            FromSnapshot(snap);
        }

        public override string ToString()
        {
            return $"Pos: {_posX},{_posY},{_posZ}";
        }

        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumTransformSnapshot(_parent.LocalId,
                _posX, _posY, _posZ,
                _quatX, _quatY, _quatZ, _quatW,
                _scaleX, _scaleY, _scaleZ);
            
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetBehaviourType() != "Transform")
            {
                _log.Warn($"Wrong snapshot type: {snap.GetBehaviourType()}");
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

        //unused
        public override void ProcessMessage(Guid clientId, JediumBehaviourMessage message)
        {
         //  if (message.GetType() != typeof(JediumTransformMessage))
         //      return;
         //
         //
         //  JediumTransformMessage amsg = (JediumTransformMessage) message;
         //
         //
         //  _posX = amsg.X;
         //  _posY = amsg.Y;
         //  _posZ = amsg.Z;
         //  _quatX = amsg.RotX;
         //  _quatY = amsg.RotY;
         //  _quatZ = amsg.RotZ;
         //  _quatW = amsg.RotW;
         //  _scaleX = amsg.ScaleX;
         //  _scaleY = amsg.ScaleY;
         //  _scaleZ = amsg.ScaleZ;
         //
         // 
         //  _parent.Actor.SendMessageToRegisteredClients(_parent.OwnerId, message); //.Wait();
        }

        public override string GetBehaviourType()
        {
            return "Transform";
        }

        //cache it!
        public override int GetBehaviourIndex()
        {
            return 0; //TYPEBEHAVIOUR.GetTypeIndex("Transform");
        }

        public override void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (messages[0].GetBehaviourType() != GetBehaviourIndex())
                return;
            //we don't need to process all messages
            JediumTransformMessage amsg = (JediumTransformMessage) messages[messages.Length - 1];

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

            _parent.Actor.SendMessagePackToRegisteredClients(Guid.Empty, messages);
        }

        public override JediumBehaviourDBSnapshot GetDbSnapshot()
        {
            return new JediumTransformDBSnapshot
            {
                LocalId = _parent.LocalId,
                Type = "Transform",
                X = _posX,
                Y = _posY,
                Z = _posZ,
                RotX = _quatX,
                RotY = _quatY,
                RotZ = _quatZ,
                RotW = _quatW,
                ScaleX = _scaleX,
                ScaleY = _scaleY,
                ScaleZ = _scaleZ
            };
        }

        public override void FromDBSnapshot(JediumBehaviourDBSnapshot snap)
        {
            if (snap.Type != "Transform")
            {
                _log.Warn("Trying to deserialize from wrong snapshot type: " + snap.Type);
                return;
            }

            JediumTransformDBSnapshot s = (JediumTransformDBSnapshot) snap;

            _posX = s.X;
            _posY = s.Y;
            _posZ = s.Z;
            _quatX = s.RotX;
            _quatY = s.RotY;
            _quatZ = s.RotZ;
            _quatW = s.RotW;
            _scaleX = s.ScaleX;
            _scaleY = s.ScaleY;
            _scaleZ = s.ScaleZ;
        }
    }

    public class JediumTransformDBSnapshot : JediumBehaviourDBSnapshot
    {
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float RotW;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float RotX;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float RotY;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float RotZ;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float ScaleX;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float ScaleY;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float ScaleZ;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float X;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float Y;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float Z;
    }
}