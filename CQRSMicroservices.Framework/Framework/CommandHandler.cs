using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public abstract class CommandHandler
  {
    private readonly Dictionary<Type, Func<Command, Task>> _handlers = new Dictionary<Type, Func<Command, Task>>();

    public AggregateRootRepository Repository => CqrsApplication.GetService<AggregateRootRepository>();

    public async Task Handle(Command command)
    {
      Func<Command, Task> handler;
      if(_handlers.TryGetValue(command.GetType(), out handler))
      {
        await handler(command);
      }
      else
      {
        throw new NotImplementedException($"No handler for commandtype {command.GetType().FullName}");
      }
    }

    public void RegisterHandler<T>(Func<T, Task> handler) where T : Command
    {
      _handlers.Add(typeof(T), c => handler((T)c));
    }

    public IEnumerable<Type> GetHandledCommands()
    {
      return _handlers.Keys;
    }
    
  }
}
