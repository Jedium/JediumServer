using System.Threading.Tasks;
using Domain;

namespace Server2
{
    //WIP
    public partial class ServerGameObject : AbstractActor, IGameObject, IGameObjectSelfAccessor
    {
        public int MessageNum;

        async Task<int> IGameObject.GetMessageCount()
        {
            int ret = MessageNum;
            MessageNum = 0;
            return ret;
        }
    }
}