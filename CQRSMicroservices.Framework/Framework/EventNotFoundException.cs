using System;
using System.Runtime.Serialization;

namespace CQRSMicroservices.Framework
{
  [Serializable]
  [DataContract]
  public class EventNotFoundException : Exception
  {
    public EventNotFoundException(string commandName)
      : base(commandName)
    {

    }
  }
}
