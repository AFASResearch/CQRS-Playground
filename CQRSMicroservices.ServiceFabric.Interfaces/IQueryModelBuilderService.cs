using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.ServiceFabric.Interfaces
{
  /// <summary>
  /// To not complicate this example any further, our QueryModelBuilder service also as a Get method to allow the reading on the in-memory repository.
  /// Normally you would let the QueryModelBuilder write to a database that is reachable for the QueryHandlers.
  /// </summary>
  /// <seealso cref="Microsoft.ServiceFabric.Services.Remoting.IService" />
  public interface IQueryModelBuilderService : IService
  {
    Task Handle(string eventJson);

    Task<string> Get(Guid id);
  }
}
