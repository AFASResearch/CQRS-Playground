using Owin;

namespace CQRSMicroservices.ServiceFabric.WebService
{
  /// <summary>
  ///   Interface that describes an Owin entry point.
  /// </summary>
  public interface IOwinAppBuilder
  {
    /// <summary>
    ///   The Configuration method for the Owin application
    /// </summary>
    /// <param name="appBuilder">The appBuilder.</param>
    void Configuration(IAppBuilder appBuilder);
  }
}