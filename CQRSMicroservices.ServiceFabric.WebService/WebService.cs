using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Fabric;

namespace CQRSMicroservices.ServiceFabric.WebService
{
  /// <summary>
  /// The FabricRuntime creates an instance of this class for each service type instance. 
  /// </summary>
  internal sealed class WebService : StatelessService
  {
    private readonly IOwinAppBuilder _appBuilder;
    private readonly string _appRoot;

    /// <summary>
    ///   Initializes a new instance of the <see cref="WebService" /> class.
    /// </summary>
    public WebService()
    {
      _appBuilder = new Startup();
      _appRoot = null;
    }

    /// <summary>
    ///   Creates the service instance listeners.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
    {
      return new[]
      {
        new ServiceInstanceListener(CreateCommunicationListener)
      };
    }

    /// <summary>
    ///   Creates the communication listener.
    /// </summary>
    private ICommunicationListener CreateCommunicationListener(ServiceInitializationParameters serviceInitializationParameters)
    {
      try
      {
        return new OwinCommunicationListener(serviceInitializationParameters, _appBuilder, appRoot: _appRoot);
      }
      catch(Exception ex)
      {
        ServiceEventSource.Current.ErrorMessage("OwinService - Failed to Create Communication listener", ex);
        throw;
      }
    }
  }
}
