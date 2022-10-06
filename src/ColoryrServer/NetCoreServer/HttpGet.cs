using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using NetCoreServer;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace ColoryrServer.NetCoreServer;

internal class HttpGet
{
    private static HttpDllRequest InitArg(HttpRequest request)
    {
        var temp = new Dictionary<string, dynamic>();
        string url = request.Url;
        int index = url.IndexOf('?');
        NameValueCollection collection = new();
        if (index != -1)
        {
            index++;
            string queryString = url[index..];
            var b = Uri.UnescapeDataString(queryString);
            foreach (string a in b.Split('&'))
            {
                var item = a.Split("=");
                temp.Add(item[0], item[1]);
            }
        }

        for (int i = 0; i < request.Headers; i++)
        {
            var a = request.Header(i);
            collection.Add(a.Item1, a.Item2);
        }
        Dictionary<string, List<string>> cookie = new();
        for (int i = 0; i < request.Cookies; i++)
        {
            List<string> list;
            var item = request.Cookie(i);
            var key = item.Item1;
            var value = item.Item2;
            if (cookie.ContainsKey(key))
            {
                list = cookie[key];
            }
            else
            {
                list = new();
                cookie.Add(key, list);
            }
            list.Add(value);
        }

        return new HttpDllRequest()
        {
            Cookie = cookie,
            RowRequest = collection,
            Parameter = temp,
            Method = request.Method,
            ContentType = MyContentType.XFormData
        };
    }

    public static void Get(IHttp session, HttpRequest request)
    {
        if (request.Url == "/")
        {
            session.Response.MakeGetResponse(WebBinManager.BaseDir.HtmlIndex, ServerContentType.HTML);
        }
        else
        {
            string url = Uri.UnescapeDataString(request.Url);
            HttpReturn httpReturn;
            int index = url.IndexOf('?');
            string name;
            if (index != -1)
            {
                name = url[..index];
            }
            else
            {
                name = url;
            }
            var route = HttpUtils.GetUUID(name, out string funtion);
            if (route != null)
            {
                if (route.IsDll)
                {
                    var arg = InitArg(request);
                    httpReturn = route.Invoke(arg, funtion);
                }
                else
                {
                    httpReturn = route.Invoke(null, funtion);
                }
                session.Response.SetBegin(httpReturn.ReCode);
                session.Response.SetHeader("Content-Type", httpReturn.ContentType);
                if (httpReturn.Head != null)
                    foreach (var Item in httpReturn.Head)
                    {
                        session.Response.SetHeader(Item.Key, Item.Value);
                    }
                if (httpReturn.Cookie != null)
                    foreach (var item in httpReturn.Cookie)
                    {
                        session.Response.SetCookie(item.Key, item.Value);
                    }
                switch (httpReturn.Res)
                {
                    case ResType.String:
                        session.Response.SetBody(httpReturn.Encoding.GetBytes(httpReturn.Data as string));
                        break;
                    case ResType.Byte:
                        var bytes = httpReturn.Data as byte[];
                        session.Response.SetBody(bytes);
                        break;
                    case ResType.Json:
                        session.Response.SetBody(JsonConvert.SerializeObject(httpReturn.Data));
                        break;
                        //case ResType.Stream:
                        //    var stream = httpReturn.Data as Stream;
                        //    if (stream == null)
                        //    {
                        //        session.Response.MakeErrorResponse(500, "stream in null");
                        //    }
                        //    else
                        //    {
                        //        stream.Seek(httpReturn.Pos, SeekOrigin.Begin);
                        //        session.Response.MakeOkResponse(206);
                        //        Buffer buffer = new();
                        //        session.Response.SetBody(stream);
                        //    }
                        //    break;
                }
            }
            else
            {
                HttpUtils.Static(name, request, session.Response);
            }
        }
        session.SendResponseAsync(session.Response);
    }
}
