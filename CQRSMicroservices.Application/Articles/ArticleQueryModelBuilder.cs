using System.Threading.Tasks;
using CQRSMicroservices.Customers;
using CQRSMicroservices.Framework;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.Articles
{
  public class ArticleQueryModelBuilder : QueryModelBuilder
  {
    public ArticleQueryModelBuilder()
    {
      RegisterHandler<CustomerCreatedEvent>(Handle);
      RegisterHandler<ArticleCreatedEvent>(Handle);
      RegisterHandler<ArticleUnsaleableEvent>(Handle);
      RegisterHandler<ArticleSaleableEvent>(Handle);
      RegisterHandler<ArticleSoldEvent>(Handle);
    }

    private async Task Handle(CustomerCreatedEvent @event)
    {
      await Repository.Add(@event.CustomerId, new JObject(
        new JProperty("CustomerId", @event.CustomerId),
        new JProperty("Name", @event.Name)));

      // We should check all articles to update customerids with their name
    }

    private async Task Handle(ArticleCreatedEvent @event)
    {
      await Repository.Add(@event.ArticleId, new JObject(
        new JProperty("ArticleId", @event.ArticleId),
        new JProperty("Saleable", true),
        new JProperty("Sold", new JArray()),
        new JProperty("Description", @event.Description),
        new JProperty("Price", @event.Price)));
    }

    private async Task Handle(ArticleSaleableEvent @event)
    {
      var articleObject = await Repository.Get(@event.ArticleId);
      articleObject.Property("Saleable").Value = true;
      await Repository.Update(@event.ArticleId, articleObject);
    }

    private async Task Handle(ArticleUnsaleableEvent @event)
    {
      var articleObject = await Repository.Get(@event.ArticleId);
      articleObject.Property("Saleable").Value = false;
      await Repository.Update(@event.ArticleId, articleObject);
    }

    private async Task Handle(ArticleSoldEvent @event)
    {
      var articleObject = await Repository.Get(@event.ArticleId);

      var customerObject = await Repository.Get(@event.CustomerId);
      var sells = articleObject["Sold"].Value<JArray>();
      sells.Add(customerObject?["Name"].Value<string>() ?? @event.CustomerId.ToString());
      articleObject.Property("Sold").Value = sells;

      await Repository.Update(@event.ArticleId, articleObject);
    }
  }
}
