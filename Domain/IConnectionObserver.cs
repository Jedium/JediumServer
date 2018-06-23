using System;
using Akka.Interfaced;

namespace Domain
{
    //подписка клиента на сообщения с сервера
    /// <summary>
    /// Client-side connection observer
    /// </summary>
    public interface IConnectionObserver : IInterfacedObserver
    {
        //оповещаем клиент о том, что другой клиент (или тот же самый) залогинился на сервер
        /// <summary>
        /// Notifies the client that another client logged in
        /// </summary>
        /// <param name="clientid"></param>
        void ClientLoggedIn(Guid clientid);

        //оповещаем о создании обьекта на сервере
        //вызывать или при создании нового обьекта - для всех подключенных клиентов
        //или при поделючении нового клиента - только для него
        //nameNotOwnedPrefab - имя префаба для других игроков (если owned)
        /// <summary>
        /// Object is created on server/sent to client
        /// </summary>
        /// <param name="namePrefab"></param>
        /// <param name="nameNotOwnedPrefab"></param>
        /// <param name="localId"></param>
        /// <param name="ownerId"></param>
        /// <param name="bundleId"></param>
        /// <param name="avatarId"></param>
        /// <param name="address"></param>
        /// <param name="snap"></param>
        void OnSpawnedGameObject(string namePrefab, string nameNotOwnedPrefab, Guid localId, Guid ownerId,
            Guid bundleId, Guid avatarId, string address, ObjectSnapshot snap);

        void KillOwnedObjects(Guid clientId);
    }
}