using ColoryrServer.Core.Database;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ColoryrServer.Core.Dll;
/// <summary>
/// DLL反射
/// </summary>
internal static partial class DllRun
{
    /// <summary>
    /// 错误返回
    /// </summary>
    private readonly static Dictionary<string, object> ErrorObj = new() { { "res", 0 }, { "text", "服务器内部错误" } };

    private readonly static Dictionary<string, object> ErrorObj1 = new() { { "res", 0 }, { "text", "服务器运行发生了错误" } };

    private readonly static Dictionary<string, object> ErrorObj2 = new() { { "res", 0 }, { "text", "服务器参数错误" } };
    /// <summary>
    /// 接口DLL反射
    /// </summary>
    /// <param name="dll">dll储存</param>
    /// <param name="arg">参数</param>
    /// <param name="function">方法名</param>
    /// <returns></returns>
    internal static CoreHttpReturn DllGo(DllAssembly dll, HttpDllRequest arg, string function)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(function))
            {
                function = CodeDemo.DllMain;
            }
            else if (!dll.MethodInfos.ContainsKey(function))
            {
                return new CoreHttpReturn
                {
                    Data = new GetMeesage
                    {
                        Res = 90,
                        Text = "找不到方法"
                    },
                    Res = ResType.Json,
                    ReCode = 404
                };
            }

            MethodInfo mi = dll.MethodInfos[function];

            object dllres;
            if (mi.IsStatic)
            {
                if (mi.ReturnType == typeof(Task<object>))
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.DllAsyncIN), mi) as Dll.DllAsyncIN;
                    dllres = temp!(arg).Result;
                }
                else
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.DllIN), mi) as Dll.DllIN;
                    dllres = temp!(arg);
                }
            }
            else
            {
                var obj1 = Activator.CreateInstance(dll.SelfType);
                if (mi.ReturnType == typeof(Task<object>))
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.DllAsyncIN), obj1, mi) as Dll.DllAsyncIN;
                    dllres = temp!(arg).Result;
                }
                else
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.DllIN), obj1, mi) as Dll.DllIN;
                    dllres = temp!(arg);
                }
            }

            if (dllres is string)
            {
                return new CoreHttpReturn
                {
                    Data = dllres,
                    Res = ResType.String
                };
            }
            else if (dllres is HttpResponseString dr)
            {
                return new CoreHttpReturn
                {
                    Data = dr.Data,
                    Res = ResType.String,
                    Head = dr.Head,
                    ContentType = dr.ContentType,
                    ReCode = dr.ReCode,
                    Cookie = dr.Cookie
                };
            }
            else if (dllres is HttpResponseDictionary dr1)
            {
                return new CoreHttpReturn
                {
                    Data = dr1.Data,
                    Res = ResType.Json,
                    Head = dr1.Head,
                    ContentType = dr1.ContentType,
                    ReCode = dr1.ReCode,
                    Cookie = dr1.Cookie
                };
            }
            else if (dllres is HttpResponseStream dr2)
            {
                return new CoreHttpReturn
                {
                    Data = dr2.Data,
                    Res = ResType.Stream,
                    Head = dr2.Head,
                    ContentType = dr2.ContentType,
                    ReCode = dr2.ReCode,
                    Cookie = dr2.Cookie,
                    Pos = dr2.Pos
                };
            }
            else if (dllres is HttpResponseBytes dr3)
            {
                return new CoreHttpReturn
                {
                    Data = dr3.Data,
                    Res = ResType.Byte,
                    Head = dr3.Head,
                    ContentType = dr3.ContentType,
                    ReCode = dr3.ReCode,
                    Cookie = dr3.Cookie
                };
            }
            else
            {
                return new CoreHttpReturn
                {
                    Data = dllres,
                    Res = ResType.Json
                };
            }
        }
        catch (Exception e)
        {
            _ = Task.Run(() => ServiceOnError(e));
            if (e.InnerException is VarDump dump)
            {
                if (dll.Debug)
                    return new CoreHttpReturn
                    {
                        Data = dump.Get(),
                        Res = ResType.String,
                        ReCode = 200
                    };
                else
                    return new CoreHttpReturn
                    {
                        Data = ErrorObj2,
                        Res = ResType.Json,
                        ReCode = 500
                    };
            }
            else if (e.InnerException is ErrorDump dump1)
            {
                string error = dump1.data + "\n" + e.ToString();
                LogDatabsae.PutError($"[Dll]{dll.Name}", error);
                if (dll.Debug)
                    return new CoreHttpReturn
                    {
                        Data = error,
                        Res = ResType.String,
                        ReCode = 500
                    };
                else
                    return new CoreHttpReturn
                    {
                        Data = ErrorObj,
                        Res = ResType.Json,
                        ReCode = 500
                    };
            }
            else
            {
                string error = e.ToString();
                LogDatabsae.PutError($"[Dll]{dll.Name}", error);
                if (dll.Debug)
                    return new CoreHttpReturn
                    {
                        Data = error,
                        Res = ResType.String,
                        ReCode = 500
                    };
                else
                    return new CoreHttpReturn
                    {
                        Data = ErrorObj1,
                        Res = ResType.Json,
                        ReCode = 500
                    };
            }
        }
    }
    /// <summary>
    /// TCP数据
    /// </summary>
    /// <param name="head"></param>
    public static void SocketGo(SocketTcpRequest head)
    {
        foreach (var dll in AssemblyList.GetSocket())
        {
            try
            {
                if (dll.Netty || !dll.MethodInfos.ContainsKey(CodeDemo.SocketTcp))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.SocketTcp];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.SocketTcpIn), mi) as Dll.SocketTcpIn;
                    dllres = temp!(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.SocketTcpIn), obj1, mi) as Dll.SocketTcpIn;
                    dllres = temp!(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Socket]{dll.Name}", error);
                ServerMain.LogError("[Socket]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// UDP数据
    /// </summary>
    /// <param name="head"></param>
    public static void SocketGo(SocketUdpRequest head)
    {
        foreach (var dll in AssemblyList.GetSocket())
        {
            try
            {
                if (dll.Netty || !dll.MethodInfos.ContainsKey(CodeDemo.SocketUdp))
                    continue;
                MethodInfo mi = dll.MethodInfos[CodeDemo.SocketUdp];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.SocketUdpIn), mi) as Dll.SocketUdpIn;
                    dllres = temp!(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.SocketUdpIn), obj1, mi) as Dll.SocketUdpIn;
                    dllres = temp!(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Socket]{dll.Name}", error);
                ServerMain.LogError("[Socket]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// WebSocket数据
    /// </summary>
    /// <param name="head"></param>
    public static void WebSocketGo(WebSocketMessage head)
    {
        foreach (var dll in AssemblyList.GetWebSocket())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.WebSocketMessage))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketMessage];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.WebSocketMessageIn), mi) as Dll.WebSocketMessageIn;
                    dllres = temp!(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.WebSocketMessageIn), obj1, mi) as Dll.WebSocketMessageIn;
                    dllres = temp!(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[WebSocket]{dll.Name}", error);
                ServerMain.LogError("[WebSocket]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// WebSocket链接
    /// </summary>
    /// <param name="head"></param>
    public static void WebSocketGo(WebSocketOpen head)
    {
        foreach (var dll in AssemblyList.GetWebSocket())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.WebSocketOpen))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketOpen];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.WebSocketOpenIn), mi) as Dll.WebSocketOpenIn;
                    dllres = temp!(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.WebSocketOpenIn), obj1, mi) as Dll.WebSocketOpenIn;
                    dllres = temp!(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[WebSocket]{dll.Name}", error);
                ServerMain.LogError("[WebSocket]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// WebSocket断开
    /// </summary>
    /// <param name="head"></param>
    public static void WebSocketGo(WebSocketClose head)
    {
        foreach (var dll in AssemblyList.GetWebSocket())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.WebSocketClose))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketClose];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.WebSocketCloseIn), mi) as Dll.WebSocketCloseIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.WebSocketCloseIn), obj1, mi) as Dll.WebSocketCloseIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[WebSocket]{dll.Name}", error);
                ServerMain.LogError("[WebSocket]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// 消息请求
    /// </summary>
    /// <param name="head"></param>
    public static void RobotGo(RobotMessage head)
    {
        foreach (var dll in AssemblyList.GetRobot())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.RobotMessage))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.RobotMessage];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.RobotMessageIn), mi) as Dll.RobotMessageIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.RobotMessageIn), obj1, mi) as Dll.RobotMessageIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Robot]{dll.Name}", error);
                ServerMain.LogError("[Robot]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// 机器人事件
    /// </summary>
    /// <param name="head"></param>
    public static void RobotGo(RobotEvent head)
    {
        foreach (var dll in AssemblyList.GetRobot())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.RobotEvent) || !dll.Check(head.Type))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.RobotEvent];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.RobotEventIn), mi) as Dll.RobotEventIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.RobotEventIn), obj1, mi) as Dll.RobotEventIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Robot]{dll.Name}", error);
                ServerMain.LogError("[Robot]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// 机器人发送消息后
    /// </summary>
    /// <param name="head"></param>
    public static void RobotGo(RobotSend head)
    {
        foreach (var dll in AssemblyList.GetRobot())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.RobotSend))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.RobotSend];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.RobotSendIn), mi) as Dll.RobotSendIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.RobotSendIn), obj1, mi) as Dll.RobotSendIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Robot]{dll.Name}", error);
                ServerMain.LogError("[Robot]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt加载消息
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttLoadingRetainedMessages head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTMessageLoading))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTMessageLoading];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTMessageLoadingIn), mi) as Dll.MQTTMessageLoadingIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTMessageLoadingIn), obj1, mi) as Dll.MQTTMessageLoadingIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt验证
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttConnectionValidator head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTValidator))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTValidator];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTValidatorIn), mi) as Dll.MQTTValidatorIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTValidatorIn), obj1, mi) as Dll.MQTTValidatorIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt取消订阅
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttUnsubscription head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTUnsubscription))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTUnsubscription];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTUnsubscriptionIn), mi) as Dll.MQTTUnsubscriptionIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTUnsubscriptionIn), obj1, mi) as Dll.MQTTUnsubscriptionIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt消息
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttMessage head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTMessage))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTMessage];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTMessageIn), mi) as Dll.MQTTMessageIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTMessageIn), obj1, mi) as Dll.MQTTMessageIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt订阅
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttSubscription head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTSubscription))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTSubscription];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTSubscriptionIn), mi) as Dll.MQTTSubscriptionIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTSubscriptionIn), obj1, mi) as Dll.MQTTSubscriptionIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt客户端链接
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttClientConnected head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTClientConnected))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTClientConnected];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTClientConnectedIn), mi) as Dll.MQTTClientConnectedIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTClientConnectedIn), obj1, mi) as Dll.MQTTClientConnectedIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt客户端断开链接
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttClientDisconnected head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTClientDisconnected))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTClientDisconnected];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTClientDisconnectedIn), mi) as Dll.MQTTClientDisconnectedIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTClientDisconnectedIn), obj1, mi) as Dll.MQTTClientDisconnectedIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt推送消息
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttInterceptingPublish head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTInterceptingPublish))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTInterceptingPublish];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTInterceptingPublishIn), mi) as Dll.MQTTInterceptingPublishIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTInterceptingPublishIn), obj1, mi) as Dll.MQTTInterceptingPublishIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Mqtt推送消息
    /// </summary>
    /// <param name="head"></param>
    public static void MqttGo(DllMqttRetainedMessageChanged head)
    {
        foreach (var dll in AssemblyList.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTRetainedMessageChanged))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTRetainedMessageChanged];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTRetainedMessageChangedIn), mi) as Dll.MQTTRetainedMessageChangedIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.MQTTRetainedMessageChangedIn), obj1, mi) as Dll.MQTTRetainedMessageChangedIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                Task.Run(() => ServiceOnError(e));
                LogDatabsae.PutError($"[Mqtt]{dll.Name}", error);
                ServerMain.LogError("[Mqtt]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Service任务
    /// </summary>
    /// <param name="arg">错误信息</param>
    /// <returns></returns>
    public static void ServiceOnError(Exception head)
    {
        foreach (var dll in AssemblyList.GetService())
        {
            try
            {
                if (dll.ServiceType != ServiceType.ErrorDump)
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.ServiceError];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.ServiceErrorIn), mi) as Dll.ServiceErrorIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.ServiceErrorIn), obj1, mi) as Dll.ServiceErrorIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                LogDatabsae.PutError($"[Service]{dll.Name}", error);
                ServerMain.LogError("[Service]{dll.Name}运行错误", e);
            }
        }
    }

    /// <summary>
    /// Service任务
    /// </summary>
    /// <param name="arg">构建信息</param>
    public static void ServiceOnBuild(PerBuildArg head)
    {
        foreach (var dll in AssemblyList.GetService())
        {
            try
            {
                if (dll.ServiceType != ServiceType.Builder)
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.ServicePerBuild];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.ServicePerBuildIn), mi) as Dll.ServicePerBuildIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.ServicePerBuildIn), obj1, mi) as Dll.ServicePerBuildIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                LogDatabsae.PutError($"[Service]{dll.Name}", error);
                ServerMain.LogError("[Service]{dll.Name}运行错误", e);
            }
        }
    }
    /// <summary>
    /// Service任务
    /// </summary>
    /// <param name="arg">构建信息</param>
    public static void ServiceOnBuild(PostBuildArg head)
    {
        foreach (var dll in AssemblyList.GetService())
        {
            try
            {
                if (dll.ServiceType != ServiceType.Builder)
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.ServicePostBuild];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(Dll.ServicePostBuildIn), mi) as Dll.ServicePostBuildIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(Dll.ServicePostBuildIn), obj1, mi) as Dll.ServicePostBuildIn;
                    dllres = temp(head);
                }

                if (dllres)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                }
                else
                {
                    error = e.ToString();
                }

                LogDatabsae.PutError($"[Service]{dll.Name}", error);
                ServerMain.LogError("[Service]{dll.Name}运行错误", e);
            }
        }
    }
}
