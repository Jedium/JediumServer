using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
//основное соединение клиент-сервер
    /// <summary>
    /// Client-to-server connection server-side.
    /// </summary>
    public interface IConnection : IAbstractActor
    {
        //тестовый метод
        /// <summary>
        /// Unused
        /// </summary>
        /// <returns></returns>
        Task TestConnection();


        //логин 
        //TODO добавить проверку логина-пароля, возвращать результат
        /// <summary>
        /// Authorizes the client on server.
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="password">Password</param>
        /// <returns>Item1 - if the client may log in, Item2 - the error message, Item3 - ServerInfo structure</returns>
        Task<Tuple<bool, string, ServerInfo>> DoLogin(string username, string password);

        //разлогин
      
        //TODO: check this
        /// <summary>
        /// Logs out the client
        /// </summary>
        /// <param name="clientId">Client GUID.</param>
        /// <returns></returns>
        Task DoLogout(Guid clientId);

        //регистрация клиента на сервере. вызывать с клиента после проверки пароля
        /// <summary>
        /// Registers the client connection on server
        /// </summary>
        /// <param name="clientId">Client GUID</param>
        /// <param name="sceneId">Scene GUID</param>
        /// <param name="client">Client observer</param>
        /// <returns>Scene actor handle.</returns>
        Task<ISceneActor> RegisterClient(Guid clientId, Guid sceneId, IConnectionObserver client);

        /// <summary>
        /// Called on client when Unity loads the scene.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="sceneId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        Task NotifySceneLoaded(Guid clientId, Guid sceneId, string username);

        /// <summary>
        /// Spawn game object on server
        /// </summary>
        /// <param name="namePrefab">Префаб </param>
        /// <param name="nameNotOwnedPrefab"> Префаб у других игроков, если owned</param>
        /// <param name="localID">Object id</param>
        /// <param name="ownerId">Owner id</param>
        /// <param name="bundleId">Bundle id, if not from bundle - Guid.Empty</param>
        /// <param name="obj"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task SpawnGameObject(string namePrefab, string nameNotOwnedPrefab, Guid localID, Guid ownerId, Guid bundleId,
            Guid avatarId, IGameObject obj, string address);

        Task AddLoadedScene(Guid sceneId, ISceneActor scene);
    }

    /// <summary>
    /// Some basic information about the server
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// Behaviours from loaded plugins
        /// </summary>
        public Dictionary<int, string> AdditionalRegisteredBehaviours;
    }
}