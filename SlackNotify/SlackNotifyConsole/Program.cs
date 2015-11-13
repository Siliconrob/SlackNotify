using System;
using System.IO;
using System.Linq;
using SlackNotify;

namespace SlackNotifyConsole
{
  class Program
  {
    private static void Main(string[] args)
    {
      var fileName = "settings.json";
      if (args.Length > 0)
      {
        var firstOrDefault = args.FirstOrDefault();
        if (firstOrDefault == null)
        {
          Console.WriteLine("Null settings file input");
          return;
        }
        fileName = firstOrDefault.Trim();
      }

      var settingsFile = new FileInfo(fileName);
      if (!settingsFile.Exists)
      {
        Console.WriteLine("{0} does not exist", fileName);
        return;
      }
      var notify = new Notify(settingsFile);
      notify.Send();
    }
  }
}
