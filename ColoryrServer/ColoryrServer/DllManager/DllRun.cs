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
        public static HttpReturn DllGo(DllBuildSave Dll, HttpRequest Arg, string FunName)
        {
            try
            {
                if (FunName != null)
                {
                    if (!Dll.MethodInfos.ContainsKey(FunName))
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
                    FunName = CodeDemo.DllMain;

                MethodInfo MI = Dll.MethodInfos[FunName];
                var Tran = new object[1] { Arg };
                var Assembly = Dll.DllType.Assembly.CreateInstance(Dll.DllType.FullName, true);
                dynamic DllReturn = MI.Invoke(Assembly, Tran);
                if (DllReturn is Dictionary<string, object>)
                {
                    return new HttpReturn
                    {
                        Data = StreamUtils.JsonOBJ(DllReturn)
                    };
                }
                else if (DllReturn is string)
                {
                    return new HttpReturn
                    {
                        Data = StreamUtils.StringOBJ(DllReturn)
                    };
                }
                else if (DllReturn is HttpResponseString)
                {
                    return new HttpReturn
                    {
                        Data = StreamUtils.StringOBJ(DllReturn.Data),
                        Head = DllReturn.Head,
                        ReCode = DllReturn.ReCode,
                        Cookie = DllReturn.SetCookie ? DllReturn.Cookie : null
                    };
                }
                else if (DllReturn is HttpResponseDictionary)
                {
                    return new HttpReturn
                    {
                        Data = StreamUtils.JsonOBJ(DllReturn.Data),
                        Head = DllReturn.Head,
                        ReCode = DllReturn.ReCode,
                        Cookie = DllReturn.SetCookie ? DllReturn.Cookie : null
                    };
                }
                else if (DllReturn is HttpResponseStream)
                {
                    return new HttpReturn
                    {
                        Data1 = DllReturn.Data,
                        Head = DllReturn.Head,
                        ReCode = DllReturn.ReCode,
                        Cookie = DllReturn.SetCookie ? DllReturn.Cookie : null
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
                            Data = DllReturn
                        })
                    };
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    return new HttpReturn
                    {
                        Data = StreamUtils.StringOBJ(Dump.Get()),
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
    }
}
