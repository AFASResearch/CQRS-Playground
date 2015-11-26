using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  public class MakeArticleUnsaleableCommand : Command
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