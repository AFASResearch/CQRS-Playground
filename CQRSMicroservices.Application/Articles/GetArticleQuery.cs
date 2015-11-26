using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  public class GetArticleQuery : Query
  {
    public Guid ArticleId { get; set; }
  }
}
