using System.Diagnostics;
using System.Threading;
using System;
using System.Reflection;

namespace ColorMCU
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            //Assembly assembly = Assembly.Load();
            
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
