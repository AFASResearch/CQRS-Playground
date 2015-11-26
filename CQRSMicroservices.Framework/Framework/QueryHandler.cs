using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public class QueryHandler
  {
    private readonly Dictionary<Type, Func<Query, Task<object>>> _handlers = new Dictionary<Type, Func<Query, Task<object>>>();

    public QueryRepository Repository => CqrsApplication.GetService<QueryRepository>();

    public IEnumerable<Type> GetHandledQueries()
    {
      return _handlers.Keys;
    }

    public void RegisterHandler<T, TOut>(Func<T, Task<TOut>> handler) where T : Query
    {
      _handlers.Add(typeof(T), async c => await handler((T)c));
    }

    public Task<object> Handle(Query query)
    {
      Func<Query, Task<object>> handler;
      if(_handlers.TryGetValue(query.GetType(), out handler))
      {
        return handler(query);
      }
      else
      {
        throw new NotImplementedException($"No handler for querytype {query.GetType().FullName}");
      }
    }
  }
}