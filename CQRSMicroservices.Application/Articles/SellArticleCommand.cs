using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  internal class SellArticleCommand : Command
  {
    public Guid ArticleId { get; set; }

    public Guid CustomerId { get; set; }

    public override string ToJson()
    {
      return $@"{{ ""{GetType().FullName}"" : {{
          ""ArticleId"": ""{ArticleId}"",
          ""CustomerId"": ""{CustomerId}""
        }}
      }}";
    }
  }
}