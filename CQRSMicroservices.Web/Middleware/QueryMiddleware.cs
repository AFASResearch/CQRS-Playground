using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using Microsoft.Owin;

namespace CQRSMicroservices.Web.Middleware
{
  public class QueryMiddleware : OwinMiddleware
  {
    public QueryMiddleware(OwinMiddleware next, Action<Exception> exceptionHandler) : base(next)
    {
      ExceptionHandler = exceptionHandler;
    }

    private Action<Exception> ExceptionHandler { get; }

    public override async Task Invoke(IOwinContext context)
    {
      if(context.Request.Method.Equals("get", StringComparison.OrdinalIgnoreCase) && 
         context.Request.Path.GetExtension().Equals("query", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          // Deserialize the querystring and dispatch it
          var query = DeserializeFromQueryString(context.Request.Path.ToString(), context.Request.Query);
          var result = await CqrsApplication.GetService<QueryBus>().Dispatch(query);
          context.Response.StatusCode = 200;
          await context.Response.WriteAsync(result?.ToString() ?? "null");
        }
        catch(AggregateException aEx)
        {
          var ex = aEx.InnerExceptions.FirstOrDefault();
          ExceptionHandler(ex ?? aEx);
          context.Response.StatusCode = 500;
          await context.Response.WriteAsync($"{{ \"queryAccepted\": false, \"error\": \"{ex?.Message}\" }}");
        }
        catch(Exception ex)
        {
          ExceptionHandler(ex);
          context.Response.StatusCode = 500;
          await context.Response.WriteAsync($"{{ \"queryAccepted\": false, \"error\": \"{ex.Message}\" }}");
        }
      }
      else
      {
        await Next.Invoke(context);
      }
    }

    private Query DeserializeFromQueryString(string name, IReadableStringCollection query)
    {
      return CqrsApplication.GetService<IDeserializer>().CreateQuery(name, query.Select(kv => new KeyValuePair<string, IEnumerable<string>>(kv.Key, kv.Value)));
    }
  }
}
