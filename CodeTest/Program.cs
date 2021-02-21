using System;
using System.Threading;

namespace CodeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            EventWaitHandle eventWait = new EventWaitHandle(true, EventResetMode.ManualReset);
            new Thread(() =>
            {
                while (true)
                {
                    eventWait.WaitOne();
                    Console.WriteLine("tick1");
                    Thread.Sleep(1000);
                }
            }).Start();
            new Thread(() =>
            {
                while (true)
                {
                    eventWait.WaitOne();
                    Console.WriteLine("tick2");
                    Thread.Sleep(1000);
                }
            }).Start();
            new Thread(() =>
            {
                while (true)
                {
                    eventWait.WaitOne();
                    Console.WriteLine("tick3");
                    Thread.Sleep(1000);
                }
            }).Start();
            while(true)
            {
                string temp = Console.ReadLine();
                switch (temp)
                {
                    case "run":
                        eventWait.Set();
                        break;
                    case "stop":
                        eventWait.Reset();
                        break;
                }
            }
        }
    }
}
