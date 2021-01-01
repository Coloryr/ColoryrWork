using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ColoryrServer.DllManager
{
    internal class DllRun
    {
        public static HttpReturn DllGo(AssemblySave Dll, HttpRequest Arg, string FunName)
        {
            try
            {
                if (FunName != null)
                {
                    if (!Dll.MethodInfos.ContainsKey(FunName))
                    {
                        return new HttpReturn
                        {
                            Data = new GetMeesage
                            {
                                res = "90",
                                text = "找不到方法",
                                data = null
                            },
                            ReCode = 404
                        };
                    }
                }
                else
                    FunName = "main";

                MethodInfo MI = Dll.MethodInfos[FunName];
                var Tran = new object[1] { Arg };
                var Assembly = Dll.Type.Assembly.CreateInstance(Dll.Type.FullName, true);
                dynamic DllReturn = MI.Invoke(Assembly, Tran);
                if (DllReturn is Dictionary<string, object>)
                {
                    return new HttpReturn
                    {
                        Data = DllReturn
                    };
                }
                else if (DllReturn is string)
                {
                    return new HttpReturn
                    {
                        Data = DllReturn,
                        IsObj = false
                    };
                }
                else if (DllReturn is HttpResponse)
                {
                    return new HttpReturn
                    {
                        Data = DllReturn.Response,
                        Head = DllReturn.Head,
                        ReCode = DllReturn.ReCode,
                        IsObj = false
                    };
                }
                else if (DllReturn is HttpResponseSession)
                {
                    return new HttpReturn
                    {
                        Data = DllReturn.Response,
                        Head = DllReturn.Head,
                        Cookie = DllReturn.Cookie,
                        ReCode = DllReturn.ReCode,
                        IsObj = false
                    };
                }
                else if (DllReturn is HttpResponseDictionary)
                {
                    return new HttpReturn
                    {
                        Data = DllReturn.Response,
                        Head = DllReturn.Head,
                        ReCode = DllReturn.ReCode
                    };
                }
                else if (DllReturn is HttpResponseDictionarySession)
                {
                    return new HttpReturn
                    {
                        Data = DllReturn.Response,
                        Head = DllReturn.Head,
                        Cookie = DllReturn.Cookie,
                        ReCode = DllReturn.ReCode
                    };
                }
                else
                {
                    return new HttpReturn
                    {
                        Data = new GetMeesage
                        {
                            res = "80",
                            text = "DLL返回错误",
                            data = DllReturn
                        }
                    };
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is VarDump Dump)
                {
                    return new HttpReturn
                    {
                        Data = Dump.Get(),
                        ReCode = 200,
                        IsObj = false
                    };
                }
                return new HttpReturn
                {
                    Data = e.ToString(),
                    ReCode = 400,
                    IsObj = false
                };
            }
        }
        public static void IoTGo(AssemblySave Dll, IoTRequest Head)
        {
            try
            {
                MethodInfo MI = Dll.MethodInfos["main"];
                var Tran = new object[1] { Head };
                var Assembly = Dll.Type.Assembly.CreateInstance(Dll.Type.FullName, true);
                MI.Invoke(Assembly, Tran);
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
                    MethodInfo MI = Dll.MethodInfos["main"];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.Type.Assembly.CreateInstance(Dll.Type.FullName, true);
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
                    MethodInfo MI = Dll.MethodInfos["open"];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.Type.Assembly.CreateInstance(Dll.Type.FullName, true);
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
                    MethodInfo MI = Dll.MethodInfos["close"];
                    var Tran = new object[1] { Head };
                    var Assembly = Dll.Type.Assembly.CreateInstance(Dll.Type.FullName, true);
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
                    if (Dll.MethodInfos.ContainsKey("main"))
                    {
                        MethodInfo MI = Dll.MethodInfos["main"];
                        var Tran = new object[1] { Head };
                        var Assembly = Dll.Type.Assembly.CreateInstance(Dll.Type.FullName, true);
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
                        var Assembly = Dll.Type.Assembly.CreateInstance(Dll.Type.FullName, true);
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
                        var Assembly = Dll.Type.Assembly.CreateInstance(Dll.Type.FullName, true);
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
