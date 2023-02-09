using ColoryrServer.Core.Database;
using ColoryrServer.Core.Http;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace ColoryrServer.Core.FileSystem.Managers;

public static class WebFileManager
{
    /// <summary>
    /// 代码存放
    /// </summary>
    public static readonly string WebCodeLocal = ServerMain.RunLocal + "Codes/Static/";
    /// <summary>
    /// Web项目删除
    /// </summary>
    private static readonly string WebRemoveLocal = ServerMain.RunLocal + "Removes/Web/";

    /// <summary>
    /// Web项目储存
    /// </summary>
    public static ConcurrentDictionary<string, WebObj> HtmlCodeList { get; } = new();

    /// <summary>
    /// 获取Web项目
    /// </summary>
    /// <param name="uuid">UUID</param>
    /// <returns></returns>
    public static WebObj GetHtml(string uuid)
    {
        if (!HtmlCodeList.TryGetValue(uuid, out var item))
        {
            return null;
        }
        return item;
    }

    private static void Stop()
    {
        HtmlCodeList.Clear();
    }
    public static void Start()
    {
        ServerMain.OnStop += Stop;
        if (!Directory.Exists(WebCodeLocal))
            Directory.CreateDirectory(WebCodeLocal);
        if (!Directory.Exists(WebRemoveLocal))
            Directory.CreateDirectory(WebRemoveLocal);

        WebDatabase.Start();

        var list = WebDatabase.LoadAll();
        foreach (var item in list)
        {
            string dir = WebCodeLocal + item.UUID + "/";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var info = new DirectoryInfo(dir);

            Add(item);

            ServerMain.LogOut($"加载Web[{item.UUID}]");
        }
    }
    public static void StorageCode(WebObj obj, string name, string code)
    {
        string local = WebCodeLocal + obj.UUID + "/" + name;
        FileInfo info = new(local);
        if (!Directory.Exists(info.DirectoryName))
        {
            Directory.CreateDirectory(info.DirectoryName);
        }
        WebDatabase.StorageCode(obj, name, code);

        if (!obj.Codes.ContainsKey(name))
        {
            obj.Codes.Add(name, code);
        }
        else
        {
            obj.Codes[name] = code;
        }

        //更新bin
        if (obj.IsVue)
        {
            File.WriteAllText(local, code);
        }
    }
    public static void SetIsVue(WebObj obj, bool IsVue)
    {
        obj.IsVue = IsVue;
        WebDatabase.Storage(obj);

        HttpInvokeRoute.AddWeb(obj.UUID, obj);
    }
    public static void DeleteAll(WebObj obj)
    {
        ServerMain.LogOut($"删除Web[{obj.UUID}]");

        HttpInvokeRoute.Remove(obj.UUID);
        string time = string.Format("{0:s}", DateTime.Now).Replace(":", ".");
        string dir = WebRemoveLocal + $"{obj.UUID}-{time}" + "/";
        Directory.CreateDirectory(dir);
        File.WriteAllText(dir + "info.txt", $@"UUID:{obj.UUID}
Text:{obj.Text}
IsVue:{obj.IsVue}
Version:{obj.Version}
CreateTime:{obj.CreateTime}
UpdateTime:{obj.UpdateTime}");
        foreach (var item in obj.Codes)
        {
            var info = new FileInfo(dir + item.Key);
            info.Directory?.Create();
            File.WriteAllText(dir + item.Key, item.Value);
        }
        foreach (var item in obj.Files)
        {
            var info = new FileInfo(dir + item.Key);
            info.Directory?.Create();
            File.WriteAllBytes(info.FullName, item.Value);
        }

        string dir1 = WebCodeLocal + obj.UUID;
        Directory.Delete(dir1, true);

        HtmlCodeList.TryRemove(obj.UUID, out var temp1);
        WebDatabase.Remove(obj.UUID);
    }
    public static void AddItem(WebObj obj, string name, bool isCode, string code = null, byte[] data = null)
    {
        if (isCode)
        {
            HtmlCodeList[obj.UUID].Codes.Add(name, code);
            StorageCode(obj, name, code);
        }
        else
        {
            HtmlCodeList[obj.UUID].Files.Add(name, data);
            WebDatabase.StorageFile(obj, name, data);
        }

        obj.Up();
        WebDatabase.Storage(obj);

        if (!obj.IsVue)
        {
            HttpInvokeRoute.ReloadFile(obj.UUID, name);
        }
    }

    public static void StorageFile(WebObj obj, string name, byte[] data)
    {
        WebDatabase.StorageFile(obj, name, data);

        if (obj.IsVue)
        {
            string local = WebCodeLocal + obj.UUID + "/" + name;
            File.WriteAllBytes(local, data);
        }
    }

    public static void Add(WebObj obj)
    {
        HtmlCodeList.TryAdd(obj.UUID, obj);
        HttpInvokeRoute.AddWeb(obj.UUID, obj);
    }

    public static void New(WebObj obj)
    {
        Add(obj);
        foreach (var item in obj.Codes)
        {
            StorageCode(obj, item.Key, item.Value);
        }
        foreach (var item in obj.Files)
        {
            WebDatabase.StorageFile(obj, item.Key, item.Value);
        }
        obj.Up();
        WebDatabase.Storage(obj);
    }

    public static void RemoveItem(WebObj obj, string name, bool isCode)
    {
        if (isCode)
        {
            HtmlCodeList[obj.UUID].Codes.Remove(name);
            WebDatabase.RemoveCode(obj, name);
        }
        else
        {
            HtmlCodeList[obj.UUID].Files.Remove(name);
            WebDatabase.RemoveFile(obj, name);
        }
        obj.Up();
        WebDatabase.Storage(obj);

        if (!obj.IsVue)
        {
            HttpInvokeRoute.RemoveFile(obj.UUID, name);
        }
    }
}