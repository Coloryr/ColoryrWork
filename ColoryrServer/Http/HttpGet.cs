using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using Lib.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ColoryrServer.Http
{
    public class HttpGet
    {
        public static HttpReturn HttpGET(string Url, NameValueCollection Hashtable, NameValueCollection Data)
        {
            if (Url.StartsWith("//"))
            {
                Url = Url[1..];
            }
            if (Url.StartsWith(ServerMain.Config.Requset.WebAPI))
            {
                string UUID = "0";
                string FunctionName = null;
                Url = Url.Substring(ServerMain.Config.Requset.WebAPI.Length);
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

            return HttpStatic.Get(Url);
        }
    }
}
