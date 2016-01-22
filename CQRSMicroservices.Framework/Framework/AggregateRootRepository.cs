using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public class AggregateRootRepository
  {
    private readonly Dictionary<Guid, AggregateRoot> _aggregateRoots = new Dictionary<Guid, AggregateRoot>();

    public EventBus EventBus => CqrsApplication.GetService<EventBus>();
    public IEventStore EventStore => CqrsApplication.GetEventStore();
  
    public virtual async Task ExecuteOn<T>(Guid aggregateId, Command command) where T: AggregateRoot
    {
      T aggregateRoot = LoadAggregateRoot<T>(aggregateId);
      aggregateRoot.Handle(command);
      await SaveAndDispatchEvents(aggregateId, aggregateRoot);
    }

    public virtual async Task ExecuteOnNew<T>(Guid aggregateId, Command command) where T : AggregateRoot
    {
      if(_aggregateRoots.ContainsKey(aggregateId))
      {
        throw new Exception($"AggregateRoot with id {aggregateId} already exists.");
      }

      var aggregateRoot = (T)Activator.CreateInstance(typeof(T));
      _aggregateRoots.Add(aggregateId, aggregateRoot);

      aggregateRoot.Handle(command);
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
      AggregateRoot root;
      if(_aggregateRoots.TryGetValue(id, out root))
      {
        root.LoadHistory(EventStore.GetEvents(id));
        return (T)root;
      }
      throw new KeyNotFoundException($"AggregateRoot with id {id} does not exist.");
    }

    private async Task SaveAndDispatchEvents(Guid aggregateId, AggregateRoot root)
    {
      var commit = root.Commit();

      EventStore.AddEvents(aggregateId, commit.Events);

      await Task.WhenAll(commit.Events.Select(EventBus.Dispatch));
    }
  }
}
