using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ColoryrServer.Core.FileSystem.Html;

public static class WebBinManager
{
    private static readonly string WebBinLocal = ServerMain.RunLocal + "Web/";
    private static readonly string WebBinStatic = ServerMain.RunLocal + "Static/";

    private static readonly ConcurrentDictionary<string, Dictionary<string, byte[]>> HtmlBinList = new();
    private static readonly ConcurrentDictionary<string, Dictionary<string, StaticTempFile>> HtmlFileList = new();

    public static StaticDir BaseDir { get; private set; }

    private static bool IsRun;
    private static readonly Thread TickThread = new(TickTask);

    private static void TickTask()
    {
        while (IsRun)
        {
            try
            {
                foreach (var item in HtmlFileList)
                {
                    foreach (var item1 in item.Value)
                    {
                        item1.Value.Tick();
                        if (item1.Value.Time <= 0)
                        {
                            item.Value.Remove(item1.Key);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
    }

    private static void Stop()
    {
        BaseDir.Stop();
        IsRun = false;
    }

    public static HttpResponseStream GetStream(HttpRequest request, string arg)
    {
        return FileHttpStream.StartStream(request, $"{WebBinStatic}/{arg}");
    }

    public static void LoadWeb(WebObj obj, List<string> list)
    {
        string dir = WebBinLocal + obj.UUID + "/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        foreach (var item1 in list)
        {
            string local = dir + item1;
            if (File.Exists(local))
            {
                obj.Files.Add(item1);
            }
        }

        Dictionary<string, byte[]> list2 = new();
        //if (ServerMain.Config.Requset.Temp.Contains(info1.Extension))
        //    continue;
        //list2.Add(item1, File.ReadAllBytes(info1.FullName));
        //HtmlBinList.TryAdd(item.UUID, list2);
    }

    public static void Start()
    {
        if (!Directory.Exists(WebBinLocal))
            Directory.CreateDirectory(WebBinLocal);
        if (!Directory.Exists(WebBinStatic))
            Directory.CreateDirectory(WebBinStatic);

        if (!File.Exists(WebBinStatic + "index.html"))
        {
            File.WriteAllText(WebBinStatic + "index.html", WebResource.IndexHtml, Encoding.UTF8);
        }

        if (!File.Exists(WebBinStatic + "404.html"))
        {
            File.WriteAllText(WebBinStatic + "404.html", WebResource._404Html, Encoding.UTF8);
        }

        if (!File.Exists(WebBinStatic + "favicon.ico"))
        {
            File.WriteAllBytes(WebBinStatic + "favicon.ico", WebResource.Icon);
        }

        BaseDir = new StaticDir(WebBinStatic);

        IsRun = true;
        TickThread.Start();
        ServerMain.OnStop += Stop;
    }

    public static byte[] GetByUUID(string uuid)
    {
        if (HtmlBinList.ContainsKey(uuid))
        {
            if (HtmlBinList[uuid].TryGetValue("index.html", out var temp1))
                return temp1;
        }
        return null;
    }

    public static byte[] GetFile(string uuid, string name)
    {
        if (HtmlBinList.ContainsKey(uuid))
        {
            int index = name.LastIndexOf(".");
            string type = name[index..];
            if (ServerMain.Config.Requset.Temp.Contains(type))
            {
                if (!HtmlFileList.ContainsKey(uuid))
                {
                    HtmlFileList.TryAdd(uuid, new());
                }
                if (HtmlFileList[uuid].ContainsKey(name))
                {
                    return HtmlFileList[uuid][name].Data;
                }
                else
                {
                    string local = WebBinLocal + uuid + "/" + name;
                    if (File.Exists(local))
                    {
                        var data = File.ReadAllBytes(local);
                        var obj = new StaticTempFile()
                        {
                            Data = data
                        };
                        obj.Reset();
                        HtmlFileList[uuid].TryAdd(name, obj);
                        return data;
                    }
                    return null;
                }
            }
            else
            {
                if (HtmlBinList[uuid].TryGetValue(name, out var temp1))
                    return temp1;
            }
        }
        return null;
    }

    public static byte[] ReadFile(string uuid, string name)
    {
        string local = WebBinLocal + uuid + "/" + name;
        if (File.Exists(local))
            return File.ReadAllBytes(local);
        return null;
    }

    public static void DeleteAll(WebObj obj, string dir)
    {
        string temp = WebBinLocal + obj.UUID + "/";
        foreach (var item in Directory.GetFiles(temp))
        {
            File.Delete(item);
        }
        Directory.Delete(temp);

        foreach (var item in HtmlBinList[obj.UUID])
        {
            File.WriteAllBytes(dir + item.Key, item.Value);
        }

        HtmlBinList.TryRemove(obj.UUID, out var temp2);
    }

    public static void Save(WebObj obj, string name)
    {
        HtmlBinList[obj.UUID][name] = File.ReadAllBytes(WebBinLocal + obj.UUID + "/" + name);
    }

    public static void SaveFile(WebObj obj, string name, byte[] data)
    {
        string temp = WebBinLocal + obj.UUID + "/";
        if (File.Exists(temp + name))
            File.Delete(temp + name);
        HtmlBinList[obj.UUID].Remove(name);
        HtmlBinList[obj.UUID].Add(name, data);
    }

    public static void StorageWeb(WebObj obj, string name, string code, bool isCode = true, byte[] file = null)
    {
        string temp = WebBinLocal + obj.UUID + "/";
        if (!Directory.Exists(temp))
            Directory.CreateDirectory(temp);
        string local = temp + name;
        FileInfo info = new(local);
        if (!Directory.Exists(info.DirectoryName))
        {
            Directory.CreateDirectory(info.DirectoryName);
        }

        if (isCode)
        {
            var temp1 = name.Split('.');
            if (temp1.Length == 1)
            {
                File.WriteAllText(local, code, Encoding.UTF8);
            }
            else
            {
                if (temp1[1] is "html")
                {
                    code = Tools.CompressHTML(code);
                    File.WriteAllText(local, code, Encoding.UTF8);
                }
                else if (temp1[1] is "css")
                {
                    code = Tools.CompressCSS(code);
                    File.WriteAllText(local, code, Encoding.UTF8);
                }
                else if (temp1[1] is "js")
                {
                    code = Tools.CompressJS(code);
                    File.WriteAllText(local, code, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllText(local, code, Encoding.UTF8);
                }
            }
        }
        else
        {
            File.WriteAllBytes(local, file);
        }

        SaveFile(obj, name, file);
    }
}
