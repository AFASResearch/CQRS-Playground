using System;
using System.Runtime.Serialization;

namespace CQRSMicroservices.Framework
{
  [Serializable]
  [DataContract]
  public class CommandValidationException : Exception
  {
    public CommandValidationException(string message) : base(message)
    {
    }
  }
}
