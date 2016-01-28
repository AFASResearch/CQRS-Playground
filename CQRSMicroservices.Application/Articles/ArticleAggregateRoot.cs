using System;
using System.Linq;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  public class ArticleAggregateRoot : AggregateRoot
  {
    private bool _available;
    private decimal _price;

    public ArticleAggregateRoot()
    {
      RegisterApply<ArticleCreatedEvent>(Apply);
      RegisterApply<ArticleUnavailableEvent>(Apply);
      RegisterApply<ArticleAvailableEvent>(Apply);
      RegisterHandler<CreateArticleCommand>(Handle);
      RegisterHandler<MakeArticleUnavailableCommand>(Handle);
      RegisterHandler<MakeArticleAvailableCommand>(Handle);
      RegisterHandler<SellArticleCommand>(Handle);
    }

    private void Apply(ArticleCreatedEvent @event)
    {
      _available = true;
      _price = @event.Price;
    }

    private void Apply(ArticleUnavailableEvent @event)
    {
      _available = false;
    }

    private void Apply(ArticleAvailableEvent @event)
    {
      _available = true;
    }

    private void Handle(CreateArticleCommand command)
    {
      if(command.Price <= 0)
      {
        throw new CommandValidationException("Price should be above 0.");
      }
      if(string.IsNullOrEmpty(command.Description) || command.Description.Length > 50)
      {
        throw new CommandValidationException("Description is mandatory, and cannot be longer then 50 characters.");
      }
      if(CqrsApplication.GetService<IEventStore>().GetEvents(command.ArticleId).Any())
      {
        throw new CommandValidationException("ArticleId Already exists.");
      }

      RaiseEvent(new ArticleCreatedEvent
      {
        ArticleId = command.ArticleId,
        Description = command.Description,
        Price = command.Price
      });
    }

    private void Handle(MakeArticleUnavailableCommand command)
    {
      RaiseEvent(new ArticleUnavailableEvent { ArticleId = command.ArticleId });
    }

    private void Handle(MakeArticleAvailableCommand command)
    {
      RaiseEvent(new ArticleAvailableEvent { ArticleId = command.ArticleId });
    }

    private void Handle(SellArticleCommand command)
    {
      if(!_available)
      {
        throw new Exception("This article is unavailable.");
      }
      
      RaiseEvent(new ArticleSoldEvent { ArticleId = command.ArticleId, CustomerId = command.CustomerId, Price = _price });
    }
  }
}
