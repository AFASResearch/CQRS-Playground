using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Customers
{
  public class GetCustomerQuery : Query
  {
    public Guid CustomerId { get; set; }
  }
}