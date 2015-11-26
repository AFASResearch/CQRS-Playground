using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  public class ArticleCreatedEvent : Event
  {
    public decimal Price { get; set; }
    public string Description { get; set; }
    public Guid ArticleId { get; set; }

    public override string ToJson()
    {
      return $@"{{ ""{GetType().FullName}"" : {{
          ""ArticleId"": ""{ArticleId}"",
          ""Description"": ""{Description}"",
          ""Price"": {Price.ToString(System.Globalization.CultureInfo.InvariantCulture)}
        }}
      }}";
    }
  }
}