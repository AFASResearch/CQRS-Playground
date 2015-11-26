using System;
using CQRSMicroservices.Framework;

namespace CQRSMicroservices.Articles
{
  public class ArticleAggregateRoot : AggregateRoot
  {
    private bool _saleable;
    private decimal _price;

    public ArticleAggregateRoot()
    {
      RegisterApply<ArticleCreatedEvent>(Apply);
      RegisterApply<ArticleUnsaleableEvent>(Apply);
      RegisterApply<ArticleSaleableEvent>(Apply);
      RegisterHandler<CreateArticleCommand>(Handle);
      RegisterHandler<MakeArticleUnsaleableCommand>(Handle);
      RegisterHandler<MakeArticleSaleableCommand>(Handle);
      RegisterHandler<SellArticleCommand>(Handle);
    }

    private void Apply(ArticleCreatedEvent @event)
    {
      _saleable = true;
      _price = @event.Price;
    }

    private void Apply(ArticleUnsaleableEvent @event)
    {
      _saleable = false;
    }

    private void Apply(ArticleSaleableEvent @event)
    {
      _saleable = true;
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

      RaiseEvent(new ArticleCreatedEvent
      {
        ArticleId = command.ArticleId,
        Description = command.Description,
        Price = command.Price
      });
    }

    private void Handle(MakeArticleUnsaleableCommand command)
    {
      RaiseEvent(new ArticleUnsaleableEvent { ArticleId = command.ArticleId });
    }

    private void Handle(MakeArticleSaleableCommand command)
    {
      RaiseEvent(new ArticleSaleableEvent { ArticleId = command.ArticleId });
    }

    private void Handle(SellArticleCommand command)
    {
      if(!_saleable)
      {
        throw new Exception($"This article is unsaleable.");
      }
      
      RaiseEvent(new ArticleSoldEvent { ArticleId = command.ArticleId, CustomerId = command.CustomerId, Price = _price });
    }
  }
}
