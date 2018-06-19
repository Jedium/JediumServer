using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Scene actor.
    /// </summary>
    public interface ISceneActor : IAbstractActor
    {
        Task<string> GetServerName();
        Task<string> GetSceneName();
        Task LoadSceneObjects();

        Task PushObjectsToClient(Guid clientId, IConnectionObserver client);

        Task LogoutClient(Guid clientId);

        #region Stats 

        Task<List<Tuple<Guid, string>>> GetSceneObjects();

        #endregion

        Task SaveObjectsToDB();

        Task<Guid> GetBundleId();
    }
}