using ColoryrServer.Core.Database;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using HtmlCompression.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Yahoo.Yui.Compressor;

namespace ColoryrServer.Core.Utils;

/// <summary>
/// 快速方法
/// </summary>
public static class ExtensionMethods
{
    public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
    {
        dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
    }
}
public static class CodeCompressUtils
{
    private static readonly HtmlCompressor html = new();
    private static readonly CssCompressor css = new();

    public static string JS(string code)
    {
        return JavaScriptMinifier.Minify(code).ToString();
    }
    public static string CSS(string code)
    {
        return css.Compress(code);
    }
    public static string HTML(string code)
    {
        return html.Compress(code);
    }
}


public static class StreamUtils
{
    public static byte[] JsonOBJ(object obj)
    {
        var data = Tools.ToJson(obj);
        return Encoding.UTF8.GetBytes(data);
    }

    public static byte[] StringOBJ(string data)
    {
        return Encoding.UTF8.GetBytes(data);
    }
}

public static class FileUtils
{
    /// <summary>
    /// 获得指定路径下所有文件名
    /// </summary>
    /// <param name="sw">文件写入流</param>
    /// <param name="path">文件写入流</param>
    /// <param name="indent">输出时的缩进量</param>
    public static FileInfo[] GetFileName(string path)
    {
        DirectoryInfo root = new(path);
        return root.GetFiles();
    }

    /// <summary>
    /// 获得指定路径下所有子目录名
    /// </summary>
    /// <param name="sw">文件写入流</param>
    /// <param name="path">文件夹路径</param>
    /// <param name="indent">输出时的缩进量</param>
    public static List<FileInfo> GetDirectoryFile(DirectoryInfo path)
    {
        List<FileInfo> list = new();
        list.AddRange(path.GetFiles());
        foreach (DirectoryInfo d in path.GetDirectories())
        {
            list.AddRange(GetDirectoryFile(d));
        }

        return list;
    }

    public static string LoadString(string filename, Encoding encoding = null)
    {
        try
        {
            encoding ??= Encoding.UTF8;
            var data = LoadBytes(filename);
            return encoding.GetString(data);
        }
        catch (Exception e)
        {
            throw new ErrorDump("读取文件错误", e);
        }
    }

    public static byte[] LoadBytes(string filename)
    {
        int times = 10;
        Exception e;
        do
        {
            try
            {
                using FileStream Stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
                    ServerMain.LogWarn($"文件:{filename} 被占用 剩余重试测试:{times}");
                    Thread.Sleep(100);
                }
            }
        } while (times > 0);

        throw new ErrorDump("读取文件错误", e);
    }

    public static string StartEdit(string old, List<CodeEditObj> editText)
    {
        var temp = old.Replace("\r", "").Split("\n");
        int arg = 0;
        var list = new List<string>(temp);
        foreach (var item in editText)
        {
            switch (item.Fun)
            {
                case EditFun.Add:
                    list.Insert(item.Line + arg, item.Code);
                    break;
                case EditFun.Edit:
                    list[item.Line + arg] = item.Code;
                    break;
                case EditFun.Remove:
                    list.RemoveAt(item.Line + arg);
                    arg--;
                    break;
            }
        }
        old = "";
        foreach (var item in list)
        {
            old += item + "\n";
        }
        return old[0..^1];
    }

    public static string GetDllName(string local)
    {
        string temp = local.Replace("\\", "/");
        string[] arg = temp.Split('/');
        string name = arg[^1];
        if (name.ToLower().EndsWith(".dll"))
        {
            return name;
        }
        for (int a = arg.Length - 1; a > 0; a++)
        {

        }

        return "";
    }
}

public static class CodeUtils
{
    public static QCodeObj ToQCode(this CSFileCode obj)
    {
        return new QCodeObj()
        {
            uuid = obj.UUID,
            text = obj.Text,
            version = obj.Version,
            createtime = obj.CreateTime,
            updatetime = obj.UpdateTime
        };
    }

    public static WebObj ToWeb(this QWebObj obj)
    {
        return new()
        {
            UUID = obj.uuid,
            Text = obj.text,
            Version = obj.version,
            CreateTime = obj.createtime,
            UpdateTime = obj.updatetime,
            IsVue = obj.vue,
            Codes = new(),
            Files = new()
        };
    }
}


public static class ZipUtils
{
    public static void ZipDeCompress(byte[] zip, string path)
    {
        using ZipInputStream s = new(new MemoryStream(zip));
        ZipEntry theEntry;
        while ((theEntry = s.GetNextEntry()) != null)
        {
            string fileName = Path.GetFileName(theEntry.Name);
            if (fileName != string.Empty)
            {
                var info = new FileInfo(path + @"\" + theEntry.Name);
                var dir = new DirectoryInfo(info.DirectoryName);
                dir.Create();
                using FileStream streamWriter = File.Create(info.FullName);
                int size = 4096;
                byte[] data = new byte[4096];
                while (true)
                {
                    size = s.Read(data, 0, data.Length);
                    if (size > 0)
                    {
                        streamWriter.Write(data, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}