using ColoryrServer.Core.DllManager;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ColoryrServer.Core.Http;

public abstract class RouteObj
{ 
    public bool IsDll { get; protected set; }
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
    }
}

public class WebRoute : RouteObj
{
    private WebObj obj;
    public WebRoute(WebObj obj) 
    {
        IsDll = false;
        this.obj = obj;
    }

    public override HttpReturn Invoke(HttpDllRequest arg, string function)
    {
        return null;
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

    public static void AddWeb(string uuid, WebObj obj)
{
        Routes.TryAdd(uuid, new WebRoute(obj));
    }

    public static RouteObj Get(string url) 
    {
        if (Routes.TryGetValue(url, out var item))
        {
            return item;
        }

        return null;
    }
}
