using ColoryrServer.SDK;
using System;
using System.IO;
using System.Threading;

namespace ColoryrServer.FileSystem
{
    public class FileTemp
    {
        public static string Local;

        public static void Start()
        {
            Local = ServerMain.RunLocal + @"/TempFile/";
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
        }

        public static string LoadString(string filename, bool IsTemp = true)
        {
            try
            {
                if (IsTemp)
                    return File.ReadAllText(Local + filename);
                else
                    return File.ReadAllText(filename);
            }
            catch (Exception e)
            {
                throw new ErrorDump("读取文件错误", e);
            }
        }

        public static byte[] LoadBytes(string filename, bool IsTemp = true)
        {
            try
            {
                string name = IsTemp ? Local + filename : filename;
                using (FileStream OpenFile = new FileStream(name, FileMode.Open, FileAccess.Read))
                {
                    Thread.Sleep(20);
                    var outputdata = new byte[OpenFile.Length];
                    OpenFile.Read(outputdata, 0, outputdata.Length);
                    return outputdata;
                }
            }
            catch (Exception e)
            {
                throw new ErrorDump("读取文件错误", e);
            }
        }
    }
}
