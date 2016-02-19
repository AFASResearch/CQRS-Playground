using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CQRSMicroservices.Framework
{
  public class FileEventStore : IEventStore
  {
    private const string _eventDelimiter = "end event";
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
          streamWriter.WriteLine(e.CommitTimestamp);
          streamWriter.WriteLine(e.ToJson());
          streamWriter.WriteLine(_eventDelimiter);
        }
      }
    }

    public IEnumerable<Event> GetEvents(Guid aggregateId)
    {
      var events = new List<Event>();
      var path = Path.Combine(_dir, $"{aggregateId}.txt");
      if(File.Exists(path))
      {

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var textAccumalated = new StringBuilder();
        var currentEventDate = DateTime.MinValue;

        using(var streamReader = new StreamReader(stream))
        {
          while(!streamReader.EndOfStream)
          {
            var text = streamReader.ReadLine();
            DateTime parsedEventDate;

            if(DateTime.TryParse(text, out parsedEventDate))
            {
              currentEventDate = parsedEventDate;
              textAccumalated.Clear();
            }
            else if(text != null && text.Equals(_eventDelimiter))
            {
              var eventJson = JObject.Parse(textAccumalated.ToString());
              var @event = _deserializer.CreateEvent(eventJson);
              @event.CommitTimestamp = currentEventDate;
              events.Add(@event);
            }
            else
            {
              textAccumalated.Append(text);
            }
          }
        }
      }
      return events;
    }

    public IEnumerable<KeyValuePair<Guid, IEnumerable<Event>>> GetStreams()
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

    public IEnumerable<Guid> GetExistingStreamIds()
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
