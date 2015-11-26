using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace CQRSMicroservicesSample.Webserver.Middleware
{
  public static class Extensions
  {
    /// <summary>
    ///   Gets the extension of a path string without the . (so: NewPerson.command will return "command")
    /// </summary>
    /// <param name="pathString">The path string.</param>
    public static string GetExtension(this PathString pathString)
    {
      if(pathString.HasValue)
      {
        string value = pathString.Value;
        int pos = value.LastIndexOf('.');
        if(pos > 0)
        {
          return value.Substring(pos + 1);
        }
      }
      return null;
    }
  }
}
