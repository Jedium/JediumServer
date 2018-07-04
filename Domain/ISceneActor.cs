using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Interfaced;
using Domain.BehaviourMessages;

namespace Domain
{
    /// <summary>
    /// Scene actor.
    /// </summary>
    public interface ISceneActor : IInterfacedActor
    {
        Task<string> GetServerName();
        Task<string> GetSceneName();
        Task LoadSceneObjects();

        Task PushObjectsToClient(Guid clientId, IConnectionObserver client);

        Task LogoutClient(Guid clientId);

        Task<string> GetActorAddress();

        #region Stats 

        Task<List<Tuple<Guid, string>>> GetSceneObjects();

        //new object from editor
        Task AddSceneObject(Guid localId, Guid prefabId, List<JediumBehaviourSnapshot> snapshots);

        Task DeleteSceneObject(Guid localId);

        #endregion

        /// <summary>
        /// Gets object details
        /// </summary>
        /// <param name="localId"></param>
        /// <returns>Tuple (bundleid, snapshot)</returns>
        Task<Tuple<Guid, ObjectSnapshot>> GetObjectInfo(Guid localId);

        Task SetObjectBehaviour(Guid localId, JediumBehaviourSnapshot snap);

        Task SaveObjectsToDB();

        Task TestConnection();

        Task<Guid> GetBundleId();
    }
}