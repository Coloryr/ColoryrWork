using System.Diagnostics;
using System.Threading;

namespace ColorMCU
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
