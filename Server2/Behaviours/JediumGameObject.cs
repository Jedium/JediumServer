using System;
using System.Collections.Generic;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal.Behaviours;

namespace Server2.Behaviours
{
    public class JediumGameObject : IJediumGameObject
    {
        public readonly Guid LocalId;
        public readonly Guid OwnerId;
        private readonly Dictionary<int, JediumBehaviour> _behaviours;
        public IGameObjectSelfAccessor Actor;

        //Мы предполагаем, что компонент Transform есть всегда
        public JediumGameObject(IGameObjectSelfAccessor actor, List<JediumBehaviourSnapshot> behaviours,
            Dictionary<string, JediumBehaviourDBSnapshot> db_snaps,
            Guid ownerId, Guid localId)
        {
            Actor = actor;
            OwnerId = ownerId;
            LocalId = localId;
            _behaviours = new Dictionary<int, JediumBehaviour>();
          

            foreach (var new_beh in db_snaps)
            {
                JediumBehaviour toAdd = null;


                if (BehaviourTypeRegistry.BehaviourTypes.ContainsKey(new_beh.Key))
                {
                    Type behType = BehaviourTypeRegistry.BehaviourTypes[new_beh.Key];


                    if (behType != null)
                    {
                        JediumBehaviour plugin_beh =
                            (JediumBehaviour) Activator.CreateInstance(behType, this);
                        plugin_beh.FromDBSnapshot(new_beh.Value);
                        toAdd = plugin_beh;
                    }
                }


                if (toAdd != null) _behaviours.Add(TYPEBEHAVIOUR.GetTypeIndex(new_beh.Key), toAdd);
            }


            //OLD DB
            if (behaviours != null)
                foreach (var beh in behaviours)
                    if (BehaviourTypeRegistry.BehaviourTypes.ContainsKey(beh.GetBehaviourType()))
                    {
                        //loaded behaviour
                        string bname = beh.GetBehaviourType();

                        Type behType = BehaviourTypeRegistry.BehaviourTypes[bname];

                        if (behType != null)
                        {
                            JediumBehaviour plugin_beh =
                                (JediumBehaviour) Activator.CreateInstance(behType, this);
                            plugin_beh.FromSnapshot(beh);
                            _behaviours.Add(TYPEBEHAVIOUR.GetTypeIndex(bname), plugin_beh);
                        }
                    }

            //special - transform

            if (!_behaviours.ContainsKey(TYPEBEHAVIOUR.GetTypeIndex("Transform")))
                _behaviours.Add(TYPEBEHAVIOUR.GetTypeIndex("Transform"),
                    new JediumTransform(this, JediumTransformSnapshot.Identity));
        }

        Guid IJediumGameObject.LocalId => LocalId;

        IGameObjectSelfAccessor IJediumGameObject.Actor => Actor;

        Guid IJediumGameObject.OwnerId => OwnerId;

        public void ProcessComponentMessage(Guid clientId, JediumBehaviourMessage message)
        {
            if (_behaviours.ContainsKey(message.GetBehaviourType()))
                _behaviours[message.GetBehaviourType()].ProcessMessage(clientId, message);
        }

        public void ProcessBehaviourMessagePack(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (_behaviours.ContainsKey(messages[0].GetBehaviourType()))
                _behaviours[messages[0].GetBehaviourType()].ProcessMessagePack(clientId, messages);
        }

        public ObjectSnapshot GetSnapshot()
        {
            ObjectSnapshot ret = new ObjectSnapshot();
            
            //component snapshots
            ret.Snapshots = new Dictionary<string, JediumBehaviourSnapshot>();

            foreach (var beh in _behaviours) ret.Snapshots.Add(beh.Value.GetBehaviourType(), beh.Value.GetSnapshot());

            return ret;
        }

        public List<JediumBehaviourDBSnapshot> GetDbSnapshots()
        {
            List<JediumBehaviourDBSnapshot> ret = new List<JediumBehaviourDBSnapshot>();

            foreach (var beh in _behaviours) ret.Add(beh.Value.GetDbSnapshot());

            return ret;
        }
    }
}