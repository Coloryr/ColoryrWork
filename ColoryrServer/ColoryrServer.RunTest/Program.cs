using ColoryrServer;
using System;

namespace ColorServerRunTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ServerMain.Start();
            while (true)
            {
                if (Console.ReadLine() == "stop")
                {
                    ServerMain.Stop();
                    break;
                }
            }
        }
    }
}
