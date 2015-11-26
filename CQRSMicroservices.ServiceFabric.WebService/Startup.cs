using System;
using System.Diagnostics;
using System.Fabric;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using CQRSMicroservices.Application;
using CQRSMicroservices.Framework;
using CQRSMicroservices.Web.Middleware;
using Owin;

namespace CQRSMicroservices.ServiceFabric.WebService
{
  /// <summary>
  ///   OWin startup class for the CQRS BackEnd webservice.
  /// </summary>
  public class Startup : IOwinAppBuilder
  {
    /// <summary>
    ///   The Configuration method for the Owin application
    /// </summary>
    /// <param name="appBuilder">The appBuilder.</param>
    public void Configuration(IAppBuilder appBuilder)
    {
      try
      {
        CqrsApplication.Bootstrap(
          new ServiceFabricAggregateRootRepository(), 
          new ServiceFabricQueryRepository(),
          Handlers.CommandHandlers, Handlers.QueryHandlers, new QueryModelBuilder[0]);

        CqrsApplication.SetService<IDeserializer>(new Deserializer());

        ServicePointManager.DefaultConnectionLimit = 1000;
        ThreadPool.SetMaxThreads(4096, 1000);

        Action<Exception> exceptionHandler = ex => ServiceEventSource.Current.ErrorMessage("WebService - Middleware failed", ex);
        appBuilder.Use<CommandMiddleware>(exceptionHandler);
        appBuilder.Use<QueryMiddleware>(exceptionHandler);
      }
      catch(Exception ex)
      {
        ServiceEventSource.Current.ErrorMessage("WebService - Startup.Configure failed", ex);
        throw;
      }
    }
  }
}