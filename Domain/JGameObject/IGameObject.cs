using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.BehaviourMessages;

namespace Domain
{
    //"сетевой" 3d обьект
    public partial interface IGameObject : IAbstractActor
    {
       
        Task RegisterClient(Guid clientId, IGameObjectObserver client);

        Task UnregisterClient(Guid clientId);

        Task<string> GetNameOfPrefab();
        Task<string> GetNameOfOthersPrefab();

        Task<string> GetServerAddress();

        Task<ObjectSnapshot> GetSnapshot();

        Task DestroyObject();

        Task<Guid> GetBundleId();

        Task<Guid> GetAvatarId();

        //TODO - stats
        Task<int> GetMessageCount();

        Task SaveToDB();

        Task<string> GetAvatarProps();
        Task SetAvatarProps(string props);
    }

    public interface IGameObjectSelfAccessor
    {
        void SendMessageToRegisteredClients(Guid excludeId, JediumBehaviourMessage message);
        void SendMessagePackToRegisteredClients(Guid excludeId, JediumBehaviourMessage[] messages);
    }

    public class ObjectSnapshot
    {
       public Dictionary<string, JediumBehaviourSnapshot> Snapshots;
    }
}