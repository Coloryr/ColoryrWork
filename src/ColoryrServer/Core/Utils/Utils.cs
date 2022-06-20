using ColoryrServer.SDK;
using HtmlCompression.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yahoo.Yui.Compressor;

namespace ColoryrServer.Core.Utils;

internal static class CodeCompressUtils
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


public class StreamUtils
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

public class FileUtils
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