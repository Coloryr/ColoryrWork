using ColoryrServer.DllManager;
using ColoryrServer.SDK;
using ColoryrServer.Utils;
using Lib.App;
using Lib.Build;
using Lib.Build.Object;
using Lib.Server;
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

        public static HttpReturn HttpPOST(StreamReader streamReader, string Url, NameValueCollection Hashtable, MyContentType type)
        {
            var Temp = new Dictionary<string, dynamic>();
            switch (type)
            {
                case MyContentType.Json:
                    string Str = streamReader.ReadToEnd();
                    try
                    {
                        JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                        if (Hashtable[BuildKV.BuildK] == BuildKV.BuildV)
                        {
                            var Json = obj.ToObject<BuildOBJ>();
                            var List = ServerMain.Config.User.Where(a => a.Username == Json.User);
                            if (List.Any())
                            {
                                return DllBuild.StartBuild(Json, List.First());
                            }
                            else
                            {
                                return new HttpReturn
                                {
                                    Data = StreamUtils.JsonOBJ(new ReMessage
                                    {
                                        Build = false,
                                        Message = "账户错误"
                                    })
                                };
                            }
                        }
                        else if (Hashtable[APPKV.APPK] == APPKV.APPV)
                        {
                            var Json = obj.ToObject<DownloadObj>();
                            return AppDownload.Download(Json);
                        }
                        else
                        {
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
                            Data = StreamUtils.JsonOBJ(new GetMeesage
                            {
                                Res = 123,
                                Text = "Json解析发生错误",
                                Data = e
                            })
                        };
                    }
                    break;
                case MyContentType.Form:
                    Str = streamReader.ReadToEnd();
                    foreach (string Item in Str.Split('&'))
                    {
                        if (Item.Contains("="))
                        {
                            string[] KV = Item.Split('=');
                            Temp.Add(KV[0], KV[1]);
                        }
                    }
                    break;
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
                var Http = new HttpRequest
                {
                    Cookie = HaveCookie(Hashtable),
                    Parameter = Temp,
                    RowRequest = Hashtable,
                    ContentType = type,
                    Stream = type == MyContentType.Other ? streamReader.BaseStream : null
                };
                var Data = DllRun.DllGo(Dll, Http, FunctionName);
                return Data;
            }
            return new HttpReturn
            {
                Data = StreamUtils.JsonOBJ(new GetMeesage
                {
                    Res = 666,
                    Text = "未找到DLL",
                    Data = Url
                }),
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
                var Http = new HttpRequest
                {
                    Cookie = HaveCookie(Hashtable),
                    RowRequest = Hashtable,
                    Parameter = Temp
                };
                var Data1 = DllRun.DllGo(Dll, Http, FunctionName);
                return Data1;
            }
            return new HttpReturn
            {
                Data = StreamUtils.JsonOBJ(new GetMeesage
                {
                    Res = 666,
                    Text = "未找到DLL",
                    Data = Url
                }),
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
                        HttpReturn httpReturn;
                        switch (Request.HttpMethod)
                        {
                            case "POST":
                                StreamReader Reader = new StreamReader(Context.Request.InputStream, Encoding.UTF8);
                                MyContentType type;
                                if (Context.Request.ContentType == "application/x-www-form-urlencoded")
                                {
                                    type = MyContentType.Form;
                                }
                                else if (Context.Request.ContentType == "application/json")
                                {
                                    type = MyContentType.Json;
                                }
                                else
                                {
                                    type = MyContentType.Other;
                                }
                                httpReturn = HttpProcessor.HttpPOST(Reader, Request.RawUrl, Request.Headers, type);
                                Context.Response.ContentType = httpReturn.ContentType;
                                Context.Response.ContentEncoding = httpReturn.Encoding;
                                if (httpReturn.Head != null)
                                    foreach (var Item in httpReturn.Head)
                                    {
                                        Context.Response.AddHeader(Item.Key, Item.Value);
                                    }
                                if (httpReturn.Cookie != null)
                                    Context.Response.AppendCookie(new Cookie("cs", httpReturn.Cookie));
                                if (httpReturn.Data1 == null)
                                    Context.Response.OutputStream.Write(httpReturn.Data);
                                else
                                {
                                    httpReturn.Data1.Seek(0, SeekOrigin.Begin);
                                    httpReturn.Data1.CopyTo(Context.Response.OutputStream);
                                }
                                Context.Response.OutputStream.Flush();
                                Context.Response.StatusCode = httpReturn.ReCode;
                                Context.Response.Close();
                                break;
                            case "GET":
                                httpReturn = HttpProcessor.HttpGET(Request.RawUrl, Request.Headers, Request.QueryString);
                                Context.Response.ContentType = httpReturn.ContentType;
                                Context.Response.ContentEncoding = httpReturn.Encoding;
                                if (httpReturn.Head != null)
                                    foreach (var Item in httpReturn.Head)
                                    {
                                        Context.Response.AddHeader(Item.Key, Item.Value);
                                    }
                                if (httpReturn.Cookie != null)
                                    Context.Response.AppendCookie(new Cookie("cs", httpReturn.Cookie));
                                if (httpReturn.Data1 == null)
                                    Context.Response.OutputStream.Write(httpReturn.Data);
                                else
                                {
                                    httpReturn.Data1.Seek(0, SeekOrigin.Begin);
                                    httpReturn.Data1.CopyTo(Context.Response.OutputStream);
                                }
                                Context.Response.OutputStream.Flush();
                                Context.Response.StatusCode = httpReturn.ReCode;
                                Context.Response.Close();
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
