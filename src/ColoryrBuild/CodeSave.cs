using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ColoryrBuild;

internal static class CodeSave
{
    public static string FilePath = App.RunLocal + @"/CodeTEMP/";
    private static readonly ReaderWriterLockSlim lock1 = new ReaderWriterLockSlim();
    public static void Start()
    {
        if (!Directory.Exists(FilePath))
        {
            Directory.CreateDirectory(FilePath);
        }
        else if (!Directory.Exists(FilePath + "Dll"))
        {
            Directory.CreateDirectory(FilePath + "Dll");
        }
        else if (!Directory.Exists(FilePath + "Class"))
        {
            Directory.CreateDirectory(FilePath + "Class");
        }
        else if (!Directory.Exists(FilePath + "Socket"))
        {
            Directory.CreateDirectory(FilePath + "Socket");
        }
        else if (!Directory.Exists(FilePath + "WebSocket"))
        {
            Directory.CreateDirectory(FilePath + "WebSockets");
        }
        else if (!Directory.Exists(FilePath + "WebSocket"))
        {
            Directory.CreateDirectory(FilePath + "WebSockets");
        }
        else if (!Directory.Exists(FilePath + "Robot"))
        {
            Directory.CreateDirectory(FilePath + "Robot");
        }
        else if (!Directory.Exists(FilePath + "App"))
        {
            Directory.CreateDirectory(FilePath + "App");
        }
    }
    public static void Save(string name, string data)
    {
        try
        {
            lock1.EnterWriteLock();
            if (File.Exists(name))
            {
                File.Delete(name);
            }
            FileInfo info = new(name);
            if (!Directory.Exists(info.DirectoryName))
            {
                Directory.CreateDirectory(info.DirectoryName);
            }
            File.Create(name).Close();
            using FileStream Stream = File.Open(name, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            if (Stream != null && !string.IsNullOrWhiteSpace(data))
            {
                var array = Encoding.UTF8.GetBytes(data);
                Stream.Write(array, 0, array.Length);
            }
        }
        finally
        {
            lock1.ExitWriteLock();
        }
    }
    public static string Load(string code)
    {
        try
        {
            lock1.EnterReadLock();
            using FileStream Stream = File.Open(code, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            if (Stream != null)
            {
                Thread.Sleep(10);
                byte[] bytsize = new byte[Stream.Length];
                Stream.Read(bytsize, 0, (int)Stream.Length);
                return Encoding.UTF8.GetString(bytsize);
            }
        }
        catch (Exception e)
        {
            _ = new InfoWindow("加载代码错误", e.ToString());
        }
        finally
        {
            lock1.ExitReadLock();
        }
        return null;
    }
    public static string GetFileName(CSFileObj code)
    {
        return code.UUID + ".cs";
    }
}
