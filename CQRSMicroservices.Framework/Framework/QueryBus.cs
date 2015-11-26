using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public class QueryBus
  {
    private readonly Dictionary<Type, QueryHandler> _handlers = new Dictionary<Type, QueryHandler>();

    public async Task<object> Dispatch(Query query)
    {
      QueryHandler handler;
      if(_handlers.TryGetValue(query.GetType(), out handler))
      {
        return await handler.Handle(query);
      }
      else
      {
        throw new NotImplementedException($"No handler for querytype {query.GetType().FullName}");
      }
    }

    public void RegisterHandler(QueryHandler queryHandler)
    {
      foreach(var commandType in queryHandler.GetHandledQueries())
      {
        _handlers.Add(commandType, queryHandler);
      }
    }
  }
}