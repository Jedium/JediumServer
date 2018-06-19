using System;
using System.Threading.Tasks;
using Domain.BehaviourMessages;

namespace Domain
{
    public partial interface IGameObject : IAbstractActor
    {
        #region Behaviours

       //TODO - unused. Implement high prority messages
        Task SendBehaviourMessageToServer(Guid clientId, JediumBehaviourMessage message);
        Task SendBehaviourMessagePackToServer(Guid clientId, JediumBehaviourMessage[] messages);

        #endregion
    }
}