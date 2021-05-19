using System;
using System.Collections.Generic;
using System.Threading;

namespace CodeTest
{
    [AttributeUsage(AttributeTargets.Method , AllowMultiple = false)]
    public class NotesSDK : Attribute
    {
        public string Text;
        public string[] Input;
        public string[] Output;

        public NotesSDK(string Text, string[] Input = null, string[] Output = null)
        {
            this.Text = Text;
            this.Input = Input ?? new string[1];
            this.Output = Output ?? new string[1];
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
        }

        [NotesSDK("", null, null)]
        public static void test()
        { 
        
        }
    }
}
