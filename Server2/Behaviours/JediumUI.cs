using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server2.Behaviours
{
    public class JediumUI : JediumBehaviour
    {
        private readonly ILogger Log;

        public string dllName;
        public string xamlName;
        public Guid _bundleId;
        public string archiveName;

        public JediumUI(JediumGameObject _parent) : base(_parent)
        {
            Log = LogManager.GetLogger("UI: " + _parent.LocalId);
            dllName = String.Empty;
            xamlName = String.Empty;
            _bundleId = Guid.Empty;
            archiveName = String.Empty;
        }

        public JediumUI(JediumGameObject parent, JediumTransformSnapshot snap) : base(parent)
        {
            Log = LogManager.GetLogger("Transform: " + parent.LocalId);
            FromSnapshot(snap);
        }

        public override void FromDBSnapshot(JediumBehaviourDBSnapshot snap)
        {
            JediumUIDBSnapshot UIDBsnap = (JediumUIDBSnapshot)snap;


            this.dllName = UIDBsnap.dllName;
            this._bundleId = UIDBsnap._bundleId;
            this.xamlName = UIDBsnap.xamlName;
            this.archiveName = UIDBsnap.archiveName;
        }

        public override void FromSnapshot(JediumBehaviourSnapshot snap)
        {
            if (snap.GetBehaviourType() != "UI")
            {
                Log.Warn($"Wrong snapshot type: {snap.GetBehaviourType()}");
            }

            JediumUISnapshot UISnap = (JediumUISnapshot)snap;

            this.dllName = UISnap.dllName;
            this._bundleId = UISnap.bundleId;
            this.xamlName = UISnap.xamlName;
            this.archiveName = UISnap.archiveName;
        }

        public override int GetBehaviourIndex()
        {
            return TYPEBEHAVIOUR.GetTypeIndex("UI");
        }

        public override string GetBehaviourType()
        {
            return "UI";
        }

        public override JediumBehaviourDBSnapshot GetDbSnapshot()
        {
            return new JediumUIDBSnapshot()
            {
                LocalId = _parent.LocalId,
                Type = "UI",
                dllName = this.dllName,
                _bundleId = this._bundleId,
                xamlName = this.xamlName,
                archiveName = this.archiveName
                
                
            };
        }

        public override JediumBehaviourSnapshot GetSnapshot()
        {
            return new JediumUISnapshot(_parent.LocalId, dllName, _bundleId, xamlName, archiveName);
        }

        public override void ProcessMessage(Guid clientId, JediumBehaviourMessage message)
        {
            if (message.GetType() != typeof(JediumUIMessage))
                return;

            _parent.Actor.SendMessageToRegisteredClients(Guid.Empty, message);
        }

        public override void ProcessMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (messages[0].GetBehaviourType() != GetBehaviourIndex())
                return;

            _parent.Actor.SendMessagePackToRegisteredClients(Guid.Empty, messages);
        }
    }

    public class JediumUIDBSnapshot : JediumBehaviourDBSnapshot
    {
        public string dllName;
        public string xamlName;
        public Guid _bundleId;
        public string archiveName;
    }

}
