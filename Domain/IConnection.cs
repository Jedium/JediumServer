using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
//основное соединение клиент-сервер
    public interface IConnection : IAbstractActor
    {
        //тестовый метод
        Task TestConnection();


        //логин 
        //TODO добавить проверку логина-пароля, возвращать результат
        Task<Tuple<bool, string, ServerInfo>> DoLogin(string username, string password);

        //разлогин
        //не используется и не факт, что надо
        //TODO: check this
        Task DoLogout(Guid clientId);

        //регистрация клиента на сервере. вызывать с клиента после проверки пароля
        Task<ISceneActor> RegisterClient(Guid clientId, Guid sceneId, IConnectionObserver client);

        Task NotifySceneLoaded(Guid clientId, Guid sceneId, string username);

        /// <summary>
        /// </summary>
        /// <param name="namePrefab">Префаб </param>
        /// <param name="nameNotOwnedPrefab"> Префаб у других игроков, если owned</param>
        /// <param name="localID"></param>
        /// <param name="ownerId"></param>
        /// <param name="bundleId"></param>
        /// <param name="obj"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task SpawnGameObject(string namePrefab, string nameNotOwnedPrefab, Guid localID, Guid ownerId, Guid bundleId,
            Guid avatarId, IGameObject obj, string address);

        Task AddLoadedScene(Guid sceneId, ISceneActor scene);
    }

    public class ServerInfo
    {
        public Dictionary<int, string> AdditionalRegisteredBehaviours;
    }
}