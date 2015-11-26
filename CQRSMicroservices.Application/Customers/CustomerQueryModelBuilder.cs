using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.Customers
{
  public class CustomerQueryModelBuilder : QueryModelBuilder
  {
    public CustomerQueryModelBuilder()
    {
      RegisterHandler<CustomerCreatedEvent>(Handle);
    }

    private async Task Handle(CustomerCreatedEvent @event)
    {
      await Repository.Add(@event.CustomerId, new JObject(
        new JProperty("CustomerId", @event.CustomerId),
        new JProperty("Name", @event.Name)));
    }
  }
}
