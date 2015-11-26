using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  internal class ArticleUnsaleableEvent : Event
  {
    public Guid ArticleId { get; set; }

    public override string ToJson()
    {
      return $@"{{ ""{GetType().FullName}"" : {{
          ""ArticleId"": ""{ArticleId}""
        }}
      }}";
    }
  }
}