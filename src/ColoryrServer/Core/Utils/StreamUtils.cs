using ColoryrServer.SDK;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ColoryrServer.Core.Utils;

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