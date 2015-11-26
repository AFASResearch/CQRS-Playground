using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservicesSample.Webserver.Middleware
{
  public class CommandMiddleware : OwinMiddleware
  {
    public CommandMiddleware(OwinMiddleware next, Action<Exception> exceptionHandler) : base(next)
    {
      ExceptionHandler = exceptionHandler;
    }

    private Action<Exception> ExceptionHandler { get; }

    public override async Task Invoke(IOwinContext context)
    {
      if(context.Request.Method.Equals("post", StringComparison.OrdinalIgnoreCase) && context.Request.Path.GetExtension() == "command")
      {
        try
        {
          // Deserialize the command and dispatch it
          var command = DeserializeFromBody(context.Request.Body);
          await CqrsApplication.GetService<CommandBus>().Dispatch(command);
          context.Response.StatusCode = 200;
          await context.Response.WriteAsync("{ \"commandAccepted\": true }");
        }
        catch(AggregateException aEx)
        {
          var ex = aEx.InnerExceptions.FirstOrDefault();
          ExceptionHandler(ex ?? aEx);
          context.Response.StatusCode = 500;
          await context.Response.WriteAsync($"{{ \"commandAccepted\": false, \"error\": \"{ex?.Message}\" }}");
        }
        catch(Exception ex)
        {
          ExceptionHandler(ex);
          context.Response.StatusCode = 500;
          await context.Response.WriteAsync($"{{ \"commandAccepted\": false, \"error\": \"{ex.Message}\" }}");
        }
      }
      else
      {
        await Next.Invoke(context);
      }
    }

    private Command DeserializeFromBody(Stream body)
    {
      using(var sr = new StreamReader(body))
      {
        using(JsonReader jsonReader = new JsonTextReader(sr))
        {
          return CqrsApplication.GetService<IDeserializer>().CreateCommand(JObject.Load(jsonReader));
        }
      }
    }
  }
}
