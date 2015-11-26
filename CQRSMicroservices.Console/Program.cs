using System;
using System.Net.Http;
using System.Threading.Tasks;
using CQRSMicroservices.Application;
using CQRSMicroservices.Articles;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Console
{
  class Program
  {
    public static void Main(string[] args)
    {
      Uri server;
      if(args.Length == 1 && Uri.TryCreate(args[0], UriKind.Absolute, out server))
      {
        RunClientServerSample(server).Wait();
      }
      else
      {
        RunInProcessSample().Wait();
      }
    }

    private static async Task RunClientServerSample(Uri server)
    {
      using(var httpClient = new HttpClient())
      {
        {
          var result = await httpClient.PostAsync($"{server}CreateArticle.command", new StringContent(@"{
  ""CQRSMicroservices.Articles.CreateArticleCommand"" : {
    ""ArticleId"": ""d0174342-71b0-4deb-b5b8-d1064d07ec3c"",
    ""Description"": ""iPhone 6S 64 GB Space Gray"",
    ""Price"": 850.99,
  }
}"));
          var resultContent = await result.Content.ReadAsStringAsync();
          System.Console.WriteLine(resultContent);
        }
        {
          var result = await httpClient.PostAsync($"{server}CreateCustomer.command", new StringContent(@"{
  ""CQRSMicroservices.Customers.CreateCustomerCommand"" : {
    ""CustomerId"": ""14b2e8ec-31e2-4a19-b40a-6f77ae3cf4f0"",
    ""Name"": ""AFAS Software""
  }
}"));
          var resultContent = await result.Content.ReadAsStringAsync();
          System.Console.WriteLine(resultContent);
        }
        {
          var result = await httpClient.PostAsync($"{server}SellArticle.command", new StringContent(@"{
  ""CQRSMicroservices.Articles.SellArticleCommand"" : {
    ""ArticleId"": ""d0174342-71b0-4deb-b5b8-d1064d07ec3c"",
    ""CustomerId"": ""14b2e8ec-31e2-4a19-b40a-6f77ae3cf4f0""
  }
}"));
          var resultContent = await result.Content.ReadAsStringAsync();
          System.Console.WriteLine(resultContent);
        }

        System.Console.WriteLine("Wait a second or two to let the QueryModelBuilder catch up...");
        await Task.Delay(2000);

        {
          var result = await httpClient.GetAsync($"{server}CQRSMicroservices/Articles/GetArticleQuery.query?ArticleId=d0174342-71b0-4deb-b5b8-d1064d07ec3c");
          var document = await result.Content.ReadAsStringAsync();
          System.Console.WriteLine(document);
        }

        System.Console.ReadKey();
      }
    }

    private static async Task RunInProcessSample()
    {
      CqrsApplication.Bootstrap(
        Handlers.CommandHandlers,
        Handlers.QueryHandlers,
        Handlers.QueryModelBuilders);

      var iphoneId = Guid.Parse("d0174342-71b0-4deb-b5b8-d1064d07ec3c");

      await CqrsApplication.GetService<CommandBus>().Dispatch(new CreateArticleCommand
      {
        ArticleId = iphoneId,
        Description = "iPhone 6S 64GB Space Gray",
        Price = 850.99m
      });

      var document = await CqrsApplication.GetService<QueryBus>().Dispatch(new GetArticleQuery
      {
        ArticleId = iphoneId
      });

      System.Console.WriteLine(document ?? "null");
      System.Console.ReadKey();
    }
  }
}
