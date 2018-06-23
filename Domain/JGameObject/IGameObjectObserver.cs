using Akka.Interfaced;

namespace Domain
{
    //подписка 3d обьекта на сообщения от серверной части
    public partial interface IGameObjectObserver : IInterfacedObserver
    {

        void GotAddress();
        void DestroyObject();
    }
}