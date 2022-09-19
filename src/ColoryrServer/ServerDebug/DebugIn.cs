using ColoryrServer.Core;
using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Debug.Object;
using ColoryrWork.Lib.ServerDebug;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.ServerDebug;

internal record DllSave
{
    public Type Type;
    public Dictionary<string, MethodInfo> Methods = new();
}

public static class DebugIn
{
    private static Dictionary<string, DllSave> Saves = new();
    public static void Init(string ip, int port, string key, string iv)
    {
        DebugNetty.Start(ip, port, key, iv);
    }

    public static void Send(IByteBuffer test)
    {
        DebugNetty.Send(test);
    }

    public static void Invoke(HttpObj obj)
    {
        if (Saves.TryGetValue(obj.url, out var save)
            && save.Methods.TryGetValue(obj.function, out var mi))
        {
            var arg = new HttpDllRequest
            {
                Parameter = obj.requestObj.Parameter,
                RowRequest = obj.requestObj.RowRequest,
                Cookie = obj.requestObj.Cookie,
                ContentType = obj.requestObj.ContentType,
                Data = obj.requestObj.Data,
                Data1 = obj.requestObj.Data1,
                Method = obj.requestObj.Method
            };
            object dllres;
            if (mi.IsStatic)
            {
                var temp = Delegate.CreateDelegate(typeof(Dll.DllIN), mi) as Dll.DllIN;
                dllres = temp(arg);
            }
            else
            {
                var obj1 = Activator.CreateInstance(save.Type);
                var temp = Delegate.CreateDelegate(typeof(Dll.DllIN), obj1, mi) as Dll.DllIN;
                dllres = temp(arg);
            }

            if (dllres is string)
            {
                return new HttpReturn
                {
                    Data = dllres,
                    Res = ResType.String
                };
            }
            else if (dllres is HttpResponseString)
            {
                var dr = dllres as HttpResponseString;
                return new HttpReturn
                {
                    Data = dr.Data,
                    Res = ResType.String,
                    Head = dr.Head,
                    ContentType = dr.ContentType,
                    ReCode = dr.ReCode,
                    Cookie = dr.Cookie
                };
            }
            else if (dllres is HttpResponseDictionary)
            {
                var dr = dllres as HttpResponseDictionary;
                return new HttpReturn
                {
                    Data = dr.Data,
                    Res = ResType.Json,
                    Head = dr.Head,
                    ContentType = dr.ContentType,
                    ReCode = dr.ReCode,
                    Cookie = dr.Cookie
                };
            }
            else if (dllres is HttpResponseStream)
            {
                var dr = dllres as HttpResponseStream;
                return new HttpReturn
                {
                    Data = dr.Data,
                    Res = ResType.Stream,
                    Head = dr.Head,
                    ContentType = dr.ContentType,
                    ReCode = dr.ReCode,
                    Cookie = dr.Cookie,
                    Pos = dr.Pos
                };
            }
            else if (dllres is HttpResponseBytes)
            {
                var dr = dllres as HttpResponseBytes;
                return new HttpReturn
                {
                    Data = dr.Data,
                    Res = ResType.Byte,
                    Head = dr.Head,
                    ContentType = dr.ContentType,
                    ReCode = dr.ReCode,
                    Cookie = dr.Cookie
                };
            }
            else
            {
                return new HttpReturn
                {
                    Data = dllres,
                    Res = ResType.Json
                };
            }
        }
    }

    public static void Register(string url, Type dll)
    {
        var list = dll.GetCustomAttribute<DllIN>(true);

        if (list == null)
        {
            throw new Exception("错误的接口类型");
        }

        var save = new DllSave
        {
            Type = dll
        };

        foreach (var item in dll.GetMethods())
        {
            if (item.Name is "GetType" or "ToString" or "Equals" or "GetHashCode"
                || !item.IsPublic)
                continue;
            save.Methods.Add(item.Name, item);
        }

        if (save.Methods.Count == 0)
        {
            throw new Exception(message: "没有方法");
        }

        if (!save.Methods.ContainsKey(CodeDemo.DllMain))
        {
            throw new Exception(message: "没有主方法");
        }

        Saves.Add(url, save);

        var pack = new RegisterObj
        {
            url = url
        }.ToPack();
        DebugNetty.Send(pack);
    }
}
