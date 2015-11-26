using System;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace CQRSMicroservices.ServiceFabric.WebService
{
  /// <summary>
  ///   An OWIN communication listener for Service Fabric stateless and stateful services.
  /// </summary>
  public class OwinCommunicationListener : ICommunicationListener
  {
    /// <summary>
    ///   OWIN server handle.
    /// </summary>
    private IDisposable _serverHandle;
    private readonly IOwinAppBuilder _startup;
    private readonly string _appRoot;
    private readonly string _endpointName;
    private string _listeningAddress;
    private readonly ServiceInitializationParameters _serviceInitializationParameters;

    /// <summary>
    ///   Initializes a new instance of the <see cref="OwinCommunicationListener" /> class.
    /// </summary>
    /// <param name="serviceInitializationParameters">The service initialization parameters.</param>
    /// <param name="startup">The startup.</param>
    /// <param name="appRoot">The application root.</param>
    /// <param name="endpointName">The name of the endpoint in the ServiceManifest.xml to use, defaults to "ServiceEndpoint"</param>
    public OwinCommunicationListener(ServiceInitializationParameters serviceInitializationParameters, IOwinAppBuilder startup, string appRoot = null,
      string endpointName = "ServiceEndpoint")
    {
      _serviceInitializationParameters = serviceInitializationParameters;
      _startup = startup;
      _appRoot = appRoot;
      _endpointName = endpointName;
    }

    /// <summary>
    ///   Aborts the communication listener.
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public void Abort()
    {
      StopWebServer();
      ServiceEventSource.Current.Message("Communication Listener aborted.");
    }

    /// <summary>
    ///   Closes the communication listener.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public Task CloseAsync(CancellationToken cancellationToken)
    {
      StopWebServer();
      ServiceEventSource.Current.Message("Communication Listener closed.");
      return Task.FromResult(1);
    }

    /// <summary>
    ///   Opens the communication listener
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public Task<string> OpenAsync(CancellationToken cancellationToken)
    {
      EndpointResourceDescription serviceEndpoint = _serviceInitializationParameters.CodePackageActivationContext.GetEndpoint(_endpointName);
      int port = serviceEndpoint.Port;
      string appRoot = string.IsNullOrWhiteSpace(_appRoot) ? string.Empty : _appRoot.TrimEnd('/') + '/';

      var statefulServiceInitializationParameters = _serviceInitializationParameters as StatefulServiceInitializationParameters;
      if(statefulServiceInitializationParameters != null)
      {
        _listeningAddress = string.Format(
          CultureInfo.InvariantCulture,
          "http://+:{0}/{1}{2}/{3}/{4}",
          port,
          appRoot,
          statefulServiceInitializationParameters.PartitionId,
          statefulServiceInitializationParameters.ReplicaId,
          Guid.NewGuid());
      }
      else if(_serviceInitializationParameters is StatelessServiceInitializationParameters)
      {
        _listeningAddress = string.Format(CultureInfo.InvariantCulture, "http://+:{0}/{1}", port, appRoot);
      }
      else
      {
        throw new InvalidOperationException();
      }

      _serverHandle = WebApp.Start(_listeningAddress, appBuilder => _startup.Configuration(appBuilder));

      string resultAddress = _listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

      ServiceEventSource.Current.Message("Listening on {0}", resultAddress);
      return Task.FromResult(resultAddress);
    }

    private void StopWebServer()
    {
      if(_serverHandle != null)
      {
        try
        {
          _serverHandle.Dispose();
        }
        catch(ObjectDisposedException)
        {
          // no-op
        }
      }
    }
  }
}