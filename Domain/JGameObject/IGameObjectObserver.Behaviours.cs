using Akka.Interfaced;
using Domain.BehaviourMessages;

namespace Domain
{
    public partial interface IGameObjectObserver : IInterfacedObserver
    {
        #region Behaviours

        void SendBehaviourMessageToClient(JediumBehaviourMessage message);

        void SendBehaviourMessagePackToClient(JediumBehaviourMessage[] messages);

        #endregion
    }
}