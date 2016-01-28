using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.Framework
{
  public class EventReplayer
  {

    public async Task ReplayAllEvents(IEventStore eventStore)
    {
      Dictionary<Guid, List<Event>> eventstore = eventStore.GetAllEvents();
      EventBus eventBus = CqrsApplication.GetService<EventBus>();
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

    private static async Task ReplayEventList (IEnumerable<Event> events)
    {
      EventBus eventBus = CqrsApplication.GetService<EventBus>();
      foreach(var e in events)
      {
        await eventBus.Dispatch(e);
      }
    }

  }
}
