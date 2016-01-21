using System;
using System.Collections.Generic;

namespace CQRSMicroservices.Framework
{
  public class FileEventStore : IEventStore
  {
    public void AddEvents(Guid source, IEnumerable<Event> events)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Event> GetEvents(Guid aggregateId)
    {
      throw new NotImplementedException();
    }
  }
}