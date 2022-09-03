using ColoryrServer.SDK;
using System;
using System.IO;
using System.Threading;

namespace ColoryrServer.Core.FileSystem;

internal static class FileLoad
{
    public static string Local;

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
        int times = 10;
        Exception e;
        do
        {
            try
            {
                string name = IsTemp ? Local + filename : filename;
                using FileStream Stream = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Thread.Sleep(20);
                var outputdata = new byte[Stream.Length];
                Stream.Read(outputdata, 0, outputdata.Length);
                return outputdata;
            }
            catch (FileNotFoundException e2)
            {
                throw new ErrorDump("读取文件找不到", e2);
            }
            catch (Exception e1)
            {
                e = e1;
                if (times > 0)
                {
                    times--;
                    ServerMain.LogError($"文件:{filename} 被占用 剩余重试测试:{times}");
                    Thread.Sleep(100);
                }
            }
        } while (times > 0);
        throw new ErrorDump("读取文件错误", e);
    }
}
