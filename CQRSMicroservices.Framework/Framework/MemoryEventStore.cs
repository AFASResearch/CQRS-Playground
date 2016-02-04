using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CQRSMicroservices.Framework
{
  public class MemoryEventStore : IEventStore
  {

    private readonly Dictionary<Guid, List<Event>> _eventstore = new Dictionary<Guid, List<Event>>();

    public void AddEvents(Guid aggregateId, IEnumerable<Event> events)
    {
      List<Event> existing;
      if(!_eventstore.TryGetValue(aggregateId, out existing))
      {
        existing = new List<Event>();
        _eventstore[aggregateId] = existing;
      }
      existing.AddRange(events);
    }

    public IEnumerable<Event> GetEvents(Guid aggregateId)
    {
      if(_eventstore.ContainsKey(aggregateId))
      {
        return _eventstore[aggregateId];
      }
      return Enumerable.Empty<Event>();
    }

    IEnumerable<KeyValuePair<Guid, IEnumerable<Event>>> IEventStore.GetAllEvents()
    {
      foreach(KeyValuePair<Guid, List<Event>> pair in _eventstore)
      {
        yield return new KeyValuePair<Guid, IEnumerable<Event>>(pair.Key, pair.Value);
      }
    }

    public IEnumerable<Guid> GetExistingArs()
    {
      foreach(var e in _eventstore.Keys)
      {
        yield return e;
      }
    }

  }
}
