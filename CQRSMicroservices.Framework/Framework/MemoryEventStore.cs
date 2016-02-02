using System;
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

    public IEnumerable<Event> GetEvents(Guid aggregateId, DateTime afterDateTime, DateTime beforeDateTime)
    {
      List<Event>events = new List<Event>();

      if(_eventstore.ContainsKey(aggregateId))
      {
        List<Event> eventsAr = _eventstore[aggregateId];
        foreach(Event e in eventsAr)
        {
          if(e.EventDate > beforeDateTime)
          {
            return events;
          }
          else if(e.EventDate < afterDateTime)
          {
            continue;
          }
          else
          {
            events.Add(e);
          }
        }
      }
      return events;
    }

    public Dictionary<Guid, List<Event>> GetAllEvents()
    {
      return _eventstore;
    }
  }
}
