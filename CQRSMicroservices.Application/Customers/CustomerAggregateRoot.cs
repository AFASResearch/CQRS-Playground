using System.Linq;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Customers
{
  public class CustomerAggregateRoot : AggregateRoot
  {
    public CustomerAggregateRoot()
    {
      RegisterHandler<CreateCustomerCommand>(Handle);
    }

    private void Handle(CreateCustomerCommand command)
    {
      if(CqrsApplication.GetService<IEventStore>().GetEvents(command.CustomerId).Any())
      {
        throw new CommandValidationException("CustomerId Already exists.");
      }
      RaiseEvent(new CustomerCreatedEvent { CustomerId = command.CustomerId, Name = command.Name });
    }
  }
}
