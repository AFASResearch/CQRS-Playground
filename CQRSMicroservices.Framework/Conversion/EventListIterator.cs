using System;
using System.Collections.Generic;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Conversion
{
  class EventListIterator
  {
    public DateTime EventTime;
    public Queue<Event> EventQueue;
    public bool Finished;

    public EventListIterator()
    {
      Finished = false;
      EventTime = DateTime.MaxValue;
      EventQueue = new Queue<Event>();
    }

    public Event GetEvent()
    {
      Event e= EventQueue.Peek();
      if(EventQueue.Count == 1)
      {
        Finished = true;
      }
      else
      {
        EventQueue.Dequeue();
        EventTime = EventQueue.Peek().EventDate;
      }
      return e;
    }



  }
}
