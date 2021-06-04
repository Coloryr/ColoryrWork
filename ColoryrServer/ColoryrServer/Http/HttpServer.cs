using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Pipe;
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

        public static HttpReturn HttpPOST(Stream stream, long Length, string Url, NameValueCollection Hashtable, MyContentType type)
        {
            var Temp = new Dictionary<string, dynamic>();
            switch (type)
            {
                case MyContentType.Json:
                    string Str = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
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
                case MyContentType.XFormData:
                    Str = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                    foreach (string Item in Str.Split('&'))
                    {
                        if (Item.Contains("="))
                        {
                            string[] KV = Item.Split('=');
                            Temp.Add(KV[0], KV[1]);
                        }
                    }
                    break;
                case MyContentType.MFormData:
                    var parser = HttpMultipart.Parse(stream, Length);
                    if (parser == null)
                    {
                        return new HttpReturn
                        {
                            Data = StreamUtils.JsonOBJ(new GetMeesage
                            {
                                Res = 123,
                                Text = "表单解析发生错误"
                            })
                        };
                    }
                    foreach (var item in parser.Parameters)
                    {
                        Temp.Add(item.Key, item.Value);
                    }
                    foreach (var item in parser.FileContents)
                    {
                        Temp.Add(item.Key, item.Value);
                    }
                    break;
                case MyContentType.Other:
                    if (Hashtable[BuildKV.BuildK] == BuildKV.BuildV)
                    {
                        if (Hashtable[UploadKV.UploadK] != null)
                        {
                            string data = Hashtable[UploadKV.UploadK];
                            var item = Tools.ToObject<UploadObj>(data);
                            var app = CSFile.GetApp(item.UUID);
                            if (app == null)
                            {
                                return new HttpReturn
                                {
                                    Data = StreamUtils.JsonOBJ(new GetMeesage
                                    {
                                        Res = 123,
                                        Text = "UUID未找到"
                                    })
                                };
                            }
                            else
                            {
                                if (CSFile.AddFileApp(app, item, stream))
                                {
                                    return new HttpReturn
                                    {
                                        Data = StreamUtils.JsonOBJ(new GetMeesage
                                        {
                                            Res = 100,
                                            Text = "上传成功"
                                        })
                                    };
                                }
                                else
                                {
                                    return new HttpReturn
                                    {
                                        Data = StreamUtils.JsonOBJ(new GetMeesage
                                        {
                                            Res = 200,
                                            Text = "上传失败"
                                        })
                                    };
                                }
                            }
                        }
                        else
                        {
                            return new HttpReturn
                            {
                                Data = StreamUtils.JsonOBJ(new GetMeesage
                                {
                                    Res = 123,
                                    Text = "上传错误"
                                })
                            };
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
                    Stream = type == MyContentType.Other ? stream : null
                };
                var Data = DllRun.DllGo(Dll, Http, FunctionName);
                return Data;
            }
            return new HttpReturn
            {
                Data = HtmlUtils.Html404,
                ContentType = ServerContentType.HTML,
                ReCode = 404
            };
        }
        public static dynamic PipeHttpPOST(StreamReader streamReader, string Url, NameValueCollection Hashtable, MyContentType type)
        {
            var Temp = new Dictionary<string, dynamic>();
            switch (type)
            {
                case MyContentType.Json:
                    string Str = streamReader.ReadToEnd();
                    try
                    {
                        JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                        if (Hashtable[APPKV.APPK] == APPKV.APPV)
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
                case MyContentType.XFormData:
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
            return new PipeHttpData
            {
                Request = new HttpRequest
                {
                    Cookie = HaveCookie(Hashtable),
                    Parameter = Temp,
                    RowRequest = Hashtable,
                    ContentType = type,
                    Stream = type == MyContentType.Other ? streamReader.BaseStream : null
                },
                FunctionName = FunctionName,
                UUID = UUID,
                UID = Guid.NewGuid().ToString().Replace("-", ""),
                Url = Url
            };
        }

        public static HttpReturn HttpGET(string Url, NameValueCollection Hashtable, NameValueCollection Data)
        {
            string UUID = "0";
            string FunctionName = null;
            if (Url.StartsWith("//"))
            {
                Url = Url[1..];
            }
            if (Url.StartsWith(ServerMain.Config.Requset.Web))
            {
                Url = Url.Substring(ServerMain.Config.Requset.Web.Length);
                var temp = HtmlUtils.GetFile(Url);
                if (temp != null)
                {
                    string type = ServerContentType.HTML;
                    if (Url.EndsWith(".jpg") || Url.EndsWith(".jpge"))
                    {
                        type = ServerContentType.JPEG;
                    }
                    else if (Url.ToLower().EndsWith(".png"))
                    {
                        type = ServerContentType.PNG;
                    }
                    else if (Url.ToLower().EndsWith(".json"))
                    {
                        type = ServerContentType.JSON;
                    }
                    else if (Url.ToLower().EndsWith(".xml"))
                    {
                        type = ServerContentType.XML;
                    }
                    else if (Url.ToLower().EndsWith(".mp3"))
                    {
                        type = ServerContentType.MP3;
                    }
                    else if (Url.ToLower().EndsWith(".mp4"))
                    {
                        type = ServerContentType.MP4;
                    }
                    else if (Url.ToLower().EndsWith(".gif"))
                    {
                        type = ServerContentType.GIF;
                    }
                    return new HttpReturn
                    {
                        Data = temp,
                        ContentType = type
                    };
                }
            }
            else if (Url.StartsWith(ServerMain.Config.Requset.Back))
            {
                Url = Url.Substring(ServerMain.Config.Requset.Back.Length);
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
                        Parameter = Temp,
                        ContentType = MyContentType.XFormData
                    };
                    var Data1 = DllRun.DllGo(Dll, Http, FunctionName);
                    return Data1;
                }
            }
            return new HttpReturn
            {
                Data = HtmlUtils.Html404,
                ContentType = ServerContentType.HTML,
                ReCode = 404
            };
        }
        public static PipeHttpData PipeHttpGET(string Url, NameValueCollection Hashtable, NameValueCollection Data)
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
            var Temp = new Dictionary<string, dynamic>();
            foreach (string a in Data.AllKeys)
            {
                Temp.Add(a, Data.Get(a));
            }
            return new PipeHttpData
            {
                Request = new HttpRequest
                {
                    Cookie = HaveCookie(Hashtable),
                    Parameter = Temp,
                    RowRequest = Hashtable,
                    ContentType = MyContentType.XFormData
                },
                FunctionName = FunctionName,
                UUID = UUID,
                UID = Guid.NewGuid().ToString().Replace("-", ""),
                Url = Url
            };
        }
    }
    class HttpServer
    {
        private static Thread[] Workers;                             // 工作线程组
        public static ConcurrentBag<HttpListenerContext> Queue;   // 请求队列
        public static List<HttpListener> Listeners;
        public static bool IsActive { get; set; }//是否在运行

        private static void Init()
        {
            ServerMain.LogOut($"Http服务器正在启动");

            Workers = new Thread[ServerMain.Config.HttpThreadNumber];
            Listeners = new();
            Queue = new();
            IsActive = false;

            foreach (var item in ServerMain.Config.Http)
            {
                var Listener = new HttpListener();
                Listener.Prefixes.Add("http://" + item.IP + ":" +
                item.Port + "/");
                ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Listener.TimeoutManager.EntityBody = TimeSpan.FromSeconds(30);
                    Listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(30);
                }
                Listeners.Add(Listener);
            }
        }

        public static void Start()
        {
            try
            {
                Init();
                // 启动工作线程
                for (int i = 0; i < Workers.Length; i++)
                {
                    Workers[i] = new Thread(HttpWork.Worker);
                    Workers[i].Start();
                }
                IsActive = true;
                for (int a = 0; a < Listeners.Count; a++)
                {
                    var item = Listeners[a];
                    item.Start();
                    item.BeginGetContext(ContextReady, a);
                }
                ServerMain.LogOut("Http服务器已启动");
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        public static void StartPipe()
        {
            try
            {
                Init();
                // 启动工作线程
                for (int i = 0; i < Workers.Length; i++)
                {
                    Workers[i] = new Thread(HttpWork.PipeWorker);
                    Workers[i].Start();
                }
                IsActive = true;
                for (int a = 0; a < Listeners.Count; a++)
                {
                    var item = Listeners[a];
                    item.Start();
                    item.BeginGetContext(ContextReady, a);
                }
                ServerMain.LogOut("Http服务器已启动");
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        private static void ContextReady(IAsyncResult ar)
        {
            if (IsActive)
            {
                var http = Listeners[(int)ar.AsyncState];
                http.BeginGetContext(ContextReady, ar.AsyncState);
                Queue.Add(http.EndGetContext(ar));
            }
        }

        public static void Stop()
        {
            if (IsActive)
            {
                ServerMain.LogOut("Http服务器正在关闭");
                IsActive = false;
                foreach (var item in Listeners)
                {
                    item.Stop();
                }
                ServerMain.LogOut("Http服务器已关闭");
            }
        }
    }

    class HttpWork
    {
        // 处理一个任务
        public static void Worker()
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
                        HttpListenerResponse Response = Context.Response;
                        HttpReturn httpReturn;
                        switch (Request.HttpMethod)
                        {
                            case "POST":
                                Stream stream = Context.Request.InputStream;
                                MyContentType type;
                                if (Context.Request.ContentType.StartsWith(ServerContentType.POSTXFORM))
                                {
                                    type = MyContentType.XFormData;
                                }
                                else if (Context.Request.ContentType.StartsWith(ServerContentType.POSTFORMDATA))
                                {
                                    type = MyContentType.MFormData;
                                }
                                else if (Context.Request.ContentType.StartsWith(ServerContentType.JSON))
                                {
                                    type = MyContentType.Json;
                                }
                                else
                                {
                                    type = MyContentType.Other;
                                }
                                httpReturn = HttpProcessor.HttpPOST(stream, Request.ContentLength64, Request.RawUrl, Request.Headers, type);
                                Response.ContentType = httpReturn.ContentType;
                                Response.ContentEncoding = httpReturn.Encoding;
                                if (httpReturn.Head != null)
                                    foreach (var Item in httpReturn.Head)
                                    {
                                        Response.AddHeader(Item.Key, Item.Value);
                                    }
                                if (httpReturn.Cookie != null)
                                    Response.AppendCookie(new Cookie("cs", httpReturn.Cookie));
                                if (httpReturn.Data1 == null)
                                    Response.OutputStream.Write(httpReturn.Data);
                                else
                                {
                                    httpReturn.Data1.Seek(0, SeekOrigin.Begin);
                                    httpReturn.Data1.CopyTo(Response.OutputStream);
                                }
                                Response.OutputStream.Flush();
                                Response.StatusCode = httpReturn.ReCode;
                                Response.Close();
                                break;
                            case "GET":
                                httpReturn = HttpProcessor.HttpGET(Request.RawUrl, Request.Headers, Request.QueryString);
                                Response.ContentType = httpReturn.ContentType;
                                Response.ContentEncoding = httpReturn.Encoding;
                                if (httpReturn.Head != null)
                                    foreach (var Item in httpReturn.Head)
                                    {
                                        Response.AddHeader(Item.Key, Item.Value);
                                    }
                                if (httpReturn.Cookie != null)
                                    Response.AppendCookie(new Cookie("cs", httpReturn.Cookie));
                                if (httpReturn.Data1 == null)
                                    Response.OutputStream.Write(httpReturn.Data);
                                else
                                {
                                    httpReturn.Data1.Seek(0, SeekOrigin.Begin);
                                    httpReturn.Data1.CopyTo(Response.OutputStream);
                                }
                                Response.OutputStream.Flush();
                                Response.StatusCode = httpReturn.ReCode;
                                Response.Close();
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
        public static void PipeWorker()
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
                        HttpListenerResponse Response = Context.Response;
                        switch (Request.HttpMethod)
                        {
                            case "POST":
                                StreamReader Reader = new(Context.Request.InputStream, Encoding.UTF8);
                                MyContentType type;
                                if (Context.Request.ContentType is ServerContentType.POSTXFORM)
                                {
                                    type = MyContentType.XFormData;
                                }
                                else if (Context.Request.ContentType is ServerContentType.POSTFORMDATA)
                                {
                                    type = MyContentType.MFormData;
                                }
                                else if (Context.Request.ContentType is ServerContentType.JSON)
                                {
                                    type = MyContentType.Json;
                                }
                                else
                                {
                                    Response.OutputStream.Write(Encoding.UTF8.GetBytes($"不支持{Context.Request.ContentType}"));
                                    Response.OutputStream.Flush();
                                    Response.StatusCode = 400;
                                    Response.Close();
                                    break;
                                }
                                dynamic httpReturn = HttpProcessor.PipeHttpPOST(Reader, Request.RawUrl, Request.Headers, type);
                                if (httpReturn is HttpReturn)
                                {
                                    Response.ContentType = httpReturn.ContentType;
                                    Response.ContentEncoding = httpReturn.Encoding;
                                    Response.OutputStream.Write(httpReturn.Data);
                                    Response.OutputStream.Flush();
                                    Response.StatusCode = httpReturn.ReCode;
                                    Response.Close();
                                    break;
                                }
                                else
                                {
                                    PipeClient.Http(httpReturn, Response);
                                }
                                break;
                            case "GET":
                                httpReturn = HttpProcessor.PipeHttpGET(Request.RawUrl, Request.Headers, Request.QueryString);
                                PipeClient.Http(httpReturn, Response);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                    }
                }
                Thread.Sleep(TimeSpan.FromTicks(100));
            }
        }
    }
}
