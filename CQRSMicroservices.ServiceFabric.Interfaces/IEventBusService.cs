using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CQRSMicroservices.ServiceFabric.Interfaces
{
  public interface IEventBusService : IService
  {
    Task Dispatch(string eventJson);
  }
}
