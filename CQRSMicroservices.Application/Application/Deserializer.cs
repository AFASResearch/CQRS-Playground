using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CQRSMicroservices.Articles;
using CQRSMicroservices.Customers;
using CQRSMicroservices.Framework;
using Newtonsoft.Json.Linq;

namespace CQRSMicroservices.Application
{
  public class Deserializer : IDeserializer
  {
    /// <summary>
    /// You could implement this with reflection or some other deserialize algorithm.
    /// For this sample, we explicitly implemented this.
    /// </summary>
    /// <param name="commandJson"></param>
    /// <returns></returns>
    public Command CreateCommand(JObject commandJson)
    {
      var commandName = commandJson.Properties().First().Name;
      var commandBody = commandJson.Properties().First().Value.Value<JObject>();

      switch(commandName)
      {
        case "CQRSMicroservices.Articles.CreateArticleCommand":
          return new CreateArticleCommand
          {
            ArticleId = Guid.Parse(commandBody["ArticleId"].Value<string>()),
            Description = commandBody["Description"].Value<string>(),
            Price = decimal.Parse(commandBody["Price"].Value<string>(), System.Globalization.CultureInfo.InvariantCulture)
          };

        case "CQRSMicroservices.Articles.MakeArticleAvailableCommand":

          return new MakeArticleAvailableCommand
          {
            ArticleId = Guid.Parse(commandBody["ArticleId"].Value<string>()),
          };

        case "CQRSMicroservices.Articles.MakeArticleUnavailableCommand":
          return new MakeArticleUnavailableCommand
          {
            ArticleId = Guid.Parse(commandBody["ArticleId"].Value<string>()),
          };

        case "CQRSMicroservices.Articles.SellArticleCommand":
          return new SellArticleCommand
          {
            ArticleId = Guid.Parse(commandBody["ArticleId"].Value<string>()),
            CustomerId = Guid.Parse(commandBody["CustomerId"].Value<string>()),
          };

        case "CQRSMicroservices.Customers.CreateCustomerCommand":
          return new CreateCustomerCommand
          {
            CustomerId = Guid.Parse(commandBody["CustomerId"].Value<string>()),
            Name = commandBody["Name"].Value<string>()
          };

        default:
          throw new CommandNotFoundException(commandName);
      }
    }
    /// <summary>
    /// You could implement this with reflection or some other deserialize algorithm.
    /// For this sample, we explicitly implemented this.
    /// </summary>
    /// <param name="eventJson"></param>
    /// <returns></returns>
    public Event CreateEvent(JObject eventJson)
    {
      var eventName = eventJson.Properties().First().Name;
      var eventBody = eventJson.Properties().First().Value.Value<JObject>();

      switch(eventName)
      {
        case "CQRSMicroservices.Articles.ArticleCreatedEvent":

          return new ArticleCreatedEvent
          {
            ArticleId = Guid.Parse(eventBody["ArticleId"].Value<string>()),
            Description = eventBody["Description"].Value<string>(),
            Price = decimal.Parse(eventBody["Price"].Value<string>(), CultureInfo.InvariantCulture)
          };

        case "CQRSMicroservices.Articles.ArticleAvailableEvent":

          return new ArticleAvailableEvent
          {
            ArticleId = Guid.Parse(eventBody["ArticleId"].Value<string>()),
          };

        case "CQRSMicroservices.Articles.ArticleUnavailableEvent":
          return new ArticleUnavailableEvent
          {
            ArticleId = Guid.Parse(eventBody["ArticleId"].Value<string>()),
          };

        case "CQRSMicroservices.Articles.ArticleSoldEvent":
          return new ArticleSoldEvent
          {
            ArticleId = Guid.Parse(eventBody["ArticleId"].Value<string>()),
            CustomerId = Guid.Parse(eventBody["CustomerId"].Value<string>()),
            Price = decimal.Parse(eventBody["Price"].Value<string>(), CultureInfo.InvariantCulture)
          };

        case "CQRSMicroservices.Customers.CustomerCreatedEvent":
          return new CustomerCreatedEvent
          {
            CustomerId = Guid.Parse(eventBody["CustomerId"].Value<string>()),
            Name = eventBody["Name"].Value<string>()
          };

        default:
          throw new EventNotFoundException(eventName);
      }
    }

    public Query CreateQuery(string name, IEnumerable<KeyValuePair<string, IEnumerable<string>>> query)
    {
      switch(name)
      {
        case "/CQRSMicroservices/Articles/GetArticleQuery.query":
          return new GetArticleQuery
          {
            ArticleId = Guid.Parse(query.First(kv => kv.Key == "ArticleId").Value.First())
          };

        case "/CQRSMicroservices/Customers/GetCustomerQuery.query":
          return new GetCustomerQuery
          {
            CustomerId = Guid.Parse(query.First(kv => kv.Key == "CustomerId").Value.First())
          };

        default:
          throw new QueryNotFoundException(name);
      }
    }
  }
}
