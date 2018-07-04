using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Interfaced;
using Akka.Remote;
using Akka.Util.Internal;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal;
using DomainInternal.Behaviours;
using NLog;
using Server2.Behaviours;

namespace Server2
{
    public partial class ServerGameObject : InterfacedActor, IGameObject, IGameObjectSelfAccessor,IAbstractActor
    {
        private readonly IConnection _connection;

        private readonly Guid _avatarId;

        private readonly Guid _localId;
        private readonly Guid _ownerId;

        private readonly string _avatarProps;

        //private const int MAX_PACKETS = 10;

      
        private readonly Guid _bundleId;
        private readonly Dictionary<Guid, ClientConnectionHolderRef> _clients;
        private readonly DatabaseAgentRef _database;
        private readonly JediumGameObject _gameObject;
        private readonly ILogger _log;
        private readonly string _nameNotOwnedPrefab;
        private readonly string _namePrefab;

        private readonly Guid _userId;


        Task IGameObject.UnregisterClient(Guid clientid)
        {
            _log.Warn("Unregistering client:" + clientid);
            if (_clients.ContainsKey(clientid))
                _clients.Remove(clientid);

            return Task.FromResult(true);
        }


         Task IGameObject.DestroyObject()
        {
            foreach (var cln in _clients)
        {
                Console.WriteLine("___DESTROY0 FOR:" + cln.Key);
                cln.Value.Stop().Wait();
            }
         

            Context.Stop(Self);
            return Task.FromResult(true);
        }

         Task IGameObject.SaveToDB()
        {
            PostStop();
            List<JediumBehaviourDBSnapshot> snaps = _gameObject.GetDbSnapshots();
            foreach (var s in snaps)
            {
                _log.Info($"Saving to DB: {s.LocalId}, {s.Type}");
                _database.StoreDBBehaviour(s).Wait();
            }

            return Task.FromResult(true);
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
        public ServerGameObject(IConnection conn, DatabaseAgentRef database, string namePrefab, string nameNotOwnedPrefab,
            Guid localId, Guid ownerID,
            Guid bundleId, Guid avatarId, Guid userId, string avProps,
            List<string> additionalBehavioursNamed, List<JediumBehaviourSnapshot> additionalBehaviours
        ) 
        {
            _log = LogManager.GetLogger("Object: " + localId + ", prefab:" + namePrefab);
            _localId = localId;
            _bundleId = bundleId;
            _avatarId = avatarId;
            _clients = new Dictionary<Guid, ClientConnectionHolderRef>();
            _connection = conn;
            _database = database;
            _namePrefab = namePrefab;
            _nameNotOwnedPrefab = nameNotOwnedPrefab;
            _ownerId = ownerID;


           
            Dictionary<string, JediumBehaviourDBSnapshot> dbsnaps = _database.GetObjectBehaviours(_localId).Result;

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

            _gameObject = new JediumGameObject(this, additionalBehaviours, dbsnaps, _ownerId, _localId);


            conn.SpawnGameObject(_namePrefab, _nameNotOwnedPrefab, localId, ownerID, _bundleId, avatarId, this
).Wait();

            MessageNum = 0;
            _log.Info(" is online, address: " + GetRemoteAddress());

           
        }

        protected string GetRemoteAddress()
              {
                  Address addr = Context.System.AsInstanceOf<ExtendedActorSystem>().Provider
                      .AsInstanceOf<RemoteActorRefProvider>().DefaultAddress;
        
                  return Self.Path.ToStringWithAddress(addr);
        }


        public override string ToString()
        {
            return Self.ToString();
        }



        #endregion

        #region Implement Interface IGameObject

        Task<Guid> IGameObject.GetBundleId()
        {
            return Task.FromResult(_bundleId);
        }

        async Task<Guid> IGameObject.GetAvatarId()
        {
            return _avatarId;
        }

        Task<string> IGameObject.GetNameOfPrefab()
        {
            return  Task.FromResult(_namePrefab);
        }

        Task<string> IGameObject.GetNameOfOthersPrefab()
        {
            return Task.FromResult(_nameNotOwnedPrefab);
        }

        Task<string> IGameObject.GetServerAddress()
        {

            string defaddress = GetRemoteAddress();//Self.ToString();
           
            return Task.FromResult(defaddress);
        }

        Task<ObjectSnapshot> IGameObject.GetSnapshot()
        {
            var snap = _gameObject.GetSnapshot();

            return Task.FromResult(snap);
        }

        #region Сервер агент обьекта цикл: рассылка состояний всем клиентам

        //todo - maybe handler
        Task IGameObject.SendBehaviourMessageToServer(Guid clientId, JediumBehaviourMessage message)
        {
            _gameObject.ProcessComponentMessage(clientId, message);

            return Task.FromResult(true);
        }

        Task IGameObject.SendBehaviourMessagePackToServer(Guid clientId, JediumBehaviourMessage[] messages)
        {
            if (MainSettings.CollectMessageStats)
                MessageNum = MessageNum + messages.Length;

           
            _gameObject.ProcessBehaviourMessagePack(clientId, messages);

            return Task.FromResult(true);
        }

        

         Task<string> IGameObject.GetAvatarProps()
        {
            return Task.FromResult(_avatarProps);
        }

        Task IGameObject.SetAvatarProps(string props)
        {
            _log.Info($"Setting avatar props:{props}");

            _database.UpdateAvatarProps(_userId, props).Wait();

            return Task.FromResult(true);
        }

        void IGameObjectSelfAccessor.SendMessagePackToProcess(Guid clientId, JediumBehaviourMessage[] msg)
        {

            _gameObject.ProcessBehaviourMessagePack(clientId, msg);
        }
        
        //UNUSED
        void IGameObjectSelfAccessor.SendMessageToRegisteredClients(Guid excludeId, JediumBehaviourMessage message)
        {
           // foreach (var client in _clients)
           //     if (client.Key != excludeId || excludeId == Guid.Empty)
           //         client.Value.SendBehaviourMessageToClient(message);
        }

        void IGameObjectSelfAccessor.SendMessagePackToRegisteredClients(Guid excludeId,
            JediumBehaviourMessage[] messages)
        {
            
           
            foreach (var client in _clients)
                if (client.Key != excludeId)
                {
                   client.Value.CastToIActorRef().Tell(new PackToConnection()
                   {
                       Messages = messages
                   });
                }
                
        }

        public class PackToConnection
        {

            public JediumBehaviourMessage[] Messages;
        }

        //UNUSED
        Task IGameObject.TickBehaviours()
        {
            
            return Task.FromResult(true);
        }

        public Task SetBehaviourSnapshot(JediumBehaviourSnapshot snapshot)
        {
            _log.Info($"Setting snapshot for {snapshot.BehaviourType}");
            _gameObject.MarkedForSave = true;
            _gameObject.SetBehaviourFromSnapshot(snapshot);
            List<JediumBehaviourDBSnapshot> snaps = _gameObject.GetDbSnapshots();
            foreach (var s in snaps)
            {
                _log.Info($"Saving to DB: {s.LocalId}, {s.Type}");
                _database.StoreDBBehaviour(s).Wait();
            }
       
            return Task.FromResult(true);
        }

        #endregion


        #region Server: Регистрация объекта с клиентским объектом

         Task IGameObject.RegisterClient(Guid clientid, IGameObjectObserver client)
        {
            _log.Info("Registering client:" + clientid);

            if (_clients.ContainsKey(clientid))
                _log.Error("Client with this id already exists!");
            else
            {
                var holder = Context.ActorOf(Props.Create(() => new ClientConnectionHolder(client, clientid,this)))
                    .Cast<ClientConnectionHolderRef>();
                _clients.Add(clientid, holder);
            }

            return Task.FromResult(true);
        }

        #endregion

        #endregion

        Task<Guid> IAbstractActor.GetGuid()
        {
            return Task.FromResult(_localId);
        }

        Task<Guid> IAbstractActor.GetOwnerId()
        {
            return Task.FromResult(_ownerId);
        }
    }
}