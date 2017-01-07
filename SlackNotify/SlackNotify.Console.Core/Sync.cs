using System;
using System.Threading;
using System.Threading.Tasks;

namespace SlackNotify.Console.Core
{
  public static class Sync
  {
    private static readonly TaskFactory Factory = new TaskFactory(
      CancellationToken.None,
      TaskCreationOptions.None,
      TaskContinuationOptions.None,
      TaskScheduler.Default);

    public static T Run<T>(Func<Task<T>> fn)
    {
      return Factory.StartNew(fn).Unwrap().GetAwaiter().GetResult();
    }

    public static void Run(Func<Task> fn)
    {
      Factory.StartNew(fn).Unwrap().GetAwaiter().GetResult();
    }
  }
}