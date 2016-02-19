using System;
using System.Collections;
using System.Collections.Generic;

namespace CQRSMicroservices.Framework
{
  public interface IEventStore
  {
    void AddEvents(Guid source, IEnumerable<Event> events);

    IEnumerable<Event> GetEvents(Guid aggregateId);

    IEnumerable<KeyValuePair<Guid, IEnumerable<Event>>> GetStreams();

    IEnumerable<Guid> GetExistingStreamIds();

  }
}