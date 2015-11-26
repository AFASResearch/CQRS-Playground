using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSMicroservices.Application;
using CQRSMicroservices.ServiceFabric.Interfaces;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.ServiceFabric.EventBusService
{
  /// <summary>
  /// The FabricRuntime creates an instance of this class for each service type instance.
  /// </summary>
  internal sealed class EventBusService : StatefulService, IEventBusService
  {
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

    /// <summary>
    /// Optional override to create listeners (like tcp, http) for this service replica.
    /// </summary>
    /// <returns>The collection of listeners.</returns>
    protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
    {
      return new[]
      {
        new ServiceReplicaListener(CreateCommunicationListener)
      };
    }

    /// <summary>
    ///   Creates the communication listener.
    /// </summary>
    /// <returns></returns>
    private ICommunicationListener CreateCommunicationListener(ServiceInitializationParameters serviceInitializationParameters)
    {
      return new ServiceRemotingListener<EventBusService>(serviceInitializationParameters, this);
    }

    /// <summary>
    /// This is the main entry point for your service's partition replica. 
    /// RunAsync executes when the primary replica for this partition has write status.
    /// </summary>
    /// <param name="cancellationToken">Canceled when Service Fabric terminates this partition's replica.</param>
    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
      var queue = await StateManager.GetOrAddAsync<IReliableQueue<string>>("eventBusQueue");

      // We use the ServiceFabricEventBus as our way of dispatching in this service.
      // The class has all the registered builders and can locate them through the ServiceClient.

      var eventBus = new ServiceFabricEventBus();
      Handlers.QueryModelBuilders.ToList().ForEach(eventBus.RegisterBuilder);
      var deserializer = new Deserializer();

      var count = (int)await queue.GetCountAsync();
      if(count > 0)
      {
        _semaphore.Release(count);
      }

      while(true)
      {
        cancellationToken.ThrowIfCancellationRequested();

        using(ITransaction tx = StateManager.CreateTransaction())
        {
          ConditionalResult<string> dequeueReply = await queue.TryDequeueAsync(tx);
          if(dequeueReply.HasValue)
          {
            string message = dequeueReply.Value;
            await eventBus.Dispatch(deserializer.CreateEvent(JObject.Parse(message)));
            await tx.CommitAsync();
          }
        }

        await _semaphore.WaitAsync(cancellationToken);
      }
    }

    public async Task Dispatch(string eventJson)
    {
      var queue = await StateManager.GetOrAddAsync<IReliableQueue<string>>("eventBusQueue");
      using(ITransaction tx = StateManager.CreateTransaction())
      {
        await queue.EnqueueAsync(tx, eventJson);
        await tx.CommitAsync();
      }

      _semaphore.Release();
    }
  }
}
