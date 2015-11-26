using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using CQRSMicroservices.ServiceFabric.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CQRSMicroservices.ServiceFabric.EventBusService
{
  public class ServiceFabricEventBus : EventBus
  {
    public override async Task Dispatch(Event @event)
    {
      List<QueryModelBuilder> handlers;
      if(_handlers.TryGetValue(@event.GetType(), out handlers))
      {
        await Task.WhenAll(
          handlers.Select(b =>
          {
            var builderType = b.GetType();
            var queryModelBuilderService = ServiceProxy.Create<IQueryModelBuilderService>(
              $"{builderType.FullName}, {builderType.Assembly.GetName().Name}",
              new Uri("fabric:/CQRSMicroservices.ServiceFabric.Application/QueryModelBuilderService"
            ));
            return queryModelBuilderService.Handle(@event.ToJson());
          })
        );
      }
      else
      {
        throw new NotImplementedException($"No handler for eventtype {@event.GetType().FullName}");
      }
    }
  }
}
