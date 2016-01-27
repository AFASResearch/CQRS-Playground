using System;
using System.Reflection;

namespace CQRSMicroservices.Framework
{
  public class Event
  {

    public DateTime _EventDate { get; set; }
    public virtual string ToJson()
    {
      string s;
      s = $@"{{ ""{GetType().FullName}"" : {{";

      var fields = this.GetType().GetProperties();
      foreach(PropertyInfo e in fields)
      {
        if(e.Name != "_EventDate")
        {
          s += $@" ""{e.Name}"":""{e.GetValue(this)}"", ";
        }
      }
      s += "}   }";
      return s;
    }
  }
}