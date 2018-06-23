using System;
using System.Configuration;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Interfaced;
using Akka.Remote;
using Akka.Util.Internal;
using Domain;

namespace Server2
{
  //  public abstract class AbstractActor : InterfacedActor, IAbstractActor
  //  {
  //      protected AbstractActor(Guid localID, Guid ownerID)
  //      {
  //          _localID = localID;
  //          _ownerID = ownerID;
  //      }
  //
  //      protected Guid _localID { get; }
  //      protected Guid _ownerID { get; }
  //
  //
  //       Task<Guid> IAbstractActor.GetGuid()
  //      {
  //          return Task.FromResult(_localID);
  //      }
  //
  //      Task<Guid> IAbstractActor.GetOwnerId()
  //      {
  //          return Task.FromResult(_ownerID);
  //      }
  //
  //      protected string GetRemoteAddress()
  //      {
  //          Address addr = Context.System.AsInstanceOf<ExtendedActorSystem>().Provider
  //              .AsInstanceOf<RemoteActorRefProvider>().DefaultAddress;
  //
  //          return Self.Path.ToStringWithAddress(addr);
  //      }
  //  }
}