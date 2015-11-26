using System.Collections.Generic;
using CQRSMicroservices.Articles;
using CQRSMicroservices.Customers;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Application
{
  public static class Handlers
  {
    public static readonly IEnumerable<QueryHandler> QueryHandlers = new QueryHandler[] { new ArticleQueryHandler(), new CustomerQueryHandler() };

    public static readonly IEnumerable<CommandHandler> CommandHandlers = new CommandHandler[] { new ArticleCommandHandler(), new CustomerCommandHandler() };

    public static readonly IEnumerable<QueryModelBuilder> QueryModelBuilders = new QueryModelBuilder[] { new ArticleQueryModelBuilder(), new CustomerQueryModelBuilder() };

  }
}
