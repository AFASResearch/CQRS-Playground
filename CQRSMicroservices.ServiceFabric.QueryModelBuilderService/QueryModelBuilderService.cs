using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSMicroservices.Application;
using CQRSMicroservices.Framework;
using CQRSMicroservices.ServiceFabric.Interfaces;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Newtonsoft.Json.Linq;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CQRSMicroservices.ServiceFabric.QueryModelBuilderService
{
  /// <summary>
  /// The FabricRuntime creates an instance of this class for each service type instance.
  /// </summary>
  internal sealed class QueryModelBuilderService : StatefulService, IQueryModelBuilderService
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
      return new ServiceRemotingListener<QueryModelBuilderService>(serviceInitializationParameters, this);
    }

    /// <summary>
    /// This is the main entry point for your service's partition replica. 
    /// RunAsync executes when the primary replica for this partition has write status.
    /// </summary>
    /// <param name="cancellationToken">Canceled when Service Fabric terminates this partition's replica.</param>
    protected override async Task RunAsync(CancellationToken cancellationToken)
    {
      var queryModelBuilder = GetQueryModelBuilder();
      var queue = await StateManager.GetOrAddAsync<IReliableQueue<string>>("queryModelBuilderQueue");

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
            await queryModelBuilder.Handle(CqrsApplication.GetService<IDeserializer>().CreateEvent(JObject.Parse(message)));

            await tx.CommitAsync();
          }
        }

        await _semaphore.WaitAsync(cancellationToken);
      }
    }

    private QueryModelBuilder GetQueryModelBuilder()
    {
      var namedPartition = (NamedPartitionInformation)ServicePartition.PartitionInfo;
      var queryModelBuilder = (QueryModelBuilder)Activator.CreateInstance(Type.GetType(namedPartition.Name, true));
      return queryModelBuilder;
    }

    public async Task Handle(string eventJson)
    {
      var queue = await StateManager.GetOrAddAsync<IReliableQueue<string>>("queryModelBuilderQueue");
      using(ITransaction tx = StateManager.CreateTransaction())
      {
        await queue.EnqueueAsync(tx, eventJson);
        await tx.CommitAsync();
      }

      _semaphore.Release();
    }

    public async Task<string> Get(Guid id)
    {
      var result = await CqrsApplication.GetService<QueryRepository>().Get(id);
      return result?.ToString();
    }
  }
}
