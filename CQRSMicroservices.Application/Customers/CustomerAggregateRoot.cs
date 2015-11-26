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
      RaiseEvent(new CustomerCreatedEvent { CustomerId = command.CustomerId, Name = command.Name });
    }
  }
}
