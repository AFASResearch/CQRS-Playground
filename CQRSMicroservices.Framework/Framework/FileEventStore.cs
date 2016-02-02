using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CQRSMicroservices.Framework
{
  public class FileEventStore : IEventStore
  {
    private readonly string _dir;
    private readonly IDeserializer _deserializer;

    public FileEventStore(string baseDir, IDeserializer deserializer)
    {
      this._dir = baseDir;
      this._deserializer = deserializer;
      if(!Directory.Exists(this._dir))
      {
        Directory.CreateDirectory(this._dir);
      }
    }

    public void AddEvents(Guid aggregateId, IEnumerable<Event> events)
    {
      string path = Path.Combine(this._dir, aggregateId.ToString() + ".txt");
      FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write);
      using(StreamWriter streamWriter = new StreamWriter(stream))
      {
        foreach(Event e in events)
        {
          streamWriter.WriteLine(e.EventDate + "  " + e.ToJson().Replace("\n", string.Empty).Replace("\r", string.Empty));
        }
      }
    }

    public IEnumerable<Event> GetEvents(Guid aggregateId)
    {
      List<Event> events = new List<Event>();
      try
      {
        string path = Path.Combine(this._dir, aggregateId.ToString() + ".txt");
        if (File.Exists(path))
        { 
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
          using(StreamReader streamReader = new StreamReader(stream))
          {
            while(!streamReader.EndOfStream)
            {
              string text = streamReader.ReadLine();
              try
              {
                if(text != null)
                {
                  DateTime eventDate = DateTime.Parse(text.Substring(0, 19));
                  JObject eventJson = JObject.Parse(text.Substring(19));
                  Event @event = this._deserializer.CreateEvent(eventJson);
                  @event.EventDate = eventDate;
                  events.Add(@event);
                }
              }
              catch(Exception value)
              {
                Console.WriteLine(value);
                Console.WriteLine(text.Substring(0, 19) + "AND" + text.Substring(19));
              }
            }
          }
        }
      }
      catch(Exception e)
      {
        Console.WriteLine("FileEventStore crashing: " + e);
      }
      return events;
    }

    public IEnumerable<Event> GetEvents(Guid aggregateId, DateTime afterDateTime, DateTime beforeDateTime)
    {
      List<Event> events = new List<Event>();
      try
      {
        string path = Path.Combine(this._dir, aggregateId.ToString() + ".txt");
        if(File.Exists(path))
        {
          FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
          using(StreamReader streamReader = new StreamReader(stream))
          {
            while(!streamReader.EndOfStream)
            {
              string text = streamReader.ReadLine();
              try
              {
                if(text != null)
                {
                  DateTime dateTime = DateTime.Parse(text.Substring(0, 19));
                  if(dateTime > beforeDateTime)
                  {
                    return events;
                  }
                  else if(dateTime < afterDateTime)
                  {
                    continue;
                  }
                  else
                  {
                    JObject eventJson = JObject.Parse(text.Substring(19));
                    Event @event = this._deserializer.CreateEvent(eventJson);
                    @event.EventDate = dateTime;
                    events.Add(@event);
                  }
                }
              }
              catch(Exception value)
              {
                Console.WriteLine(value);
                Console.WriteLine(text.Substring(0, 19) + "AND" + text.Substring(19));
              }
            }
          }
        }
      }
      catch(Exception arg)
      {
        Console.WriteLine("FileEventStore crashing: " + arg);
      }
      return events;
    }

    public Dictionary<Guid, List<Event>> GetAllEvents()
    {
      Dictionary<Guid, List<Event>> eventstore = new Dictionary<Guid, List<Event>>();
      if(Directory.Exists(_dir))
      {
        var files = Directory.GetFiles(_dir, "*.txt").Select(Path.GetFileNameWithoutExtension);
        foreach(var fileName in files)
        {
          Guid g = Guid.Parse(fileName);
          eventstore.Add(g, GetEvents(g).ToList());
        }
      }

      return eventstore;
    }
  }
}
