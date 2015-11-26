using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using CQRSMicroservices.ServiceFabric.Interfaces;
using Microsoft.ServiceFabric.Actors;

namespace CQRSMicroservices.ServiceFabric.WebService
{

  public class ServiceFabricAggregateRootRepository : AggregateRootRepository
  {
    public override async Task ExecuteOn<T>(Guid aggregateId, Command command)
    {
      var actor = ActorProxy.Create<IAggregateRootActor>(new ActorId(aggregateId), "fabric:/CQRSMicroservices.ServiceFabric.Application");
      await actor.ExecuteOn(typeof(T).AssemblyQualifiedName, command.ToJson());
    }

    public override async Task ExecuteOnNew<T>(Guid aggregateId, Command command)
    {
      var actor = ActorProxy.Create<IAggregateRootActor>(new ActorId(aggregateId), "fabric:/CQRSMicroservices.ServiceFabric.Application");
      await actor.ExecuteOnNew(typeof(T).AssemblyQualifiedName, command.ToJson());
    }
  }
}
