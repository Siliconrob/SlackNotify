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
        }


        static void DoStuff()
        {
            Console.WriteLine("Enter your message to send : ");
            string message = Console.ReadLine();

            //Console.WriteLine("Enter the channel to send the message to : ");
            //string channel = Console.ReadLine();

            Notify notify = new Notify();
            notify.Send(message);
        }
    }
}
