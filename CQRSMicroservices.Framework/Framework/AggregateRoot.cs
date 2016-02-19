using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRSMicroservices.Framework
{
  public class AggregateRoot
  {
    private readonly Dictionary<Type, Action<Event>> _appliers = new Dictionary<Type, Action<Event>>();
    private readonly Dictionary<Type, Action<Command>> _handlers = new Dictionary<Type, Action<Command>>();

    private readonly List<Event> _uncommittedEvents = new List<Event>();
    public DateTime LastEventDateTime = DateTime.MinValue;

    public bool IsNew => LastEventDateTime.Equals(DateTime.MinValue);

    protected void RaiseEvent(Event @event)
    {
      @event.CommitTimestamp = DateTime.Now;
      LastEventDateTime = @event.CommitTimestamp;
      ApplyOnThis(@event);
      _uncommittedEvents.Add(@event);
    }

    protected void RegisterApply<T>(Action<T> apply) where T : Event
    {
      _appliers.Add(typeof(T), e => apply((T)e));
    }

    protected void RegisterHandler<T>(Action<T> handler) where T : Command
    {
      _handlers.Add(typeof(T), c => handler((T)c));
    }

    /// <summary>
    /// If there is an apply method on this instance for the given type of Event, we will apply the method directly.
    /// </summary>
    /// <param name="event"></param>
    private void ApplyOnThis(Event @event)
    {
      Action<Event> apply;
      if(_appliers.TryGetValue(@event.GetType(), out apply))
      {
        apply(@event);
      }
    }

    public void Handle(Command command)
    {
      Action<Command> handler;
      if(_handlers.TryGetValue(command.GetType(), out handler))
      {
        handler(command);
      }
      else
      {
        throw new NotImplementedException($"No handler for commandtype {command.GetType().FullName}");
      }
    }

    public void LoadHistory(IEnumerable<Event> historyEvents)
    {
      if(historyEvents.Any())
      {
        foreach(Event e in historyEvents)
        {
          LastEventDateTime = e.CommitTimestamp;
          ApplyOnThis(e);
        }
      }

    }

    /// <summary>
    /// Bundle all events and clear the list.
    /// </summary>
    public Commit Commit()
    {
      var events = _uncommittedEvents.ToArray();
      _uncommittedEvents.Clear();
      return new Commit(events);
    }
  }
}