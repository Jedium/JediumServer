using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Interfaced;
using Domain;
using Domain.BehaviourMessages;
using DomainInternal;


namespace Server2
{
   public class ClientConnectionHolder:InterfacedActor,IClientConnectionHolder
   {

       private readonly IGameObjectObserver _client;
       private readonly IGameObjectSelfAccessor _parent;
       private readonly Guid _clientId;

       private bool _working;

       private Queue<JediumBehaviourMessage[]> _messageQueue;

        internal sealed class ResendMessage
        {

        }

        public ClientConnectionHolder(IGameObjectObserver client,Guid clientId, IGameObjectSelfAccessor parent)
        {
            _working = true;
            _client = client;
            _parent = parent;
            _messageQueue=new Queue<JediumBehaviourMessage[]>();
            _clientId = clientId;

            _client.GotAddress();
          Self.Tell(new ResendMessage(),null);
        }

        //TODO - maybe direct handler
        //Direct handler: error NotSupportedException: There is no active ActorContext, this is most likely due to use of async operations from within this actor.
       //Rewrite to non-interfaced or?
      public Task SendMessagePack(JediumBehaviourMessage[] messages)
       {
          
           _parent.SendMessagePackToProcess(_clientId, messages);
           return Task.FromResult(true);
      }

       //TODO - see above
      // [Reentrant]
      // [MessageHandler]
      //void HandleIncoming(Domain.BehaviourMessagePack pack)
      //{
      //    Console.WriteLine("___HANDLING");
      //    _parent.SendMessagePackToProcess(_clientId, pack.Messages);
      // }

    

       [MessageHandler]
      void HandleMsg(ServerGameObject.PackToConnection pack)
       {
        
           _messageQueue.Enqueue(pack.Messages);
        }


        Task IClientConnectionHolder.Stop()
       {
           Console.WriteLine("___DESTROY1 FOR:" + _clientId);
            // Console.WriteLine("___STOP ON "+_clientId);
            _working = false;

          // Console.WriteLine("___DESTROY FOR:" + _clientId);
          // _client.DestroyObject();
          // Self.GracefulStop(TimeSpan.FromSeconds(100), InterfacedPoisonPill.Instance).Wait();
          //
            return Task.FromResult(true);

       }
       [MessageHandler]
       void Handle(ResendMessage msg)
       {
          
            

         //  Console.WriteLine("______SENDING");
           if (_messageQueue.Count > 0&&_working)
           {
               int tosend = _messageQueue.Count > 10 ? 10 : _messageQueue.Count;

               for (int i = 0; i < tosend; i++)
               {

                   var msnd = _messageQueue.Dequeue();
                   
                       _client.SendBehaviourMessagePackToClient(msnd);
               }
           }

           if (_working)
           {
               RunTask(async () =>
               {
                   await Task.Delay(MainSettings.TickDelay);
                   Self.Tell(new ResendMessage(), null);

               }, isReentrant: true);
           }
           else
           {
               Console.WriteLine("___DESTROY FOR:"+_clientId);
            //   _client.DestroyObject();
               Self.GracefulStop(TimeSpan.FromSeconds(100), InterfacedPoisonPill.Instance).Wait();
           }
       }
   }
}
