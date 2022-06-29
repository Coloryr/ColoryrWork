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
internal static class DllRun
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

            var obj1 = Activator.CreateInstance(dll.SelfType);
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
            Task.Run(() => TaskGo(e));
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
    /// <param name="Head"></param>
    public static void SocketGo(TcpSocketRequest Head)
    {
        foreach (var dll in DllStongeManager.GetSocket())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey("Netty"))
                    continue;
                MethodInfo mi = dll.MethodInfos[CodeDemo.SocketTcp];
                var obj1 = Activator.CreateInstance(dll.SelfType);
                var res = mi.Invoke(obj1, new object[1] { Head });
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Socket]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// UDP数据
    /// </summary>
    /// <param name="Head"></param>
    public static void SocketGo(UdpSocketRequest Head)
    {
        foreach (var dll in DllStongeManager.GetSocket())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey("Netty"))
                    continue;
                MethodInfo mi = dll.MethodInfos[CodeDemo.SocketUdp];
                var obj1 = Activator.CreateInstance(dll.SelfType);
                var res = mi.Invoke(obj1, new object[1] { Head });
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Socket]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// WebSocket数据
    /// </summary>
    /// <param name="Head"></param>
    public static void WebSocketGo(WebSocketMessage Head)
    {
        foreach (var dll in DllStongeManager.GetWebSocket())
        {
            try
            {
                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketMessage];
                var obj1 = Activator.CreateInstance(dll.SelfType);
                var res = mi.Invoke(obj1, new object[1] { Head });
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[WebSocket]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// WebSocket链接
    /// </summary>
    /// <param name="Head"></param>
    public static void WebSocketGo(WebSocketOpen Head)
    {
        foreach (var dll in DllStongeManager.GetWebSocket())
        {
            try
            {
                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketOpen];
                var obj1 = Activator.CreateInstance(dll.SelfType);
                var res = mi.Invoke(obj1, new object[1] { Head });
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[WebSocket]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// WebSocket断开
    /// </summary>
    /// <param name="Head"></param>
    public static void WebSocketGo(WebSocketClose Head)
    {
        foreach (var dll in DllStongeManager.GetWebSocket())
        {
            try
            {
                MethodInfo mi = dll.MethodInfos[CodeDemo.WebSocketClose];
                var obj1 = Activator.CreateInstance(dll.SelfType);
                var res = mi.Invoke(obj1, new object[1] { Head });
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[WebSocket]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// 消息请求
    /// </summary>
    /// <param name="Head"></param>
    public static void RobotGo(RobotMessage Head)
    {
        foreach (var dll in DllStongeManager.GetRobot())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey(CodeDemo.RobotMessage))
                {
                    MethodInfo mi = dll.MethodInfos[CodeDemo.RobotMessage];
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var res = mi.Invoke(obj1, new object[1] { Head });
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Robot]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// 机器人事件
    /// </summary>
    /// <param name="Head"></param>
    public static void RobotGo(RobotEvent Head)
    {
        foreach (var dll in DllStongeManager.GetRobot())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey(CodeDemo.RobotEvent))
                {
                    MethodInfo mi = dll.MethodInfos[CodeDemo.RobotEvent];
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var res = mi.Invoke(obj1, new object[1] { Head });
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Robot]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// 机器人发送消息后
    /// </summary>
    /// <param name="Head"></param>
    public static void RobotGo(RobotSend Head)
    {
        foreach (var dll in DllStongeManager.GetRobot())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey(CodeDemo.RobotSend))
                {
                    MethodInfo mi = dll.MethodInfos[CodeDemo.RobotSend];
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var res = mi.Invoke(obj1, new object[1] { Head });
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Robot]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// Mqtt加载消息
    /// </summary>
    /// <param name="Head"></param>
    public static void MqttGo(DllMqttLoadingRetainedMessages Head)
    {
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey(CodeDemo.MQTTMessageLoading))
                {
                    MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTMessageLoading];
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var res = mi.Invoke(obj1, new object[1] { Head });
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// Mqtt验证
    /// </summary>
    /// <param name="Head"></param>
    public static void MqttGo(DllMqttConnectionValidator Head)
    {
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey(CodeDemo.MQTTValidator))
                {
                    MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTValidator];
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var res = mi.Invoke(obj1, new object[1] { Head });
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// Mqtt取消订阅
    /// </summary>
    /// <param name="Head"></param>
    public static void MqttGo(DllMqttUnsubscription Head)
    {
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey(CodeDemo.MQTTUnsubscription))
                {
                    MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTUnsubscription];
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var res = mi.Invoke(obj1, new object[1] { Head });
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// Mqtt消息
    /// </summary>
    /// <param name="Head"></param>
    public static void MqttGo(DllMqttMessage Head)
    {
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey(CodeDemo.MQTTMessage))
                {
                    MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTMessage];
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var res = mi.Invoke(obj1, new object[1] { Head });
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// Mqtt订阅
    /// </summary>
    /// <param name="Head"></param>
    public static void MqttGo(DllMqttSubscription Head)
    {
        foreach (var dll in DllStongeManager.GetMqtt())
        {
            try
            {
                if (dll.MethodInfos.ContainsKey(CodeDemo.MQTTSubscription))
                {
                    MethodInfo mi = dll.MethodInfos[CodeDemo.MQTTSubscription];
                    var obj1 = Activator.CreateInstance(dll.SelfType);
                    var res = mi.Invoke(obj1, new object[1] { Head });
                    if (res is true)
                        return;
                }
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                Task.Run(() => TaskGo(e));
                DllRunLog.PutError($"[Mqtt]{dll.Name}", error);
            }
        }
    }
    /// <summary>
    /// Task任务
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TaskRes TaskGo(TaskUserArg name)
    {
        var dll = DllStongeManager.GetTask(name.Dll);
        if (dll == null)
            return null;
        try
        {
            if (!dll.MethodInfos.ContainsKey(CodeDemo.TaskRun))
                return null;
            MethodInfo mi = dll.MethodInfos[CodeDemo.TaskRun];
            var obj1 = Activator.CreateInstance(dll.SelfType);
            var res = mi.Invoke(obj1, name.Arg);
            return res as TaskRes;
        }
        catch (Exception e)
        {
            string error;
            if (e.InnerException is ErrorDump Dump)
            {
                error = Dump.data;
                ServerMain.LogError(error);
            }
            else
            {
                error = e.ToString();
                ServerMain.LogError(e);
            }
            Task.Run(() => TaskGo(e));
            DllRunLog.PutError($"[Task]{dll.Name}", error.ToString());
        }
        return null;
    }

    public static void TaskGo(Exception arg)
    {
        foreach (var item in DllStongeManager.GetTask())
        {
            try
            {
                if (!item.MethodInfos.ContainsKey(CodeDemo.TaskError))
                    continue;
                MethodInfo mi = item.MethodInfos[CodeDemo.TaskError];
                var obj1 = Activator.CreateInstance(item.SelfType);
                var res = mi.Invoke(obj1, new object[] { arg });
                if (res is true)
                    return;
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                DllRunLog.PutError($"[Task]{item.Name}", error);
            }
        }
    }

    public static void TaskGoOnStart() 
    {
        foreach (var item in DllStongeManager.GetTask())
        {
            try
            {
                if (!item.MethodInfos.ContainsKey(CodeDemo.TaskStart))
                    continue;
                MethodInfo mi = item.MethodInfos[CodeDemo.TaskStart];
                var obj1 = Activator.CreateInstance(item.SelfType);
                mi.Invoke(obj1, null);
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                DllRunLog.PutError($"[Task]{item.Name}", error);
            }
        }
    }

    public static void TaskGoOnStop()
    {
        foreach (var item in DllStongeManager.GetTask())
        {
            try
            {
                if (!item.MethodInfos.ContainsKey(CodeDemo.TaskStop))
                    continue;
                MethodInfo mi = item.MethodInfos[CodeDemo.TaskStop];
                var obj1 = Activator.CreateInstance(item.SelfType);
                mi.Invoke(obj1, null);
            }
            catch (Exception e)
            {
                string error;
                if (e.InnerException is ErrorDump Dump)
                {
                    error = Dump.data;
                    ServerMain.LogError(error);
                }
                else
                {
                    error = e.ToString();
                    ServerMain.LogError(e);
                }

                DllRunLog.PutError($"[Task]{item.Name}", error);
            }
        }
    }
}
