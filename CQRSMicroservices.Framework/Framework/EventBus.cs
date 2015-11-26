using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public class EventBus
  {
    protected readonly Dictionary<Type, List<QueryModelBuilder>> _handlers = new Dictionary<Type, List<QueryModelBuilder>>();

    public virtual async Task Dispatch(Event @event)
    {
      List<QueryModelBuilder> handlers;
      if(_handlers.TryGetValue(@event.GetType(), out handlers))
      {
        await Task.WhenAll(handlers.Select(b => b.Handle(@event)));
      }
      else
      {
        throw new NotImplementedException($"No handler for eventtype {@event.GetType().FullName}");
      }
    }

    public void RegisterBuilder(QueryModelBuilder queryModelBuilder)
    {
      foreach(var eventType in queryModelBuilder.GetHandledEevents())
      {
        if(!_handlers.ContainsKey(eventType))
        {
          _handlers.Add(eventType, new List<QueryModelBuilder>());
        }
        _handlers[eventType].Add(queryModelBuilder);
      }
    }
  }
}