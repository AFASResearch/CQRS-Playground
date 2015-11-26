using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace CQRSMicroservices.ServiceFabric.Interfaces
{
  public interface IAggregateRootActor : IActor
  {
    Task ExecuteOn(string aggregateRootType, string command);

    Task ExecuteOnNew(string aggregateRootType, string command);
  }
}
