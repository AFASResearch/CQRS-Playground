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
  
    public virtual async Task ExecuteOn<T>(Guid aggregateId, Command command) where T: AggregateRoot
    {
      T aggregateRoot = LoadAggregateRoot<T>(aggregateId);
      aggregateRoot.Handle(command);
      await SaveAndDispatchEvents(aggregateRoot);
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
      await SaveAndDispatchEvents(aggregateRoot);
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
        return (T)root;
      }
      throw new KeyNotFoundException($"AggregateRoot with id {id} does not exist.");
    }

    private async Task SaveAndDispatchEvents(AggregateRoot root)
    {
      var commit = root.Commit();
      // We should make these persistent somewhere

      await Task.WhenAll(commit.Events.Select(EventBus.Dispatch));
    }
  }
}
