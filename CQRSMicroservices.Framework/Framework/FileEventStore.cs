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
      _dir = baseDir;
      _deserializer = deserializer;
      if(!Directory.Exists(_dir))
      {
        Directory.CreateDirectory(_dir);
      }
    }

    public void AddEvents(Guid aggregateId, IEnumerable<Event> events)
    {
      string path = Path.Combine(_dir, $"{aggregateId}.txt");
      var stream = new FileStream(path, FileMode.Append, FileAccess.Write);
      using(var streamWriter = new StreamWriter(stream))
      {
        foreach(var e in events)
        {
          streamWriter.WriteLine(e.EventDate);
          streamWriter.WriteLine(e.ToJson());
          streamWriter.WriteLine("end event");
        }
      }
    }

    public IEnumerable<Event> GetEvents(Guid aggregateId)
    {
      var events = new List<Event>();
      string path = Path.Combine(_dir, $"{aggregateId}.txt");
      if(File.Exists(path))
      {
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

        var textAccumalated = String.Empty;
        var eventDate = DateTime.MinValue;
        using(var streamReader = new StreamReader(stream))
        {
          while(!streamReader.EndOfStream)
          {
            var text = streamReader.ReadLine();
            DateTime eventDate2;
            if(DateTime.TryParse(text, out eventDate2))
            {
              eventDate = eventDate2;
              textAccumalated = string.Empty;
            }
            else if(text != null && text.Equals("end event"))
            {
              var eventJson = JObject.Parse(textAccumalated);
              var @event = _deserializer.CreateEvent(eventJson);
              @event.EventDate = eventDate;
              events.Add(@event);
            }
            else
            {
              textAccumalated += text;
            }
          }
        }
      }
      return events;
    }

    public IEnumerable<KeyValuePair<Guid, IEnumerable<Event>>> GetAllEvents()
    {
      if(Directory.Exists(_dir))
      {
        var files = Directory.GetFiles(_dir, "*.txt").Select(Path.GetFileNameWithoutExtension);
        foreach(var fileName in files)
        {
          var g = Guid.Parse(fileName);
          yield return new KeyValuePair<Guid, IEnumerable<Event>>(g, GetEvents(g));
        }
      }
    }

    public IEnumerable<Guid> GetExistingArs()
    {
      if(Directory.Exists(_dir))
      {
        var files = Directory.GetFiles(_dir, "*.txt").Select(Path.GetFileNameWithoutExtension);
        foreach(var fileName in files)
        {
          yield return Guid.Parse(fileName);
        }
      }
    }

  }
}
