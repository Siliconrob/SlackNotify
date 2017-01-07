using System.Diagnostics;
using System.IO;
using System.Linq;
using SlackNotify.Core;

namespace SlackNotify.Console.Core
{
  public class Program
  {
    private static void Main(string[] args)
    {
      var fileName = "settings.json";
      if (args.Length > 0)
      {
        var firstOrDefault = args.FirstOrDefault();
        if (firstOrDefault == null)
        {
          System.Console.WriteLine("Null settings file input");
          return;
        }
        fileName = firstOrDefault.Trim();
      }

      var settingsFile = new FileInfo(fileName);
      if (!settingsFile.Exists)
      {
        System.Console.WriteLine("{0} does not exist", fileName);
        return;
      }

      var result = Sync.Run(() => Notify.SendAsync(settingsFile));
      System.Console.WriteLine($"Send result = {result}");

      if (!Debugger.IsAttached) { return; }
      System.Console.WriteLine("Waiting because debugger is attached.  Press any key to exit");
      System.Console.ReadKey();
    }
  }
}
