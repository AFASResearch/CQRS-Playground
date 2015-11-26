using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRSMicroservices.Framework
{
  public class CommandBus
  {
    private readonly Dictionary<Type, CommandHandler> _handlers = new Dictionary<Type, CommandHandler>();

    public async Task Dispatch(Command command)
    {
      CommandHandler handler;
      if(_handlers.TryGetValue(command.GetType(), out handler))
      {
        await handler.Handle(command);
      }
      else
      {
        throw new NotImplementedException($"No handler for commandtype {command.GetType().FullName}");
      }
    }

    public void RegisterHandler(CommandHandler commandHandler)
    {
      foreach(var commandType in commandHandler.GetHandledCommands())
      {
        _handlers.Add(commandType, commandHandler);
      }
    }
  }
}
