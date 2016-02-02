using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using Priority_Queue;

namespace CQRSMicroservices.Conversion
{
  public class EventReplayer
  {
    public EventReplayer(EventBus targetBus)
    {
      _targetBus = targetBus;
    }

    private static EventBus _targetBus;

    public async Task ReplayAllEvents(IEventStore eventStore)
    {
      Dictionary<Guid, List<Event>> eventstore = eventStore.GetAllEvents();
      foreach(var key in eventstore.Keys)
      {
        await ReplayEventList(eventstore[key]);
      }
    }

    public async Task ReplaySpecificArEvents(IEventStore eventStore, Guid aggregateId)
    {
      var eventstore = eventStore.GetEvents(aggregateId);
      await ReplayEventList(eventstore);
    }

    public async Task ReplaySetOfArEvents(IEventStore eventStore, List<Guid> aggregateId)
    {
      foreach(var a in aggregateId)
      {
        var events = eventStore.GetEvents(a);
        await ReplayEventList(events);
      }
    }
  
    public async Task ReplayAllEventsChronological(IEventStore eventStore)
    {
      Dictionary<Guid, List<Event>> eventstore = eventStore.GetAllEvents();
      Dictionary <Guid, EventListIterator> iterators = new Dictionary<Guid, EventListIterator>();
      SimplePriorityQueue<Guid> priorityQueue = new SimplePriorityQueue<Guid>();

      Boolean finished = false;

      // Initiating for each AR with events an EventListIterator and add it to Iterators and insert it into the priorityQueue
      foreach(var key in eventstore.Keys)
      {
        EventListIterator i = new EventListIterator
        {
          EventTime = eventstore[key][0].EventDate,
          EventQueue = new Queue<Event>(eventstore[key])
        };
        iterators.Add(key,i);
        priorityQueue.Enqueue(key, DateTimeToUnixSeconds(eventstore[key][0].EventDate));
      }

    // Take the first from priorityqueue, play this event and add it to the priorityqueue again with the datetime from next event from the AR 
    while(!finished)
      {
        if(iterators.Count == 1)
        {
          await ReplayEventList(iterators[iterators.Keys.First()].EventQueue);
          finished = true;
        }
        else
        {
          Guid optimumGuid = priorityQueue.Dequeue();
          Event e = iterators[optimumGuid].GetEvent();

          await _targetBus.Dispatch(e);
          //System.Console.WriteLine(e.ToJson() + " " + e.EventDate);

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

    private static double DateTimeToUnixSeconds(DateTime a)
    {
      return a.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }


    private static async Task ReplayEventList (IEnumerable<Event> events)
    {
      EventBus eventBus = CqrsApplication.GetService<EventBus>();
      foreach(var e in events)
      {
        await _targetBus.Dispatch(e);
        //System.Console.WriteLine(e.ToJson()+" "+e.EventDate);
      }
    }

  }
}
