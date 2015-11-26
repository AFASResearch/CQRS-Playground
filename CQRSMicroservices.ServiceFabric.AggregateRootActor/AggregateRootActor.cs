using CQRSMicroservices.ServiceFabric.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.ServiceFabric.AggregateRootActor
{
  /// <remarks>
  /// Each ActorID maps to an instance of this class.
  /// The IActor1 interface (in a separate DLL that client code can
  /// reference) defines the operations exposed by Actor1 objects.
  /// </remarks>
  internal class AggregateRootActor : StatelessActor, IAggregateRootActor
  {
    private AggregateRoot _aggregateRoot;
    public async Task ExecuteOn(string aggregateRootType, string command)
    {
      var aggregateRoot = LoadAggregateRoot(aggregateRootType);
      aggregateRoot.Handle(Deserialize(command));
      await SaveAndDispatchEvents(aggregateRoot);
    }

    public async Task ExecuteOnNew(string aggregateRootType, string command)
    {
      if(_aggregateRoot != null)
      {
        throw new Exception($"AggregateRoot with id {Id} already exists.");
      }

      var aggregateRoot = LoadAggregateRoot(aggregateRootType);
      aggregateRoot.Handle(Deserialize(command));
      await SaveAndDispatchEvents(aggregateRoot);
    }

    private static Command Deserialize(string command)
    {
      return CqrsApplication.GetService<IDeserializer>().CreateCommand(JObject.Parse(command));
    }

    private AggregateRoot LoadAggregateRoot(string fullTypeName)
    {
      if(_aggregateRoot == null)
      {
        _aggregateRoot = (AggregateRoot)Activator.CreateInstance(Type.GetType(fullTypeName, true));
      }

      return _aggregateRoot;
    }

    private async Task SaveAndDispatchEvents(AggregateRoot root)
    {
      // Here should be the logic to persist this commit to some kind of store.
      // We use a stateless actor, so that means that this actor will be broken when it is pulled out of memory
      var commit = root.Commit();

      // We push these events to the servicebus.
      var eventBusService = ServiceProxy.Create<IEventBusService>(new Uri("fabric:/CQRSMicroservices.ServiceFabric.Application/EventBusService"));
      await Task.WhenAll(commit.Events.ToList().Select(async e => await eventBusService.Dispatch(e.ToJson())));
    }
  }
}
