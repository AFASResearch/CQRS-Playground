using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.Framework
{
  public interface IDeserializer
  {
    Command CreateCommand(JObject commandJson);
    Event CreateEvent(JObject eventJson);
    Query CreateQuery(string name, IEnumerable<KeyValuePair<string, IEnumerable<string>>> query);
  }
}
