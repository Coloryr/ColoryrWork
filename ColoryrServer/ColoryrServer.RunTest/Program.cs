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
                var data = Console.ReadLine();
                if (data == "stop")
                {
                    TestRun.Stop();
                }
            }
        }
    }
}
