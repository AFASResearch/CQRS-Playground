using System.Threading.Tasks;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Customers
{
  public class CustomerCommandHandler : CommandHandler
  {
    public CustomerCommandHandler()
    {
      RegisterHandler<CreateCustomerCommand>(Handle);
    }

    private async Task Handle(CreateCustomerCommand command)
    {
      await Repository.ExecuteOnNew<CustomerAggregateRoot>(command.CustomerId, command);
    }
  }
}
