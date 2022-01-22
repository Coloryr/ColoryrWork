using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ColoryrServer.DllManager
{
    public class DllRun
    {
        public static HttpReturn DllGo(DllBuildSave dll, HttpRequest arg, string function)
        {
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

                MethodInfo mi = dll.MethodInfos[function];
                dynamic dllres = mi.Invoke(Activator.CreateInstance(dll.DllType),
                    new object[1] { arg });
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
                    return new HttpReturn
                    {
                        Data = dump1.data + "\n" + e.ToString(),
                        Res = ResType.String,
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
            try
            {
                foreach (var Dll in DllStonge.GetWebSocket())
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.SocketTcp];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    MI.Invoke(Assembly, Tran);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void SocketGo(UdpSocketRequest Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetWebSocket())
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.SocketUdp];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    MI.Invoke(Assembly, Tran);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void WebSocketGo(WebSocketMessage Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetWebSocket())
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.WebSocketMessage];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    MI.Invoke(Assembly, Tran);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void WebSocketGo(WebSocketOpen Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetWebSocket())
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.WebSocketOpen];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    MI.Invoke(Assembly, Tran);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void WebSocketGo(WebSocketClose Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetWebSocket())
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.WebSocketClose];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                    MI.Invoke(Assembly, Tran);
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void RobotGo(RobotRequest Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetRobot())
                {
                    if (Dll.MethodInfos.ContainsKey(CodeDemo.RobotMessage))
                    {
                        MethodInfo MI = Dll.MethodInfos[CodeDemo.RobotMessage];
                        var Tran = new object[1] { Head };
                        var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                        MI.Invoke(Assembly, Tran);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void RobotGo(RobotEvent Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetRobot())
                {
                    if (Dll.MethodInfos.ContainsKey("robot"))
                    {
                        MethodInfo MI = Dll.MethodInfos["robot"];
                        var Tran = new object[1] { Head };
                        var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                        MI.Invoke(Assembly, Tran);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void RobotGo(RobotAfter Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetRobot())
                {
                    if (Dll.MethodInfos.ContainsKey("after"))
                    {
                        MethodInfo MI = Dll.MethodInfos["after"];
                        var Tran = new object[1] { Head };
                        var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                        MI.Invoke(Assembly, Tran);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void MqttGo(MqttConnectionValidator Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetMqtt())
                {
                    if (Dll.MethodInfos.ContainsKey(CodeDemo.MQTTValidator))
                    {
                        MethodInfo MI = Dll.MethodInfos[CodeDemo.MQTTValidator];
                        var Tran = new object[1] { Head };
                        var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                        MI.Invoke(Assembly, Tran);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void MqttGo(MqttMessage Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetMqtt())
                {
                    if (Dll.MethodInfos.ContainsKey(CodeDemo.MQTTMessage))
                    {
                        MethodInfo MI = Dll.MethodInfos[CodeDemo.MQTTMessage];
                        var Tran = new object[1] { Head };
                        var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                        MI.Invoke(Assembly, Tran);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
        }
        public static void MqttGo(MqttSubscription Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetMqtt())
                {
                    if (Dll.MethodInfos.ContainsKey(CodeDemo.MQTTSubscription))
                    {
                        MethodInfo MI = Dll.MethodInfos[CodeDemo.MQTTSubscription];
                        var Tran = new object[1] { Head };
                        var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                        MI.Invoke(Assembly, Tran);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
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
                if (e.InnerException is VarDump Dump)
                {
                    ServerMain.LogError(Dump.Get());
                }
                else
                    ServerMain.LogError(e);
            }
            return false;
        }
    }
}
