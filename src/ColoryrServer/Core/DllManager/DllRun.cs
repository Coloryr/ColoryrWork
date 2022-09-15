using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager;
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
    internal static HttpReturn DllGo(DllAssembly dll, HttpDllRequest arg, string function)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(function))
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

            MethodInfo mi = dll.MethodInfos[function];

            object dllres;
            if (mi.IsStatic)
            {
                var temp = Delegate.CreateDelegate(typeof(DllIN), mi) as DllIN;
                dllres = temp(arg);
            }
            else
            {
                var obj1 = Activator.CreateInstance(dll.SelfType);
                var temp = Delegate.CreateDelegate(typeof(DllIN), obj1, mi) as DllIN;
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
        catch (Exception e)
        {
            Task.Run(() => ServiceOnError(e));
            if (e.InnerException is VarDump dump)
            {
                if (dll.Debug)
                    return new HttpReturn
                    {
                        Data = dump.Get(),
                        Res = ResType.String,
                        ReCode = 200
                    };
                else
                    return new HttpReturn
                    {
                        Data = ErrorObj2,
                        Res = ResType.Json,
                        ReCode = 500
                    };
            }
            else if (e.InnerException is ErrorDump dump1)
            {
                string error = dump1.data + "\n" + e.ToString();
                DllRunLog.PutError($"[Dll]{dll.Name}", error);
                if (dll.Debug)
                    return new HttpReturn
                    {
                        Data = error,
                        Res = ResType.String,
                        ReCode = 500
                    };
                else
                    return new HttpReturn
                    {
                        Data = ErrorObj,
                        Res = ResType.Json,
                        ReCode = 500
                    };
            }
            else
            {
                string error = e.ToString();
                DllRunLog.PutError($"[Dll]{dll.Name}", error);
                if (dll.Debug)
                    return new HttpReturn
                    {
                        Data = error,
                        Res = ResType.String,
                        ReCode = 500
                    };
                else
                    return new HttpReturn
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
        foreach (var dll in DllStongeManager.GetSocket())
        {
            try
            {
                if (dll.Netty || !dll.MethodInfos.ContainsKey(CodeDemo.SocketTcp))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.SocketTcp];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(SocketTcpIn), mi) as SocketTcpIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(SocketTcpIn), obj1, mi) as SocketTcpIn;
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
                DllRunLog.PutError($"[Socket]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetSocket())
        {
            try
            {
                if (dll.Netty || !dll.MethodInfos.ContainsKey(CodeDemo.SocketUdp))
                    continue;
                MethodInfo mi = dll.MethodInfos[CodeDemo.SocketUdp];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(SocketUdpIn), mi) as SocketUdpIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(SocketUdpIn), obj1, mi) as SocketUdpIn;
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
                DllRunLog.PutError($"[Socket]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetWebSocket())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.WebSocketMessage))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketMessage];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(WebSocketMessageIn), mi) as WebSocketMessageIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(WebSocketMessageIn), obj1, mi) as WebSocketMessageIn;
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
                DllRunLog.PutError($"[WebSocket]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetWebSocket())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.WebSocketOpen))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketOpen];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(WebSocketOpenIn), mi) as WebSocketOpenIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(WebSocketOpenIn), obj1, mi) as WebSocketOpenIn;
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
                DllRunLog.PutError($"[WebSocket]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetWebSocket())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.WebSocketClose))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketClose];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(WebSocketCloseIn), mi) as WebSocketCloseIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(WebSocketCloseIn), obj1, mi) as WebSocketCloseIn;
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
                DllRunLog.PutError($"[WebSocket]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetRobot())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.RobotMessage))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.RobotMessage];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(RobotMessageIn), mi) as RobotMessageIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(RobotMessageIn), obj1, mi) as RobotMessageIn;
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
                DllRunLog.PutError($"[Robot]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetRobot())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.RobotEvent) || !dll.Check(head.Type))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.RobotEvent];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(RobotEventIn), mi) as RobotEventIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(RobotEventIn), obj1, mi) as RobotEventIn;
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
                DllRunLog.PutError($"[Robot]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetRobot())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.RobotSend))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.RobotSend];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(RobotSendIn), mi) as RobotSendIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(RobotSendIn), obj1, mi) as RobotSendIn;
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
                DllRunLog.PutError($"[Robot]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTMessageLoading))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTMessageLoading];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTMessageLoadingIn), mi) as MQTTMessageLoadingIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTMessageLoadingIn), obj1, mi) as MQTTMessageLoadingIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTValidator))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTValidator];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTValidatorIn), mi) as MQTTValidatorIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTValidatorIn), obj1, mi) as MQTTValidatorIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTUnsubscription))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTUnsubscription];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTUnsubscriptionIn), mi) as MQTTUnsubscriptionIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTUnsubscriptionIn), obj1, mi) as MQTTUnsubscriptionIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTMessage))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTMessage];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTMessageIn), mi) as MQTTMessageIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTMessageIn), obj1, mi) as MQTTMessageIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTSubscription))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTSubscription];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTSubscriptionIn), mi) as MQTTSubscriptionIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTSubscriptionIn), obj1, mi) as MQTTSubscriptionIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTClientConnected))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTClientConnected];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTClientConnectedIn), mi) as MQTTClientConnectedIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTClientConnectedIn), obj1, mi) as MQTTClientConnectedIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTClientDisconnected))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTClientDisconnected];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTClientDisconnectedIn), mi) as MQTTClientDisconnectedIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTClientDisconnectedIn), obj1, mi) as MQTTClientDisconnectedIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTInterceptingPublish))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTInterceptingPublish];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTInterceptingPublishIn), mi) as MQTTInterceptingPublishIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTInterceptingPublishIn), obj1, mi) as MQTTInterceptingPublishIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (!dll.MethodInfos.ContainsKey(CodeDemo.MQTTRetainedMessageChanged))
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTRetainedMessageChanged];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(MQTTRetainedMessageChangedIn), mi) as MQTTRetainedMessageChangedIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(MQTTRetainedMessageChangedIn), obj1, mi) as MQTTRetainedMessageChangedIn;
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
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetService())
        {
            try
            {
                if (dll.ServiceType != ServiceType.ErrorDump)
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.ServiceError];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(ServiceErrorIn), mi) as ServiceErrorIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(ServiceErrorIn), obj1, mi) as ServiceErrorIn;
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

                DllRunLog.PutError($"[Service]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetService())
        {
            try
            {
                if (dll.ServiceType != ServiceType.Builder)
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.ServicePerBuild];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(ServicePerBuildIn), mi) as ServicePerBuildIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(ServicePerBuildIn), obj1, mi) as ServicePerBuildIn;
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

                DllRunLog.PutError($"[Service]{dll.Name}", error);
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
        foreach (var dll in DllStongeManager.GetService())
        {
            try
            {
                if (dll.ServiceType != ServiceType.Builder)
                    continue;

                MethodInfo mi = dll.MethodInfos[CodeDemo.ServicePostBuild];
                bool dllres;
                if (mi.IsStatic)
                {
                    var temp = Delegate.CreateDelegate(typeof(ServicePostBuildIn), mi) as ServicePostBuildIn;
                    dllres = temp(head);
                }
                else
                {
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var temp = Delegate.CreateDelegate(typeof(ServicePostBuildIn), obj1, mi) as ServicePostBuildIn;
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

                DllRunLog.PutError($"[Service]{dll.Name}", error);
                ServerMain.LogError("[Service]{dll.Name}运行错误", e);
            }
        }
    }
}
