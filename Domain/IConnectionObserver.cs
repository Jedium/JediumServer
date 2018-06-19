using System;
using Akka.Interfaced;

namespace Domain
{
    //подписка клиента на сообщения с сервера
    public interface IConnectionObserver : IInterfacedObserver
    {
        //оповещаем клиент о том, что другой клиент (или тот же самый) залогинился на сервер
        void ClientLoggedIn(Guid clientid);

        //оповещаем о создании обьекта на сервере
        //вызывать или при создании нового обьекта - для всех подключенных клиентов
        //или при поделючении нового клиента - только для него
        //nameNotOwnedPrefab - имя префаба для других игроков (если owned)
        void OnSpawnedGameObject(string namePrefab, string nameNotOwnedPrefab, Guid localId, Guid ownerId,
            Guid bundleId, Guid avatarId, string address, ObjectSnapshot snap);
    }
}