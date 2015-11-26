using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  internal class ArticleSoldEvent : Event
  {
    public Guid ArticleId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Price { get; set; }


    public override string ToJson()
    {
      return $@"{{ ""{GetType().FullName}"" : {{
          ""ArticleId"": ""{ArticleId}"",
          ""CustomerId"": ""{CustomerId}"",
          ""Price"": {Price.ToString(System.Globalization.CultureInfo.InvariantCulture)}
        }}
      }}";
    }
  }
}