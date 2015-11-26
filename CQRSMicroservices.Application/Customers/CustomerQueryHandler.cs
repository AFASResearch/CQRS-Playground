using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.Customers
{
  public class CustomerQueryHandler : QueryHandler
  {
    public CustomerQueryHandler()
    {
      RegisterHandler<GetCustomerQuery, JObject>(Handle);
    }

    private async Task<JObject> Handle(GetCustomerQuery query)
    {
      return await Repository.Get(query.CustomerId);
    }
  }
}
