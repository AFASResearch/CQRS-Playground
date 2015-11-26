using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Customers
{
  public class CustomerCreatedEvent : Event
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