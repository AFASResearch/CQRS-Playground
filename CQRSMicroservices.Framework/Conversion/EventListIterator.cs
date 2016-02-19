using System;
using System.Collections.Generic;
using System.Linq;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Conversion
{
  class EventListIterator
  {
    private DateTime _currentStreamPosition;
    private readonly Queue<Event> _events;
    private bool _finished;

    public EventListIterator(Queue<Event> events)
    {
      _currentStreamPosition = events.Peek().CommitTimestamp;
      _events = events;
    }

    public Event GetEvent()
    {
      Event e = _events.Dequeue();
      if(!_events.Any())
      {
        _finished = true;
      }
      else
      {
        _currentStreamPosition = _events.Peek().CommitTimestamp;
      }
      return e;
    }

    public bool Finished => _finished;

    public DateTime CurrentStreamPosition => _currentStreamPosition;
  }
}
