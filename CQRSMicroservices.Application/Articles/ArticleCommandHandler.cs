using System.Threading.Tasks;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  public class ArticleCommandHandler : CommandHandler
  {
    public ArticleCommandHandler()
    {
      RegisterHandler<CreateArticleCommand>(Handle);
      RegisterHandler<MakeArticleUnsaleableCommand>(Handle);
      RegisterHandler<MakeArticleSaleableCommand>(Handle);
      RegisterHandler<SellArticleCommand>(Handle);
    }

    private async Task Handle(CreateArticleCommand command)
    {
      await Repository.ExecuteOnNew<ArticleAggregateRoot>(command.ArticleId, command);
    }

    private async Task Handle(MakeArticleUnsaleableCommand command)
    {
      await Repository.ExecuteOn<ArticleAggregateRoot>(command.ArticleId, command);
    }

    private async Task Handle(MakeArticleSaleableCommand command)
    {
      await Repository.ExecuteOn<ArticleAggregateRoot>(command.ArticleId, command);
    }

    private async Task Handle(SellArticleCommand command)
    {
      await Repository.ExecuteOn<ArticleAggregateRoot>(command.ArticleId, command);
    }
  }
}
