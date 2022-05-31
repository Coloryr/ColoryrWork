using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;

namespace ColoryrServer.DllManager;
public class DllRun
{
    private readonly static Dictionary<string, object> ErrorObj = new(){ {"res", 0 }, {"text", "服务器内部错误"}  };
    public static HttpReturn DllGo(DllBuildSave dll, HttpRequest arg, string function)
    {
        bool isDebug = false;
        try
        {
            if (function == null)
            {
                function = CodeDemo.DllMain;
            }
            else if (!dll.MethodInfos.ContainsKey(function))
            {
                return new HttpReturn
                {
                    Data = new GetMeesage
                    {
                        Res = 90,
                        Text = "找不到方法",
                        Data = null
                    },
                    Res = ResType.Json,
                    ReCode = 404
                };
            }

            List<AssemblyLoadContext> list = new();
            foreach (var item in AssemblyLoadContext.All)
            {
                list.Add(item);
            }

            isDebug = dll.MethodInfos.ContainsKey("debug");
            MethodInfo mi = dll.MethodInfos[function];

            var obj1 = Activator.CreateInstance(dll.DllType);
            dynamic dllres = mi.Invoke(obj1, new object[1] { arg });
            if (dllres is Dictionary<string, object>)
            {
                return new HttpReturn
                {
                    Data = dllres,
                    Res = ResType.Json
                };
            }
            else if (dllres is string)
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
                    Cookie = dr.SetCookie ? dr.Cookie : null
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
                    Cookie = dr.SetCookie ? dr.Cookie : null
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
                    Cookie = dr.SetCookie ? dr.Cookie : null,
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
                    Cookie = dr.SetCookie ? dr.Cookie : null
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
        catch (Exception e)
        {
            if (e.InnerException is VarDump dump)
            {
                return new HttpReturn
                {
                    Data = dump.Get(),
                    Res = ResType.String,
                    ReCode = 200
                };
            }
            else if (e.InnerException is ErrorDump dump1)
            {
                if(isDebug)
                return new HttpReturn
                {
                    Data = dump1.data + "\n" + e.ToString(),
                    Res = ResType.String,
                    ReCode = 200
                };
                else
                    return new HttpReturn
                    {
                        Data = ErrorObj,
                        Res = ResType.Json,
                        ReCode = 200
                    };
            }
            return new HttpReturn
            {
                Data = e.ToString(),
                Res = ResType.String,
                ReCode = 400
            };
        }
    }
    public static void SocketGo(TcpSocketRequest Head)
    {
        foreach (var Dll in DllStonge.GetWebSocket())
        {
            try
            {
                MethodInfo MI = Dll.MethodInfos[CodeDemo.SocketTcp];
                var Tran = new object[1] { Head };
                var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                var res = MI.Invoke(Assembly, Tran);
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void SocketGo(UdpSocketRequest Head)
    {
        foreach (var Dll in DllStonge.GetWebSocket())
        {
            try
            {
                MethodInfo MI = Dll.MethodInfos[CodeDemo.SocketUdp];
                var Tran = new object[1] { Head };
                var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                var res = MI.Invoke(Assembly, Tran);
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void WebSocketGo(WebSocketMessage Head)
    {
        foreach (var Dll in DllStonge.GetWebSocket())
        {
            try
            {
                MethodInfo MI = Dll.MethodInfos[CodeDemo.WebSocketMessage];
                var Tran = new object[1] { Head };
                var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                var res = MI.Invoke(Assembly, Tran);
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void WebSocketGo(WebSocketOpen Head)
    {
        foreach (var Dll in DllStonge.GetWebSocket())
        {
            try
            {
                MethodInfo MI = Dll.MethodInfos[CodeDemo.WebSocketOpen];
                var Tran = new object[1] { Head };
                var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                var res = MI.Invoke(Assembly, Tran);
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void WebSocketGo(WebSocketClose Head)
    {
        foreach (var Dll in DllStonge.GetWebSocket())
        {
            try
            {
                MethodInfo MI = Dll.MethodInfos[CodeDemo.WebSocketClose];
                var Tran = new object[1] { Head };
                var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                var res = MI.Invoke(Assembly, Tran);
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void RobotGo(RobotRequest Head)
    {
        foreach (var Dll in DllStonge.GetRobot())
        {
            try
            {
                if (Dll.MethodInfos.ContainsKey(CodeDemo.RobotMessage))
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.RobotMessage];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    var res = MI.Invoke(Assembly, Tran);
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void RobotGo(RobotEvent Head)
    {
        foreach (var Dll in DllStonge.GetRobot())
        {
            try
            {
                if (Dll.MethodInfos.ContainsKey("robot"))
                {
                    MethodInfo MI = Dll.MethodInfos["robot"];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    var res = MI.Invoke(Assembly, Tran);
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void RobotGo(RobotAfter Head)
    {
        foreach (var Dll in DllStonge.GetRobot())
        {
            try
            {
                if (Dll.MethodInfos.ContainsKey("after"))
                {
                    MethodInfo MI = Dll.MethodInfos["after"];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    var res = MI.Invoke(Assembly, Tran);
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void MqttGo(MqttConnectionValidator Head)
    {
        foreach (var Dll in DllStonge.GetMqtt())
        {
            try
            {
                if (Dll.MethodInfos.ContainsKey(CodeDemo.MQTTValidator))
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.MQTTValidator];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    var res = MI.Invoke(Assembly, Tran);
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void MqttGo(MqttUnsubscription Head)
    {
        foreach (var Dll in DllStonge.GetMqtt())
        {
            try
            {
                if (Dll.MethodInfos.ContainsKey(CodeDemo.MQTTUnsubscription))
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.MQTTUnsubscription];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    var res = MI.Invoke(Assembly, Tran);
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void MqttGo(MqttMessage Head)
    {
        foreach (var Dll in DllStonge.GetMqtt())
        {
            try
            {
                if (Dll.MethodInfos.ContainsKey(CodeDemo.MQTTMessage))
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.MQTTMessage];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    var res = MI.Invoke(Assembly, Tran);
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }
    public static void MqttGo(MqttSubscription Head)
    {
        foreach (var Dll in DllStonge.GetMqtt())
        {
            try
            {
                if (Dll.MethodInfos.ContainsKey(CodeDemo.MQTTSubscription))
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.MQTTSubscription];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    var res = MI.Invoke(Assembly, Tran);
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is ErrorDump Dump)
                {
                    ServerMain.LogError(Dump.data);
                }
                else
                    ServerMain.LogError(e);
            }
        }
    }

    public static bool TaskGo(TaskUserArg name)
    {
        try
        {
            var dll = DllStonge.GetTask(name.Dll);
            if (dll == null)
                return false;
            MethodInfo MI = dll.MethodInfos[CodeDemo.TaskRun];
            var Assembly = dll.DllType.Assembly.CreateInstance(dll.DllType.FullName, true);
            MI.Invoke(Assembly, name.Arg);
            return true;
        }
        catch (Exception e)
        {
            if (e.InnerException is ErrorDump Dump)
            {
                ServerMain.LogError(Dump.data);
            }
            else
                ServerMain.LogError(e);
        }
        return false;
    }
}
