using System.Threading.Tasks;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  public class ArticleCommandHandler : CommandHandler
  {
    public ArticleCommandHandler()
    {
      RegisterHandler<CreateArticleCommand>(Handle);
      RegisterHandler<MakeArticleUnavailableCommand>(Handle);
      RegisterHandler<MakeArticleAvailableCommand>(Handle);
      RegisterHandler<SellArticleCommand>(Handle);
    }

    private async Task Handle(CreateArticleCommand command)
    {
      await Repository.ExecuteOnNew<ArticleAggregateRoot>(command.ArticleId, command);
    }

    private async Task Handle(MakeArticleUnavailableCommand command)
    {
      await Repository.ExecuteOn<ArticleAggregateRoot>(command.ArticleId, command);
    }

    private async Task Handle(MakeArticleAvailableCommand command)
    {
      await Repository.ExecuteOn<ArticleAggregateRoot>(command.ArticleId, command);
    }

    private async Task Handle(SellArticleCommand command)
    {
      await Repository.ExecuteOn<ArticleAggregateRoot>(command.ArticleId, command);
    }
  }
}
