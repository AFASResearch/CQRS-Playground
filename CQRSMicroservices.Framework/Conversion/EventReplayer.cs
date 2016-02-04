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

    public IEnumerable<KeyValuePair<Guid, Event>> SimpleReplayEventStore(IEventStore eventStore)
    {
      foreach(KeyValuePair<Guid, IEnumerable<Event>> key in eventStore.GetAllEvents())
      {
        foreach(var e in ReplayEventList(key.Value))
        {
          yield return new KeyValuePair<Guid, Event>(key.Key, e);
        }
      }
    }

    public IEnumerable<Event> ReplayAr(IEventStore eventStore, Guid aggregateId)
    {
      foreach(var e in ReplayEventList(eventStore.GetEvents(aggregateId)))
      {
        yield return e;
      }
    }

    public IEnumerable<KeyValuePair<Guid, Event>> ReplaySetOfArChronological(IEventStore eventStore, IEnumerable<Guid> setOfArs)
    {
      var iterators = new Dictionary<Guid, EventListIterator>();
      var priorityQueue = new SimplePriorityQueue<Guid>();

      Boolean finished = false;

      // Initiating for each AR with events an EventListIterator and add it to Iterators and insert it into the priorityQueue
      foreach(var id in setOfArs)
      {
        var i = new EventListIterator
        {
          EventTime = eventStore.GetEvents(id).First().EventDate,
          EventQueue = new Queue<Event>(eventStore.GetEvents(id))
        };
        iterators.Add(id, i);
        priorityQueue.Enqueue(id, DateTimeToUnixSeconds(i.EventTime));
      }


      // Take the first from priorityqueue, play this event and add it to the priorityqueue again with the datetime from next event from the AR 
      while(!finished)
      {
        if(iterators.Count == 1)
        {
          var guid = iterators.Keys.First();
          foreach(var e in ReplayEventList(iterators[guid].EventQueue))
          {
            yield return new KeyValuePair<Guid, Event>(guid, e);
          }
          finished = true;
        }
        else
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
            priorityQueue.Enqueue(optimumGuid, DateTimeToUnixSeconds(iterators[optimumGuid].EventTime));
          }

        }
      }
    }

    public IEnumerable<KeyValuePair<Guid, Event>> ReplayAllEventsChronological(IEventStore eventStore)
    {
      foreach(var p in ReplaySetOfArChronological(eventStore, eventStore.GetExistingArs()))
      {
        yield return p;
      }
    }

    private static double DateTimeToUnixSeconds(DateTime a)
    {
      return a.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }


    private static IEnumerable<Event> ReplayEventList(IEnumerable<Event> events)
    {
      foreach(var e in events)
      {
        yield return e;
      }
    }

  }
}
