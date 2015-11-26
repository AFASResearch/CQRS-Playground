using System.Threading.Tasks;
using CQRSMicroservices.Framework;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.Articles
{
  public class ArticleQueryHandler : QueryHandler
  {
    public ArticleQueryHandler()
    {
      RegisterHandler<GetArticleQuery, JObject>(Handle);
    }

    private async Task<JObject> Handle(GetArticleQuery query)
    {
      return await Repository.Get(query.ArticleId);
    }
  }
}
