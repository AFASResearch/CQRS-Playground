using System.Collections.Generic;

namespace CQRSMicroservices.Framework
{
  public class Commit
  {
    public Commit(IEnumerable<Event> events)
    {
      Events = events;
    }
    public IEnumerable<Event> Events { get; } 
  }
}
