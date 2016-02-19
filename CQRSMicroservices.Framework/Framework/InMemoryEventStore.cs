using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CQRSMicroservices.Framework
{
  public class InMemoryEventStore : IEventStore
  {
    // Lookout this is leaking object references, which for this prototype is no problem

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

    IEnumerable<KeyValuePair<Guid, IEnumerable<Event>>> IEventStore.GetStreams()
    {
      foreach(KeyValuePair<Guid, List<Event>> pair in _eventstore)
      {
        yield return new KeyValuePair<Guid, IEnumerable<Event>>(pair.Key, pair.Value);
      }
    }

    public IEnumerable<Guid> GetExistingStreamIds()
    {
      return _eventstore.Keys;
    }
  }
}
