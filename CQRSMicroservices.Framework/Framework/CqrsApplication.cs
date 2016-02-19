﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRSMicroservices.Framework
{
  public static class CqrsApplication
  {
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static T GetService<T>()
    {
      object service;
      if(_services.TryGetValue(typeof(T), out service))
      {
        return (T)service;
      }
      return default(T);
    }

    public static void SetService<T>(T service)
    {
      _services[typeof(T)] = service;
    }

    public static void Bootstrap(
      IEnumerable<CommandHandler> commandHandlers,
      IEnumerable<QueryHandler> queryHandlers,
      IEnumerable<QueryModelBuilder> queryModelBuilders)
    {
      SetService(new CommandBus());
      SetService(new EventBus());
      SetService(new QueryBus());
      SetService(new AggregateRootRepository());
      SetService(new QueryRepository());
      SetService<IEventStore>(new InMemoryEventStore());

      RegisterHandlers(commandHandlers, queryHandlers, queryModelBuilders);
    }

    public static void Bootstrap(
      IEnumerable<CommandHandler> commandHandlers,
      IEnumerable<QueryHandler> queryHandlers,
      IEnumerable<QueryModelBuilder> queryModelBuilders,
      IEventStore eventStore)
    {
      SetService(new CommandBus());
      SetService(new EventBus());
      SetService(new QueryBus());
      SetService(new AggregateRootRepository());
      SetService(new QueryRepository());
      SetService<IEventStore>(eventStore);

      RegisterHandlers(commandHandlers, queryHandlers, queryModelBuilders);

      var eventBus = GetService<EventBus>();
      foreach(KeyValuePair<Guid, IEnumerable<Event>> a in eventStore.GetStreams())
      {
        foreach(var @event in a.Value)
        {
          eventBus.Dispatch(@event);
        }
      }
    }

    public static void Bootstrap(
      AggregateRootRepository aggregateRootRepository,
      QueryRepository queryRepository,
      IEnumerable<CommandHandler> commandHandlers,
      IEnumerable<QueryHandler> queryHandlers,
      IEnumerable<QueryModelBuilder> queryModelBuilders)
    {
      SetService(new CommandBus());
      SetService(new EventBus());
      SetService(new QueryBus());
      SetService(new InMemoryEventStore());
      SetService(aggregateRootRepository);
      SetService(queryRepository);
      SetService<IEventStore>(new InMemoryEventStore());

      RegisterHandlers(commandHandlers, queryHandlers, queryModelBuilders);
    }

    private static void RegisterHandlers(IEnumerable<CommandHandler> commandHandlers, IEnumerable<QueryHandler> queryHandlers, IEnumerable<QueryModelBuilder> queryModelBuilders)
    {
      CommandBus commandBus = GetService<CommandBus>();
      EventBus eventBus = GetService<EventBus>();
      QueryBus queryBus = GetService<QueryBus>();

      commandHandlers.ToList().ForEach(ch => commandBus.RegisterHandler(ch));
      queryModelBuilders.ToList().ForEach(qmb => eventBus.RegisterBuilder(qmb));
      queryHandlers.ToList().ForEach(qh => queryBus.RegisterHandler(qh));
    }
  }
}
