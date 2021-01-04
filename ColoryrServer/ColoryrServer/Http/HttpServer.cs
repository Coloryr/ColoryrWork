using ColoryrServer.DllManager;
using ColoryrServer.SDK;
using Lib.Build;
using Lib.Build.Object;
using Lib.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Http
{
    internal class HttpProcessor
    {
        public static string HaveCookie(NameValueCollection hashtable)
        {
            if (hashtable.HasKeys())
            {
                string Temp = hashtable["Cookie"];
                if (Temp == null)
                    return null;
                string[] Cookies = Temp.Split(';');
                foreach (var Item in Cookies)
                {
                    var temp = Item.Replace(" ", "");
                    if (temp.StartsWith("cs="))
                    {
                        return temp.Replace("cs=", "");
                    }
                }
            }
            return null;
        }
        public static HttpReturn HttpPOST(string Str, string Url, NameValueCollection Hashtable)
        {
            bool isJson = false;
            var Temp = new Dictionary<string, dynamic>();
            if (Str.StartsWith("{"))
            {
                try
                {
                    JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                    if (Hashtable[BuildKV.BuildK] == BuildKV.BuildV)
                    {
                        var Json = obj.ToObject<BuildOBJ>();
                        var List = ServerMain.Config.User.Where(a => a.Username == Json.User);
                        if (List.Any())
                        {
                            return DllBuild.HttpBuild(Json, List.First());
                        }
                        else
                        {
                            return new HttpReturn
                            {
                                Data = new ReMessage
                                {
                                    Build = false,
                                    Message = "账户错误"
                                }
                            };
                        }
                    }
                    else
                    {
                        isJson = true;
                        foreach (var item in obj)
                        {
                            Temp.Add(item.Key, item.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                    return new HttpReturn
                    {
                        IsObj = false,
                        Data = new Dictionary<string, dynamic>()
                        {
                            { "res", 123 },
                            { "text", "Json解析发生错误" },
                            { "data", e }
                        }
                    };
                }
            }
            string UUID = "0";
            string FunctionName = null;
            int thr = Url.IndexOf('?', 0);
            if (Function.Constr(Url, '/') >= 2)
            {
                int tow = Url.IndexOf('/', 2);
                if (thr == -1)
                {
                    UUID = Function.GetSrings(Url, "/", "/").Remove(0, 1);
                    FunctionName = Url[tow..].Remove(0, 1);
                }
                else if (tow < thr)
                {
                    UUID = Function.GetSrings(Url, "/", "/").Remove(0, 1);
                    FunctionName = Url[tow..thr];
                }
                else
                {
                    UUID = Function.GetSrings(Url, "/", "?").Remove(0, 1);
                }
            }
            else
            {
                if (thr != -1)
                    UUID = Function.GetSrings(Url, "/", "?").Remove(0, 1);
                else
                    UUID = Function.GetSrings(Url, "/").Remove(0, 1);
            }

            var Dll = DllStonge.GetDll(UUID);
            if (Dll != null)
            {
                if (!isJson)
                {
                    foreach (string Item in Str.Split('&'))
                    {
                        if (Item.Contains("="))
                        {
                            string[] KV = Item.Split('=');
                            Temp.Add(KV[0], KV[1]);
                        }
                    }
                }
                var Http = new HttpRequest(Temp, Hashtable, HaveCookie(Hashtable));
                var Data = DllRun.DllGo(Dll, Http, FunctionName);
                return Data;
            }
            return new HttpReturn
            {
                Data = new GetMeesage
                {
                    res = "666",
                    text = "未找到DLL",
                    data = Url
                },
                ReCode = 404
            };
        }

        public static HttpReturn HttpGET(string Url, NameValueCollection Hashtable, NameValueCollection Data)
        {
            string UUID = "0";
            string FunctionName = null;
            if (Function.Constr(Url, '/') >= 2)
            {
                int tow = Url.IndexOf('/', 2);
                int thr = Url.IndexOf('?', 2);
                if (thr == -1)
                {
                    UUID = Function.GetSrings(Url, "/", "/").Remove(0, 1);
                    FunctionName = Url[tow..].Remove(0, 1);
                }
                else if (tow < thr)
                {
                    UUID = Function.GetSrings(Url, "/", "/").Remove(0, 1);
                    FunctionName = Url[tow..thr];
                }
                else
                {
                    UUID = Function.GetSrings(Url, "/", "?").Remove(0, 1);
                }
            }
            else
            {
                UUID = Function.GetSrings(Url, "/", "?").Remove(0, 1);
            }
            var Dll = DllStonge.GetDll(UUID);
            if (Dll != null)
            {
                var Temp = new Dictionary<string, dynamic>();
                foreach (string a in Data.AllKeys)
                {
                    Temp.Add(a, Data.Get(a));
                }
                var Http = new HttpRequest(Temp, Hashtable, HaveCookie(Hashtable));
                var Data1 = DllRun.DllGo(Dll, Http, FunctionName);
                return Data1;
            }
            return new HttpReturn
            {
                Data = new GetMeesage
                {
                    res = "666",
                    text = "未找到DLL",
                    data = Url
                },
                ReCode = 404
            };
        }
    }
    class HttpServer
    {
        public static HttpListener Listener;//http服务器
        private static Thread[] Workers;                             // 工作线程组
        public static ConcurrentBag<HttpListenerContext> Queue;   // 请求队列
        public static bool IsActive { get; set; }//是否在运行

        public static void Start()
        {
            Task.Run(() =>
            {
                try
                {
                    ServerMain.LogOut("Http服务器正在启动");
                    Listener = new HttpListener();
                    Workers = new Thread[200];
                    Queue = new ConcurrentBag<HttpListenerContext>();
                    IsActive = false;

                    Listener.Prefixes.Add("http://" + ServerMain.Config.Http.IP + ":" +
                        ServerMain.Config.Http.Port + "/");
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Listener.TimeoutManager.EntityBody = TimeSpan.FromSeconds(30);
                        Listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(30);
                    }
                    Listener.Start();

                    // 启动工作线程
                    for (int i = 0; i < Workers.Length; i++)
                    {
                        Workers[i] = new Thread(new HttpWork().Worker);
                        Workers[i].Start();
                    }
                    IsActive = true;
                    Listener.BeginGetContext(ContextReady, null);
                    ServerMain.LogOut("Http服务器已启动");
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            });
        }

        private static void ContextReady(IAsyncResult ar)
        {
            if (IsActive)
            {
                Listener.BeginGetContext(ContextReady, null);
                Queue.Add(Listener.EndGetContext(ar));
            }
        }
        
        public static void Stop()
        {
            if (IsActive)
            {
                ServerMain.LogOut("Http服务器正在关闭");
                IsActive = false;
                Listener.Stop();
                ServerMain.LogOut("Http服务器已关闭");
            }
        }
    }

    class HttpWork
    {
        // 处理一个任务
        public void Worker()
        {
            while (!HttpServer.IsActive)
            {
                Thread.Sleep(200);
            }
            while (HttpServer.IsActive)
            {
                if (HttpServer.Queue.TryTake(out HttpListenerContext Context))
                {
                    try
                    {
                        HttpListenerRequest Request = Context.Request;
                        switch (Request.HttpMethod)
                        {
                            case "POST":
                                {
                                    StreamReader Reader = new StreamReader(Context.Request.InputStream, Encoding.UTF8);
                                    Context.Response.ContentType = "application/json;charset=UTF-8";
                                    Context.Response.ContentEncoding = Encoding.UTF8;

                                    var Temp = HttpProcessor.HttpPOST(Reader.ReadToEnd(), Request.RawUrl, Request.Headers);

                                    if (Temp.Head != null)
                                        foreach (var Item in Temp.Head)
                                        {
                                            Context.Response.AddHeader(Item.Key, Item.Value);
                                        }
                                    if (Temp.Cookie != null)
                                        Context.Response.AppendCookie(new Cookie("cs", Temp.Cookie));
                                    string Message;
                                    if (Temp.IsObj)
                                    {
                                        Message = JsonConvert.SerializeObject(Temp.Data);
                                    }
                                    else
                                    {
                                        Message = (string)Temp.Data;
                                    }
                                    var data1 = Encoding.UTF8.GetBytes(Message);
                                    using (Stream stream = Context.Response.OutputStream)
                                        stream.Write(data1, 0, data1.Length);
                                    Context.Response.StatusCode = Temp.ReCode;
                                    Context.Response.Close();
                                }
                                break;
                            case "GET":
                                {
                                    Context.Response.ContentType = "application/json;charset=UTF-8";
                                    Context.Response.ContentEncoding = Encoding.UTF8;

                                    var Temp = HttpProcessor.HttpGET(Request.RawUrl, Request.Headers, Request.QueryString);

                                    if (Temp.Head != null)
                                        foreach (var Item in Temp.Head)
                                        {
                                            Context.Response.AddHeader(Item.Key, Item.Value);
                                        }
                                    if (Temp.Cookie != null)
                                        Context.Response.AppendCookie(new Cookie("cs", Temp.Cookie));
                                    string Message;
                                    if (Temp.IsObj)
                                    {
                                        Message = JsonConvert.SerializeObject(Temp.Data);
                                    }
                                    else
                                    {
                                        Message = (string)Temp.Data;
                                    }
                                    var data1 = Encoding.UTF8.GetBytes(Message);
                                    using (Stream stream = Context.Response.OutputStream)
                                        stream.Write(data1, 0, data1.Length);
                                    Context.Response.StatusCode = Temp.ReCode;
                                    Context.Response.Close();
                                }
                                break;
                        }
                    }
                    catch(Exception e)
                    {
                        ServerMain.LogError(e);
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
