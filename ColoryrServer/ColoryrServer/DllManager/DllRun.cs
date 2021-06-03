using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using ColoryrServer.Utils;
using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ColoryrServer.DllManager
{
    internal class DllRun
    {
        public static HttpReturn DllGo(DllBuildSave dll, HttpRequest arg, string function)
        {
            try
            {
                if (function != null)
                {
                    if (!dll.MethodInfos.ContainsKey(function))
                    {
                        return new HttpReturn
                        {
                            Data = StreamUtils.JsonOBJ(new GetMeesage
                            {
                                Res = 90,
                                Text = "找不到方法",
                                Data = null
                            }),
                            ReCode = 404
                        };
                    }
                }
                else
                    function = CodeDemo.DllMain;

                MethodInfo mi = dll.MethodInfos[function];
                dynamic dllres = mi.Invoke(Activator.CreateInstance(dll.DllType),
                    new object[1] { arg });
                if (dllres is Dictionary<string, object>)
                {
                    return new HttpReturn
                    {
                        Data = StreamUtils.JsonOBJ(dllres)
                    };
                }
                else if (dllres is string)
                {
                    return new HttpReturn
                    {
                        Data = StreamUtils.StringOBJ(dllres)
                    };
                }
                else if (dllres is HttpResponseString)
                {
                    var dr = dllres as HttpResponseString;
                    return new HttpReturn
                    {
                        Data = StreamUtils.StringOBJ(dr.Data),
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
                        Data = StreamUtils.JsonOBJ(dr.Data),
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
                        Data1 = dr.Data,
                        Head = dr.Head,
                        ContentType = dr.ContentType,
                        ReCode = dr.ReCode,
                        Cookie = dr.SetCookie ? dr.Cookie : null
                    };
                }
                else if (dllres is HttpResponseBytes)
                {
                    var dr = dllres as HttpResponseBytes;
                    return new HttpReturn
                    {
                        Data = dr.Data,
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
                        Data = StreamUtils.JsonOBJ(new GetMeesage
                        {
                            Res = 80,
                            Text = "DLL返回错误",
                            Data = dllres
                        })
                    };
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump dump)
                {
                    return new HttpReturn
                    {
                        Data = StreamUtils.StringOBJ(dump.Get()),
                        ReCode = 200
                    };
                }
                return new HttpReturn
                {
                    Data = StreamUtils.StringOBJ(e.ToString()),
                    ReCode = 400
                };
            }
        }
        public static void IoTGo(TcpIoTRequest Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetWebSocket())
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.IoTTcp];
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
        public static void IoTGo(UdpIoTRequest Head)
        {
            try
            {
                foreach (var Dll in DllStonge.GetWebSocket())
                {
                    MethodInfo MI = Dll.MethodInfos[CodeDemo.IoTUdp];
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
