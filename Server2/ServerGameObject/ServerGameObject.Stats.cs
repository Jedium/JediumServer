using System.Threading.Tasks;
using Akka.Interfaced;
using Domain;

namespace Server2
{
    //WIP
    public partial class ServerGameObject : InterfacedActor, IGameObject, IGameObjectSelfAccessor, IAbstractActor
    {
        public int MessageNum;

        Task<int> IGameObject.GetMessageCount()
        {
            int ret = MessageNum;
            MessageNum = 0;
            return Task.FromResult(ret);
        }
    }
}