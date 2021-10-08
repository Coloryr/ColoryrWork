using System;
using System.IO;
using System.Text;

namespace Lib.Build
{
    public class Logs
    {
        public string log = "logs.log";
        public string RunLocal;
        private readonly object lockobject = new();

        public Logs(string RunLocal)
        {
            this.RunLocal = RunLocal;
            if (!File.Exists(RunLocal + log))
                File.Create(RunLocal + log).Close();
        }

        public void LogWrite(string a)
        {
            lock (lockobject)
            {
                try
                {
                    var date = DateTime.Now;
                    string year = date.ToShortDateString().ToString();
                    string time = date.ToLongTimeString().ToString();
                    string write = "[" + year + "]" + "[" + time + "]" + a;
                    File.AppendAllText(RunLocal + log, write + Environment.NewLine, Encoding.UTF8);
                }
                catch
                { }
            }
        }
    }
}
