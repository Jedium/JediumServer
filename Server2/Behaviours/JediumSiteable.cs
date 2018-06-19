using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using NLog;

namespace Server2.Behaviours
{
    public class JediumSiteable: JediumBehaviour
    {


        private ILogger Log;


        public float posX;
        public float posY;
        public float posZ;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;


        public JediumSiteable(JediumGameObject _parent):base(_parent)
        {
            Log = LogManager.GetLogger("Siteable: " + _parent.LocalId);
        }

        public override void FromDBSnapshot(JediumBehaviourDBSnapshot snap)
        {
            JediumSiteableDBSnapshot sitDBsnap = (JediumSiteableDBSnapshot)snap;

            posX = sitDBsnap.X;
            posY = sitDBsnap.Y;
            posZ = sitDBsnap.Z;
            RotX = sitDBsnap.RotX;
            RotY = sitDBsnap.RotY;
            RotZ = sitDBsnap.RotZ;
            RotW = sitDBsnap.RotW;


        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetBehaviourType() != "Sit")
            {
                Log.Warn($"Wrong snapshot type: {snap.GetBehaviourType()}");
            }

            JediumSitSnapshot sitSnap = (JediumSitSnapshot)snap;

            posX = sitSnap.X;
            posY = sitSnap.Y;
            posZ = sitSnap.Z;
            RotX = sitSnap.RotX;
            RotY = sitSnap.RotY;
            RotZ = sitSnap.RotZ;
            RotW = sitSnap.RotW;
        }

        public override int GetBehaviourIndex()
        {
            return TYPEBEHAVIOUR.GetTypeIndex("Sit");
        }

        public override string GetBehaviourType()
        {
            return "Sit";
        }

        public override JediumBehaviourDBSnapshot GetDbSnapshot()
        {
            return new JediumSiteableDBSnapshot()
            {
                LocalId = _parent.LocalId,
                Type = "Sit",
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
            return new JediumSitSnapshot(_parent.LocalId, posX, posY, posZ, RotX, RotY, RotZ, RotW);
        }

        public override void ProcessMessage(Guid clientId, JediumBehaviourMessage message)
        {
            if(message.GetType()!=typeof(JediumSitMessage))
            {
                return;
            }

            _parent.Actor.SendMessageToRegisteredClients(clientId, message);
        }

        public override void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (messages[0].GetBehaviourType() != GetBehaviourIndex())
                return;

            _parent.Actor.SendMessagePackToRegisteredClients(Guid.Empty, messages);
        }
    }



    public class JediumSiteableDBSnapshot:JediumBehaviourDBSnapshot
    {
        public float X;
        public float Y;
        public float Z;
        public float RotX;
        public float RotY;
        public float RotZ;
        public float RotW;
    }
}
