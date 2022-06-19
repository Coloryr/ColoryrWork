using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Collections.Specialized;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;
using System.Text;
using System.Xml.Linq;

namespace ColoryrServer.ASP;

internal static class HttpGet
{
    private static string[] data1 = Array.Empty<string>();
    internal static async Task RoteGetIndex(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;

        if (ASPServer.Config.UrlRotes.TryGetValue(Request.Host.Host, out var rote1))
        {
            await HttpRoute.RouteDo(Request, data1, rote1, Response);
        }
        else
        {
            Response.ContentType = ServerContentType.HTML;
            await Response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlIndex);
        }
    }

    internal static async Task GetIndex(HttpContext context)
    {
        HttpResponse Response = context.Response;
        Response.ContentType = ServerContentType.HTML;
        await Response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlIndex);
    }

    private static HttpDllRequest InitArg(HttpRequest Request)
    {
        var Temp = new Dictionary<string, dynamic>();
        if (Request.QueryString.HasValue)
        {
            var b = Request.QueryString.ToUriComponent()[1..];
            foreach (string a in b.Split('&'))
            {
                var item = a.Split("=");
                Temp.Add(item[0], item[1]);
            }
        }
        NameValueCollection collection = new();
        foreach (var item in Request.Headers)
        {
            collection.Add(item.Key, item.Value);
        }
        return new HttpDllRequest()
        {
            Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
            RowRequest = collection,
            Parameter = Temp,
            Method = Request.Method,
            ContentType = MyContentType.XFormData
        };
    }

    internal static async Task Get(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        HttpReturn httpReturn;
        var name = context.GetRouteValue("name") as string;
        int last = name.LastIndexOf('/');
        var uuid = name[..last];
        var funtion = name[last..];
        var Dll = HttpInvokeRoute.Get(uuid);
        if (Dll != null)
        {
            if (Dll.IsDll)
            {
                var arg = InitArg(Request);
                httpReturn = Dll.Invoke(arg, funtion);
            }
            else
            {
                httpReturn = Dll.Invoke(null, funtion);
            }
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            if (httpReturn.Head != null)
                foreach (var Item in httpReturn.Head)
                {
                    Response.Headers.Add(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                foreach (var item in httpReturn.Cookie)
                {
                    Response.Cookies.Append(item.Key, item.Value);
                }
            switch (httpReturn.Res)
            {
                case ResType.String:
                    await Response.WriteAsync(httpReturn.Data as string, httpReturn.Encoding);
                    break;
                case ResType.Byte:
                    var bytes = httpReturn.Data as byte[];
                    await Response.BodyWriter.WriteAsync(bytes);
                    break;
                case ResType.Json:
                    var obj1 = httpReturn.Data;
                    await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
                    break;
                case ResType.Stream:
                    var stream = httpReturn.Data as Stream;
                    if (stream == null)
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync("stream in null", httpReturn.Encoding);
                    }
                    else
                    {
                        stream.Seek(httpReturn.Pos, SeekOrigin.Begin);
                        Response.StatusCode = 206;
                        await stream.CopyToAsync(Response.Body);
                    }
                    break;
            }
        }
        else
        {
            if (ASPServer.Config.Requset.Stream)
            {
                var a = name.LastIndexOf('.');
                if (a != -1)
                {
                    string type = name[a..];
                    if (ASPServer.Config.Requset.StreamType.Contains(type))
                    {
                        NameValueCollection collection = new();
                        foreach (var item in Request.Headers)
                        {
                            collection.Add(item.Key, item.Value);
                        }

                        var res = HttpStatic.GetStream(new()
                        {
                            Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
                            RowRequest = collection
                        }, name);

                        if (res == null)
                        {
                            Response.ContentType = ServerContentType.HTML;
                            Response.StatusCode = 200;
                            await Response.BodyWriter.WriteAsync(WebBinManager.BaseDir.Html404);
                            return;
                        }

                        var stream = res.Data;
                        if (stream == null)
                        {
                            Response.StatusCode = 500;
                            await Response.WriteAsync("stream in null", Encoding.UTF8);
                        }
                        else
                        {
                            stream.Seek(res.Pos, SeekOrigin.Begin);
                            foreach (var item in res.Head)
                            {
                                Response.Headers.Add(item.Key, item.Value);
                            }
                            Response.StatusCode = 206;
                            Response.ContentType = ServerContentType.GetType(type);
                            await stream.CopyToAsync(Response.Body, 1024);
                        }

                        return;
                    }
                }
            }

            var arg = name.Split('/');
            httpReturn = HttpStatic.GetStatic(arg);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            await Response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
            return;
        }
    }

    internal static async Task RouteGet(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        var name = context.GetRouteValue("name") as string;
        if (name == null)
            return;
        var arg = name.Split('/');
        if (ASPServer.Config.UrlRotes.TryGetValue(Request.Host.Host, out var rote1))
        {
            await HttpRoute.RouteDo(Request, arg, rote1, Response, 0);
        }
        else if (ASPServer.Config.Rotes.TryGetValue(arg[0], out var rote))
        {
            await HttpRoute.RouteDo(Request, arg, rote, Response);
        }
        else
        {
            await Get(context);
        }
    }
}
