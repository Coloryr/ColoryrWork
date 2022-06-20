﻿using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.Core.Utils;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ColoryrServer.Core.Http;

public abstract class RouteObj
{ 
    public bool IsDll { get; protected set; }
    public bool IsReload { get; }
    public abstract HttpReturn Invoke(HttpDllRequest arg, string function);
}

public class DllRoute : RouteObj
{
    private DllAssembly dll;
    public bool IsReload { get; set; }
    public DllRoute(DllAssembly dll)
    {
        IsDll = true;
        this.dll = dll;
    }

    public override HttpReturn Invoke(HttpDllRequest arg, string function)
    {
        return DllRun.DllGo(dll, arg, function);
    }

    public void Update(DllAssembly dll)
    {
        this.dll = dll;
    }
}

public class WebRoute : RouteObj
{
    private ConcurrentDictionary<string, byte[]> cache = new();
    private WebObj obj;
    public bool IsReload { get; set; }
    public WebRoute(WebObj obj)
    {
        IsDll = false;
        this.obj = obj;
        Reload();
    }

    public override HttpReturn Invoke(HttpDllRequest arg, string function)
    {
        if (string.IsNullOrWhiteSpace(function))
        {
            function = "index.html";
        }
        if (cache.TryGetValue(function, out var item))
        {
            return new HttpReturn()
            {
                Res = ResType.Byte,
                Data = item,
                ContentType = ServerContentType.EndType(function)
            };
        }
        return new HttpReturn
        {
            Data = WebBinManager.BaseDir.Html404,
            ContentType = ServerContentType.HTML,
            Res = ResType.Byte
        };
    }

    public void Reload() 
    {
        IsReload = true;
        cache.Clear();
        if (obj.IsVue)
        {
            var dir = new DirectoryInfo(WebFileManager.WebCodeLocal + obj.UUID + "/dist/");
            if (!dir.Exists)
                return;
            foreach (var item in dir.GetFiles())
            {
                string name = item.FullName.Replace(dir.FullName, "").Replace('\\', '/');
                if (name.StartsWith("/"))
                {
                    name = name[1..];
                }
                cache.TryAdd(name, File.ReadAllBytes(item.FullName));
            }
            var dir1 = new DirectoryInfo(WebFileManager.WebCodeLocal + obj.UUID + "/dist/" + obj.UUID);
            if (!dir.Exists)
                return;
            var list = FileUtils.GetDirectoryFile(dir1);
            foreach (var item in list)
            {
                string name = item.FullName.Replace(dir1.FullName, "").Replace('\\', '/');
                if (name.StartsWith("/"))
                {
                    name = name[1..];
                }
                cache.TryAdd(name, File.ReadAllBytes(item.FullName));
            }
        }
        else
        {
            foreach (var item in obj.Codes)
            {
                string name = item.Key;
                string text = item.Value;
                if (name.ToLower().EndsWith(".html"))
                {
                    if (ServerMain.Config.CodeSetting.MinifyHtml)
                    {
                        text = CodeCompressUtils.HTML(text);
                    }
                }
                else if (name.ToLower().EndsWith(".js"))
                {
                    if (ServerMain.Config.CodeSetting.MinifyJS)
                    {
                        text = CodeCompressUtils.JS(text);
                    }
                }
                else if (name.ToLower().EndsWith(".css"))
                {
                    if (ServerMain.Config.CodeSetting.MinifyCSS)
                    {
                        text = CodeCompressUtils.CSS(text);
                    }
                }

                cache.TryAdd(name, Encoding.UTF8.GetBytes(text));
            }
            foreach (var item in obj.Files)
            {
                cache.TryAdd(item.Key, item.Value);
            }
        }
        IsReload = false;
    }
}

public static class HttpInvokeRoute
{
    private static ConcurrentDictionary<string, RouteObj> Routes = new();

    public static bool CheckBase(string name)
    {
        foreach (var item in Routes.Keys)
        {
            if (item == name)
                return true;
        }

        return false;
    }

    public static void AddDll(string name, DllAssembly dll)
    {
        if (Routes.ContainsKey(name))
        {
            (Routes[name] as DllRoute).Update(dll);
        }
        else
        {
            Routes.TryAdd(name, new DllRoute(dll));
        }
    }

    public static void AddWeb(string name, WebObj obj)
    {
        if (Routes.ContainsKey(name))
        {
            (Routes[name] as WebRoute).Reload();
        }
        else
        {
            Routes.TryAdd(name, new WebRoute(obj));
        }
    }

    public static RouteObj Get(string url)
    {
        if (Routes.TryGetValue(url, out var item))
        {
            return item;
        }

        return null;
    }

    public static void Remove(string uuid)
    {
        Routes.TryRemove(uuid, out var _);
    }
}
