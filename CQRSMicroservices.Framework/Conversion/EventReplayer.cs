using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using Priority_Queue;

namespace CQRSMicroservices.Conversion
{
  public class EventReplayer
  {

    public IEnumerable<KeyValuePair<Guid, Event>> ReplaySetOfArChronological(IEventStore eventStore, IEnumerable<Guid> setOfArs)
    {
      var iterators = new Dictionary<Guid, EventListIterator>();
      var priorityQueue = new SimplePriorityQueue<Guid>();
      
      foreach(var id in setOfArs)
      {
        var events = eventStore.GetEvents(id);
        var i = new EventListIterator(new Queue<Event>(events));

        iterators.Add(id, i);
        priorityQueue.Enqueue(id, DateTimeToUnixSeconds(i.CurrentStreamPosition));
      }

      while(priorityQueue.Any())
      {
          var optimumGuid = priorityQueue.Dequeue();

          //Can be optimized with using the first 2 AR's, so we don't have to put the AR back everytime, but only when it passes the 2nd
          var e = iterators[optimumGuid].GetEvent();

          yield return new KeyValuePair<Guid, Event>(optimumGuid, e);

          if(iterators[optimumGuid].Finished)
          {
            iterators.Remove(optimumGuid);
          }
          else
          {
            priorityQueue.Enqueue(optimumGuid, DateTimeToUnixSeconds(iterators[optimumGuid].CurrentStreamPosition));
          }
      }
    }

    public IEnumerable<KeyValuePair<Guid, Event>> ReplayAllEventsChronological(IEventStore eventStore)
    {
      return ReplaySetOfArChronological(eventStore, eventStore.GetExistingStreamIds());
    }

    // Double representation from DateTime, maybe can be simplified
    private static double DateTimeToUnixSeconds(DateTime a)
    {
      return a.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }

  }
}
