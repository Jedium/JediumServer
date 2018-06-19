using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal;
using DomainInternal.Behaviours;
using NLog;
using Server2.Behaviours;

namespace Server2
{
    public partial class ServerGameObject : AbstractActor, IGameObject, IGameObjectSelfAccessor
    {
        private readonly IConnection _connection;

        private readonly Guid _avatarId;

        private readonly string _avatarProps;

        //TODO - move to abstract?
        private readonly Guid _bundleId;
        private readonly Dictionary<Guid, IGameObjectObserver> _clients;
        private readonly IDatabaseAgent _database;
        private readonly JediumGameObject _gameObject;
        private readonly ILogger _log;
        private readonly string _nameNotOwnedPrefab;
        private readonly string _namePrefab;

        private readonly Guid _userId;

        async Task IGameObject.UnregisterClient(Guid clientid)
        {
            _log.Warn("Unregistering client:" + clientid);
            if (_clients.ContainsKey(clientid))
                _clients.Remove(clientid);
        }


        async Task IGameObject.DestroyObject()
        {
            foreach (var cln in _clients) cln.Value.DestroyObject();

            Context.Stop(Self);
            //Self.Tell(InterfacedPoisonPill.Instance,Self);
        }

        async Task IGameObject.SaveToDB()
        {
            _log.Warn("______STORING TO DB");
            PostStop();
            List<JediumBehaviourDBSnapshot> snaps = _gameObject.GetDbSnapshots();
            foreach (var s in snaps) _database.StoreDBBehaviour(s).Wait();
        }

        #region Ctors

        /// <summary>
        ///     Constructs a server-side game object
        /// </summary>
        /// <param name="conn">Connection</param>
        /// <param name="database">Main DB</param>
        /// <param name="namePrefab">Prefab for owner (avatar,for example)</param>
        /// <param name="nameNotOwnedPrefab">Prefab for others (should be same if not owned)</param>
        /// <param name="localId">Object local Id</param>
        /// <param name="ownerID">Owner Id</param>
        /// <param name="bundleId">Bundle id, if object is from bundle</param>
        /// <param name="avatarId">Avatar id</param>
        /// <param name="avProps">Avatar properties. Should be JSON.</param>
        /// <param name="additionalBehavioursNamed">Additional behaviours by typename. Currently unused.</param>
        /// <param name="additionalBehaviours">Collection of additional behaviours by snapshots</param>
        public ServerGameObject(IConnection conn, IDatabaseAgent database, string namePrefab, string nameNotOwnedPrefab,
            Guid localId, Guid ownerID,
            Guid bundleId, Guid avatarId, Guid userId, string avProps,
            List<string> additionalBehavioursNamed, List<JediumBehaviourSnapshot> additionalBehaviours
        ) : base(localId, ownerID)
        {
            _log = LogManager.GetLogger("Object: " + localId + ", prefab:" + namePrefab);

            _bundleId = bundleId;
            _avatarId = avatarId;
            _clients = new Dictionary<Guid, IGameObjectObserver>();
            _connection = conn;
            _database = database;
            _namePrefab = namePrefab;
            _nameNotOwnedPrefab = nameNotOwnedPrefab;
           
            Dictionary<string, JediumBehaviourDBSnapshot> dbsnaps = _database.GetObjectBehaviours(_localID).Result;

            //and for avatar
            if (avatarId != Guid.Empty)
            {
                Dictionary<string, JediumBehaviourDBSnapshot> avsnaps = _database.GetObjectBehaviours(avatarId).Result;

                foreach (var asnap in avsnaps)
                    if (!dbsnaps.ContainsKey(asnap.Key))
                        dbsnaps.Add(asnap.Key, asnap.Value);

                _avatarProps = avProps;
                _userId = userId;
            }

            _gameObject = new JediumGameObject(this, additionalBehaviours, dbsnaps, _ownerID, _localID);


            conn.SpawnGameObject(_namePrefab, _nameNotOwnedPrefab, localId, ownerID, _bundleId, avatarId, this,
                Self.ToString()).Wait();

            MessageNum = 0;
            _log.Info(" is online, address: " + Self);
        }


        public override string ToString()
        {
            return Self.ToString();
        }

        //UNUSED
        public ServerGameObject(IConnection conn, IDatabaseAgent database, string namePrefab) : base(
            GenerateGuids.GetActorGuid(TYPEACTOR.RANDOM), GenerateGuids.GetActorGuid(TYPEACTOR.EMPTY))
        {
            _clients = new Dictionary<Guid, IGameObjectObserver>();
            _connection = conn;
            _database = database;


            conn.SpawnGameObject(namePrefab, namePrefab, _localID, _ownerID, Guid.Empty, Guid.Empty, this,
                Self.ToString()).Wait();
        }

        #endregion

        #region Implement Interface IGameObject

        async Task<Guid> IGameObject.GetBundleId()
        {
            return _bundleId;
        }

        async Task<Guid> IGameObject.GetAvatarId()
        {
            return _avatarId;
        }

        async Task<string> IGameObject.GetNameOfPrefab()
        {
            return _namePrefab;
        }

        async Task<string> IGameObject.GetNameOfOthersPrefab()
        {
            return _nameNotOwnedPrefab;
        }

        async Task<string> IGameObject.GetServerAddress()
        {
            return Self.ToString();
        }

        async Task<ObjectSnapshot> IGameObject.GetSnapshot()
        {
            var snap = _gameObject.GetSnapshot();

            return snap;
        }

        #region Сервер агент обьекта цикл: рассылка состояний всем клиентам

        async Task IGameObject.SendBehaviourMessageToServer(Guid clientId, JediumBehaviourMessage message)
        {
            _gameObject.ProcessComponentMessage(clientId, message);
        }

        async Task IGameObject.SendBehaviourMessagePackToServer(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (MainSettings.CollectMessageStats)
                MessageNum = MessageNum + messages.Length;
            _gameObject.ProcessBehaviourMessagePack(clientId, messages);
        }

        async Task<string> IGameObject.GetAvatarProps()
        {
            return _avatarProps;
        }

        async Task IGameObject.SetAvatarProps(string props)
        {
            _log.Info($"Setting avatar props:{props}");

            _database.UpdateAvatarProps(_userId, props).Wait();
        }


        
        void IGameObjectSelfAccessor.SendMessageToRegisteredClients(Guid excludeId, JediumBehaviourMessage message)
        {
            foreach (var client in _clients)
                if (client.Key != excludeId || excludeId == Guid.Empty)
                    client.Value.SendBehaviourMessageToClient(message);
        }

        void IGameObjectSelfAccessor.SendMessagePackToRegisteredClients(Guid excludeId,
            JediumBehaviourMessage[] messages)
        {
            foreach (var client in _clients)
                if (client.Key != excludeId || excludeId == Guid.Empty)
                    client.Value.SendBehaviourMessagePackToClient(messages);
        }

       

        #endregion


        #region Server: Регистрация объекта с клиентским объектом

        async Task IGameObject.RegisterClient(Guid clientid, IGameObjectObserver client)
        {
            _log.Info("Registering client:" + clientid);

            if (_clients.ContainsKey(clientid))
                _log.Error("Client with this id already exists!");
            else
                _clients.Add(clientid, client);
        }

        #endregion

        #endregion
    }
}