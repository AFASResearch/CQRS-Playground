using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Customers
{
  public class CreateCustomerCommand : Command
  {
    public Guid CustomerId { get; set; }
    public string Name { get; set; }

    public override string ToJson()
    {
      return $@"{{ ""{GetType().FullName}"" : {{
          ""CustomerId"": ""{CustomerId}"",
          ""Name"": ""{Name}""
        }}
      }}";
    }
  }
}