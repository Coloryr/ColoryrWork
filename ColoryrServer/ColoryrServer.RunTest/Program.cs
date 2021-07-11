using System;
using System.Text;

namespace ColoryrServer.RunTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine("Hello World!");
            TestRun.Start();
            while (true)
            {
                var temp = Console.ReadLine();
                var arg = temp.Split(' ');
                if (arg[0] == "stop")
                {
                    TestRun.Stop();
                    return;
                }
                else if (arg[0] == "database")
                { 
                    
                }
            }
        }
    }
}
