using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SlackNotify;

namespace SlackNotifyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DoStuff();
            Thread th = new Thread(new ThreadStart(DoStuff));
            th.Start();

            while (th.IsAlive)
            {
                Thread.Sleep(0);
            }
        }


        async static void DoStuff()
        {
            Console.WriteLine("Enter your message to send : ");
            string message = Console.ReadLine();

            Console.WriteLine("Enter the channel to send the message to : ");
            string channel = Console.ReadLine();

            await Notify.Send(channel, message, "slack-notify-console");
        }
    }
}
