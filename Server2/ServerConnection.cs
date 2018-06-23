using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Interfaced;
using Common.Logging;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal;

namespace Server2
{
    public class ServerConnection : InterfacedActor, IConnection
    {
        private readonly ILog _logger;

        private readonly Dictionary<Guid, Tuple<Guid, IGameObject>> _allClientObjects;

        private readonly Dictionary<Guid, IConnectionObserver> _clients;


        private readonly DatabaseAgentRef _database;

        private readonly Dictionary<Guid, ISceneActor> _loadedScenes;

      //  private readonly Dictionary<Guid, IGameObject>
       //     _spawnedObjects; //todo: move to.... or keep as objects per connection?


        private List<Guid> _loggedInUsers;



        #region ctrors

        public ServerConnection(DatabaseAgentRef database) 
        {
            _loggedInUsers=new List<Guid>();
            _allClientObjects = new Dictionary<Guid, Tuple<Guid, IGameObject>>();
            _database = database;
            _logger = LogManager.GetLogger("[Connection]");
            _clients = new Dictionary<Guid, IConnectionObserver>();
          //  _spawnedObjects = new Dictionary<Guid, IGameObject>();
            _loadedScenes = new Dictionary<Guid, ISceneActor>();
            _logger.Info("Server connection initialized");
        }

        #endregion

        public Task<Guid> GetGuid()
        {
            return Task.FromResult(Guid.Empty);
        }

        public Task<Guid> GetOwnerId()
        {
            return Task.FromResult(Guid.Empty);
        }


        void CreateUserAvatar(Guid ownerId, DatabaseUser user)
        {
            var avatar = _database.GetAvatar(user.AvatarId).Result;

            Guid id = Guid.NewGuid();
            List<JediumBehaviourSnapshot> abehaviours = new List<JediumBehaviourSnapshot>(1);
            // abehaviours.Add(new JediumCharacterControllerSnapshot(id));
            List<string> namedBehaviours = new List<string>(1);
            namedBehaviours.Add("TestBehaviour");
            var obj = Context.ActorOf(Props.Create(() => new ServerGameObject(this, _database, avatar.UserAvatar,
                        avatar.OtherAvatar, ownerId, ownerId,
                        Guid.Empty, avatar.AvatarId, user.UserId, user.AvatarProps,
                        namedBehaviours, abehaviours)),
                    ownerId + "_" + Guid.NewGuid())
                .Cast<GameObjectRef>();

            _allClientObjects.Add(id, Tuple.Create<Guid, IGameObject>(ownerId, obj));
        }

        #region Implement interface IConnection 

         Task IConnection.TestConnection()
        {
            _logger.Warn("TEST CLIENT CONNECTION");
            return Task.FromResult(true);
        }


        async Task<Tuple<bool, string, ServerInfo>> IConnection.DoLogin(string username, string password)
        {
            //check user

            var user = await _database.GetUserByName(username);

            if (user == null) return Tuple.Create<bool, string, ServerInfo>(false, $"User {username} not found", null);

            if (user.Password != password)
                return Tuple.Create<bool, string, ServerInfo>(false, $"Wrong password for user {username}", null);
            if(_loggedInUsers.Contains(user.UserId))
                return Tuple.Create<bool, string, ServerInfo>(false, $"User {username} already logged in", null);

            ServerInfo info = new ServerInfo();
            info.AdditionalRegisteredBehaviours = TYPEBEHAVIOUR.AdditionalBehaviours;
            info._loggedInUserId = user.UserId;

            _loggedInUsers.Add(user.UserId);
            return Tuple.Create(true, "TestServer", info);
        }

        Task<ISceneActor> IConnection.RegisterClient(Guid clientid, Guid sceneId, IConnectionObserver client)
        {
            _logger.Info("Registering client:" + clientid);
            _logger.Warn("SceneId:" + sceneId);


            #region проверка существования сцены

            if (!_loadedScenes.ContainsKey(sceneId))
            {
                _logger.Warn($"Trying to connect to scene {sceneId}, which is not loaded!");
                return null;
            }

            #endregion

            #region Рассылка сообщения о логине

            _clients.Add(clientid, client);

            #endregion

            //TODO - track disconnected status
            //Context.Watch(Sender);

            return Task.FromResult(_loadedScenes[sceneId]);
        }

      


        async Task IConnection.NotifySceneLoaded(Guid clientId, Guid sceneId, string username)
        {
            #region Сервер: вызов метода спавна обьектов комнаты

            await _loadedScenes[sceneId].PushObjectsToClient(clientId, _clients[clientId]);

            #endregion

            foreach (var o in _allClientObjects)
                _clients[clientId].OnSpawnedGameObject(o.Value.Item2.GetNameOfPrefab().Result,
                    o.Value.Item2.GetNameOfOthersPrefab().Result, o.Key, o.Value.Item1,
                    o.Value.Item2.GetBundleId().Result,
                    o.Value.Item2.GetAvatarId().Result, o.Value.Item2.GetServerAddress().Result,
                    o.Value.Item2.GetSnapshot().Result);

            #region Сервер: спавн аватара

            var user = await _database.GetUserByName(username);

            CreateUserAvatar(clientId, user);

            #endregion

            foreach (var cln in _clients) cln.Value.ClientLoggedIn(clientId);
        }


        #region Сервер: уничтожение обьекта аватара, рассылка сообщения всем клиентам

    

        [MessageHandler]
        void HandleLogout(LogoutMessage msg)
        {
            Console.WriteLine("___HANDLE LOGOUT");
            if (_clients.ContainsKey(msg.ClientId))
            {
                //TODO - send the scene here too
                    _clients.Remove(msg.ClientId);


                    foreach (var scene in _loadedScenes)
                        scene.Value.LogoutClient(msg.ClientId).Wait();

                List<Guid> otorem = new List<Guid>();

                foreach (var oobj in _allClientObjects)
                {
                        oobj.Value.Item2.UnregisterClient(msg.ClientId).Wait();
                        if (oobj.Value.Item1 == msg.ClientId) otorem.Add(oobj.Key);
                }

                foreach (var rid in otorem)
                {
                        _allClientObjects[rid].Item2.DestroyObject().Wait();
                    _allClientObjects.Remove(rid);
                }

                    foreach (var client in _clients)
                    {
                        client.Value.KillOwnedObjects(msg.ClientId);
                    }
            
                    _loggedInUsers.Remove(msg.UserId);
                    _logger.Info($"Client - {msg.ClientId} is removed");
            }
            else
            {
                    _logger.Warn($"Not exists {msg.ClientId} client");
            }
        }

        #endregion

        Task IConnection.AddLoadedScene(Guid sceneId, ISceneActor scene)
        {
            _loadedScenes.Add(sceneId, scene);

            return Task.FromResult(true);
        }

         Task IConnection.SpawnGameObject(string namePrefab, string nameNotOwnedPrefab, Guid localID, Guid ownerId,
            Guid bundleId, Guid avatarId, IGameObject obj)
        {
            _logger.Info("Spawning object:" + localID);

           // _spawnedObjects.Add(localID, obj);

            Console.WriteLine("LocalId: " + localID);

            foreach (var cln in _clients)
                cln.Value.OnSpawnedGameObject(namePrefab, nameNotOwnedPrefab, localID, obj.GetOwnerId().Result,
                    bundleId, obj.GetAvatarId().Result,
                    obj.GetServerAddress().Result, obj.GetSnapshot().Result);

            return Task.FromResult(true);
        }

        #endregion
    }
}