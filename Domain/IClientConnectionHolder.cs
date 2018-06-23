using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Interfaced;
using Domain.BehaviourMessages;

namespace Domain
{
    public interface IClientConnectionHolder:IInterfacedActor
    {
        Task SendMessagePack(JediumBehaviourMessage[] messages);

     
        Task Stop();
    }

    //TODO - check it later
  // public class BehaviourMessagePack
  // {
  //
  //     public JediumBehaviourMessage[] Messages;
  // }
}
