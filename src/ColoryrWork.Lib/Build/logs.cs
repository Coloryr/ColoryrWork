﻿using System.Text;

namespace ColoryrWork.Lib.Build;

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
                File.AppendAllText(RunLocal + log, a + Environment.NewLine, Encoding.UTF8);
            }
            catch
            { }
        }
    }
}
