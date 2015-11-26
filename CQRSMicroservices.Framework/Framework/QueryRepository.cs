using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.Framework
{
  public class QueryRepository
  {
    private readonly Dictionary<Guid, JObject> _documents = new Dictionary<Guid, JObject>(); 

    public virtual Task Add(Guid id, JObject jObject)
    {
      _documents.Add(id, jObject);
      return Task.FromResult(1);
    }
    public virtual Task Update(Guid id, JObject jObject)
    {
      _documents[id] = jObject;
      return Task.FromResult(1);
    }

    public virtual Task<JObject> Get(Guid id)
    {
      if(_documents.ContainsKey(id))
      {
        return Task.FromResult(_documents[id]);
      }

      return Task.FromResult((JObject)null);
    }
  }
}