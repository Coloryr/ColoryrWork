using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.FileSystem.Web;
using ColoryrServer.Core.ServerDebug;
using ColoryrServer.Core.Utils;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Debug.Object;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace ColoryrServer.Core.Http;

public abstract class RouteObj
{
    public bool IsDll { get; protected set; }
    public bool IsReload { get; protected set; }
    public abstract HttpReturn Invoke(HttpDllRequest arg, string function);
}

public class DllRoute : RouteObj
{
    private DllAssembly dll;
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
        IsReload = false;
    }

    public void Lock()
    {
        IsReload = true;
    }
}

public class DebugRoute : RouteObj
{
    public static ConcurrentDictionary<long, Semaphore> locks = new();
    public static ConcurrentDictionary<long, HttpReturn> res = new();

    private static readonly Random random = new();
    private static readonly object obj1 = new();

    public static long Add()
    {
        long item;
        lock (obj1)
        {
            do
            {
                item = random.NextInt64();
            }
            while (locks.ContainsKey(item));
            locks.TryAdd(item, new Semaphore(0, 2));
            res.TryRemove(item, out _);
        }
        return item;
    }

    public static void Res(long id, HttpReturn http)
    {
        if (!res.ContainsKey(id))
        {
            res.TryAdd(id, http);
            locks[id].Release();
        }
    }

    public static void Remove(long id)
    {
        lock (obj1)
        {
            locks.TryRemove(id, out _);
            res.TryRemove(id, out _);
        }
    }

    public DebugRoute()
    {
        IsDll = true;
    }

    public IChannelHandlerContext context;
    public string url;
    public override HttpReturn Invoke(HttpDllRequest arg, string function)
    {
        var obj = new HttpObj()
        {
            requestObj = new()
            {
                Parameter = arg.Parameter,
                RowRequest = arg.RowRequest,
                Cookie = arg.Cookie,
                ContentType = arg.ContentType,
                Data = arg.Data,
                Data1 = arg.Data1,
                Method = arg.Method
            },
            url = url,
            function = function,
            id = Add()
        };
        context.SendHttp(obj);
        locks[obj.id].WaitOne();
        HttpReturn http = res[obj.id];
        Remove(obj.id);

        return http;
    }
}
public class WebRoute : RouteObj
{
    private ConcurrentDictionary<string, byte[]> cache = new();
    private WebObj obj;
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
        return HttpReturnSave.Res404;
    }

    public void Reload(string name)
    {
        IsReload = true;

        if (obj.Codes.ContainsKey(name))
        {
            string text = obj.Codes[name];
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

            cache.AddOrUpdate(name, Encoding.UTF8.GetBytes(text));
        }
        else
        {
            var data = obj.Files[name];
            cache.AddOrUpdate(name, data);
        }

        IsReload = false;
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

    internal void Remove(string name)
    {
        cache.TryRemove(name, out var _);
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

    public static void AddDebug(string name, IChannelHandlerContext context)
    {
        if (!Routes.ContainsKey(name))
        {
            Routes.TryAdd(name, new DebugRoute
            {
                url = name,
                context = context
            });
            ServerMain.LogOut($"开始调试[{name}]");
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

    public static void ReloadFile(string uuid, string name)
    {
        if (Routes.TryGetValue(uuid, out var item))
        {
            if (item is WebRoute)
            {
                (item as WebRoute).Reload(name);
            }
        }
    }

    public static void RemoveFile(string uuid, string name)
    {
        if (Routes.TryGetValue(uuid, out var item))
        {
            if (item is WebRoute)
            {
                (item as WebRoute).Remove(name);
            }
        }
    }

    public static void Lock(string uuid)
    {
        if (Routes.TryGetValue(uuid, out var item))
        {
            if (item is DllRoute)
            {
                (item as DllRoute).Lock();
            }
        }
    }
}
