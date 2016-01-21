using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  class MemoryEventStore : IEventStore
  {

    private readonly Dictionary<Guid, List<Event>> _eventstore = new Dictionary<Guid, List<Event>>();

    public void AddEvents(Guid aggregateGuid, IEnumerable<Event> events)
    {
      List<Event> existing;
      if(!_eventstore.TryGetValue(aggregateGuid, out existing))
      {
        existing = new List<Event>();
        _eventstore[aggregateGuid] = existing;
      }
      existing.AddRange(events);
    }

    public IEnumerable<Event> GetEvents(Guid aggregateGuid)
    {
      if(_eventstore.ContainsKey(aggregateGuid))
      {
        return _eventstore[aggregateGuid];
      }
      return Enumerable.Empty<Event>();
    }
  }
}
