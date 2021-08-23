using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Pipe;
using ColoryrServer.SDK;
using Lib.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ColoryrServer.Http
{
    class HttpGet
    {
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
                if (Url.Length == 0)
                {
                    return new HttpReturn
                    {
                        Data = HtmlUtils.Html404,
                        ContentType = ServerContentType.HTML,
                        ReCode = 200
                    };
                }
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
                        Cookie = HttpUtils.HaveCookie(Hashtable),
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
                ReCode = 200
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
                    Cookie = HttpUtils.HaveCookie(Hashtable),
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
}
