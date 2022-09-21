using ColoryrServer.Core.DllManager;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Debug.Object;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

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


    public static HttpResObj? Invoke(HttpObj obj)
    {
        if (Saves.TryGetValue(obj.url, out var save))
        {
            if (string.IsNullOrWhiteSpace(obj.function))
            {
                obj.function = CodeDemo.DllMain;
            }
            else if (!save.Methods.ContainsKey(obj.function))
            {
                return new HttpResObj
                {
                    resopneObj = new()
                    {
                        ReCode = 90,
                        ContentType = ServerContentType.TXT,
                        Data = Encoding.UTF8.GetBytes("找不到方法")
                    },
                    id = obj.id
                };
            }

            MethodInfo mi = save.Methods[obj.function];

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
                return new HttpResObj
                {
                    resopneObj = new()
                    {
                        ReCode = 200,
                        ContentType = ServerContentType.TXT,
                        Data = Encoding.UTF8.GetBytes(dllres as string)
                    },
                    id = obj.id
                };
            }
            else if (dllres is HttpResponseString)
            {
                var dr = dllres as HttpResponseString;
                return new HttpResObj
                {
                    resopneObj = new()
                    {
                        ReCode = dr.ReCode,
                        Head = dr.Head,
                        ContentType = dr.ContentType,
                        Cookie = dr.Cookie,
                        Data = Encoding.UTF8.GetBytes(dr.Data)
                    },
                    id = obj.id
                };
            }
            else if (dllres is HttpResponseDictionary)
            {
                var dr = dllres as HttpResponseDictionary;
                return new HttpResObj
                {
                    resopneObj = new()
                    {
                        ReCode = dr.ReCode,
                        Head = dr.Head,
                        ContentType = dr.ContentType,
                        Cookie = dr.Cookie,
                        Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dr.Data))
                    },
                    id = obj.id
                };
            }
            else if (dllres is HttpResponseStream)
            {
                throw new Exception("Debug不支持流");
            }
            else if (dllres is HttpResponseBytes)
            {
                var dr = dllres as HttpResponseBytes;
                return new HttpResObj
                {
                    resopneObj = new()
                    {
                        ReCode = dr.ReCode,
                        Head = dr.Head,
                        ContentType = dr.ContentType,
                        Cookie = dr.Cookie,
                        Data = dr.Data
                    },
                    id = obj.id
                };
            }
            else
            {
                return new HttpResObj
                {
                    resopneObj = new()
                    {
                        ReCode = 200,
                        ContentType = ServerContentType.JSON,
                        Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dllres))
                    },
                    id = obj.id
                };
            }
        }

        return null;
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

        var obj = new RegisterObj
        {
            url = url
        };
        PackWrite.SendRegister(obj);
    }
}
