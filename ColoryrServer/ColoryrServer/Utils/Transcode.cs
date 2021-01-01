using ColoryrServer.FileSystem;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ColoryrServer
{
    public enum TranscodeType
    {
        mp3, amr
    }
    internal class Transcode
    {
        public static byte[] Start(TranscodeType InputType, TranscodeType OutputType, byte[] InputData)
        {
            Guid guid = Guid.NewGuid();
            string uuid = guid.ToString().Replace("-", "");
            string inputfile = FileTemp.Local + uuid + "." + InputType.ToString();
            string output = FileTemp.Local + uuid + "." + OutputType.ToString();
            File.WriteAllBytes(inputfile, InputData);

            Process Process = new Process();
            Process.StartInfo.FileName = ServerMain.Config.MPGE;
            Process.StartInfo.Arguments = " -i \"" + inputfile + "\" -ar 8000 -ab 12.2k -ac 1 \"" + output + '\"';
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.CreateNoWindow = true;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.RedirectStandardInput = true;
            Process.StartInfo.RedirectStandardError = true;

            Process.Start();
            Process.BeginErrorReadLine();
            Process.WaitForExit();

            FileStream OpenFile = new FileStream(output, FileMode.Open, FileAccess.Read);
            Thread.Sleep(20);
            var outputdata = new byte[OpenFile.Length];
            OpenFile.Read(outputdata, 0, outputdata.Length);
            OpenFile.Close();

            File.Delete(inputfile);
            File.Delete(output);

            return outputdata;
        }
    }
}
