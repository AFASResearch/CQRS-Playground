using System;
using System.Collections.Generic;

namespace CQRSMicroservices.Framework
{
  public interface IEventStore
  {
    void AddEvents(Guid source, IEnumerable<Event> events);

    IEnumerable<Event> GetEvents(Guid aggregateId);

    IEnumerable<Event> GetEvents(Guid aggregateId, DateTime afterDateTime, DateTime beforeDateTime);

    Dictionary<Guid, List<Event>> GetAllEvents();

  }
}