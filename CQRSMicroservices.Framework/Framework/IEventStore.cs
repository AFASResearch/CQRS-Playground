using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public interface IEventStore
  {
    void AddEvents(Guid source, IEnumerable<Event> events);

    IEnumerable<Event> GetEvents(Guid aggregateId);

  }
}