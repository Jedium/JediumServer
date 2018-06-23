using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server2.Behaviours
{
    public class JediumTakeable : JediumBehaviour
    {
        private ILogger Log;


        public float posX;
        public float posY;
        public float posZ;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;


        public JediumTakeable(JediumGameObject _parent):base(_parent)
        {
            Log = LogManager.GetLogger("Takeable: " + _parent.LocalId);

        }

        public override void FromDBSnapshot(JediumBehaviourDBSnapshot snap)
        {

            JediumTakeableDBSnapshot takeDBsnap = (JediumTakeableDBSnapshot)snap;


            posX = takeDBsnap.X;
            posY = takeDBsnap.Y;
            posZ = takeDBsnap.Z;
            RotX = takeDBsnap.RotX;
            RotY = takeDBsnap.RotY;
            RotZ = takeDBsnap.RotZ;
            RotW = takeDBsnap.RotW;
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetBehaviourType() != "Take")
            {
                Log.Warn($"Wrong snapshot type: {snap.GetBehaviourType()}");
            }

            JediumTakeSnapshot takeSnap = (JediumTakeSnapshot)snap;

            posX = takeSnap.X;
            posY = takeSnap.Y;
            posZ = takeSnap.Z;
            RotX = takeSnap.RotX;
            RotY = takeSnap.RotY;
            RotZ = takeSnap.RotZ;
            RotW = takeSnap.RotW;
        }

        public override int GetBehaviourIndex()
        {
            return TYPEBEHAVIOUR.GetTypeIndex("Take");
        }

        public override string GetBehaviourType()
        {
            return "Take";
        }

        public override JediumBehaviourDBSnapshot GetDbSnapshot()
        {
            return new JediumTakeableDBSnapshot()
            {
                LocalId = _parent.LocalId,
                Type = "Take",
                    X = posX,
                    Y = posY,
                    Z = posZ,
                    RotX = RotX,
                    RotY = RotY,
                    RotZ = RotZ,
                    RotW = RotW
            };

            
        }


        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumTakeSnapshot(_parent.LocalId, posX, posY, posZ, RotX, RotY, RotZ, RotW);
        }

        public override void ProcessMessage(Guid clientId, JediumBehaviourMessage message)
        {
            if (message.GetType() != typeof(JediumTakeMessage))
                return;

            _parent.Actor.SendMessageToRegisteredClients(Guid.Empty, message);
        }

        public override void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if(messages[0].GetBehaviourType()!=GetBehaviourIndex())
                return;

            _parent.Actor.SendMessagePackToRegisteredClients(Guid.Empty, messages);
        }
    }


    public class JediumTakeableDBSnapshot : JediumBehaviourDBSnapshot
    {
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float X;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float Y;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float Z;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float RotX;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float RotY;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float RotZ;
        [BsonRepresentation(BsonType.Double, AllowTruncation = true)]
        public float RotW;
    }
}
