using System;
using System.Runtime.Serialization;

namespace CQRSMicroservices.Framework
{
  [Serializable]
  [DataContract]
  public class CommandNotFoundException : Exception
    {
      public CommandNotFoundException(string commandName)
        : base(commandName)
      {
        
      }
  }
}
