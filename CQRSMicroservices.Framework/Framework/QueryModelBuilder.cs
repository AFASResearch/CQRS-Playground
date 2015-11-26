using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public class QueryModelBuilder
  {
    private readonly Dictionary<Type, Func<Event, Task>> _handlers = new Dictionary<Type, Func<Event, Task>>();

    public QueryRepository Repository => CqrsApplication.GetService<QueryRepository>();

    public IEnumerable<Type> GetHandledEevents()
    {
      return _handlers.Keys;
    }

    public void RegisterHandler<T>(Func<T, Task> handler) where T : Event
    {
      _handlers.Add(typeof(T), c => handler((T)c));
    }

    public IEnumerable<Type> GetHandledCommands()
    {
      return _handlers.Keys;
    }

    public async Task Handle(Event @event)
    {
      Func<Event, Task> handler;
      if(_handlers.TryGetValue(@event.GetType(), out handler))
      {
        await handler(@event);
      }
    }
  }
}