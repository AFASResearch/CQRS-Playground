using System;
using System.Runtime.Serialization;

namespace CQRSMicroservices.Framework
{
  [Serializable]
  [DataContract]
  public class QueryNotFoundException : Exception
  {
    public QueryNotFoundException(string queryName) : base(queryName)
    {

    }
  }
}
