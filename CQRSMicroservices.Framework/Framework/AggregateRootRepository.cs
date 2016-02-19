using System;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public class AggregateRootRepository
  {
    
    public EventBus EventBus => CqrsApplication.GetService<EventBus>();
    public IEventStore EventStore => CqrsApplication.GetService<IEventStore>();

    public virtual async Task ExecuteOn<T>(Guid aggregateId, Command command) where T : AggregateRoot
    {
      T aggregateRoot = LoadAggregateRoot<T>(aggregateId);

      if(aggregateRoot.IsNew)
      {
        throw new Exception($"AggregateRoot with id {aggregateId} didn't exist.");
      }
      try
      {
        aggregateRoot.Handle(command);
      }
      catch
      {
        throw new Exception($"Problem with executing command on AR.");
      }
      await SaveAndDispatchEvents(aggregateId, aggregateRoot);
    }

    public virtual async Task ExecuteOnNew<T>(Guid aggregateId, Command command) where T : AggregateRoot
    {
      T aggregateRoot = LoadAggregateRoot<T>(aggregateId);

      if(!aggregateRoot.IsNew)
      {
        throw new Exception($"AggregateRoot with id {aggregateId} did exist.");
      }
      try
      {
        aggregateRoot.Handle(command);
      }
      catch
      {
        throw new Exception($"Problem with executing command on new AR.");
      }
      await SaveAndDispatchEvents(aggregateId, aggregateRoot);
    }

    /// <summary>
    /// Here we should rehydrate an AggregateRoot by applying the events from his stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    private T LoadAggregateRoot<T>(Guid id) where T : AggregateRoot
    {
      var aggregateRoot = (T)Activator.CreateInstance(typeof(T));

      var history = EventStore.GetEvents(id);
      aggregateRoot.LoadHistory(history);

      return aggregateRoot;
    }

    private async Task SaveAndDispatchEvents(Guid aggregateId, AggregateRoot root)
    {
      var commit = root.Commit();
      EventStore.AddEvents(aggregateId, commit.Events);

      await Task.WhenAll(commit.Events.Select(EventBus.Dispatch));
    }
  }
}
