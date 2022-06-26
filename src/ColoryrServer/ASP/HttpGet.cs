using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using Newtonsoft.Json;
using System.Collections.Specialized;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP;

internal static class HttpGet
{
    private static string[] data1 = Array.Empty<string>();
    internal static async Task RoteGetIndex(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;

        if (ASPServer.Config.UrlRoutes.TryGetValue(request.Host.Host, out var rote1))
        {
            await HttpRoute.RouteDo(request, data1, rote1, response);
        }
        else
        {
            response.ContentType = ServerContentType.HTML;
            await response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlIndex);
        }
    }

    internal static async Task GetIndex(HttpContext context)
    {
        HttpResponse Response = context.Response;
        Response.ContentType = ServerContentType.HTML;
        await Response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlIndex);
    }

    private static HttpDllRequest InitArg(HttpRequest request)
    {
        var temp = new Dictionary<string, dynamic>();
        if (request.QueryString.HasValue)
        {
            var b = request.QueryString.ToUriComponent()[1..];
            foreach (string a in b.Split('&'))
            {
                var item = a.Split("=");
                temp.Add(item[0], item[1]);
            }
        }
        NameValueCollection collection = new();
        foreach (var item in request.Headers)
        {
            collection.Add(item.Key, item.Value);
        }
        return new HttpDllRequest()
        {
            Cookie = ASPHttpUtils.HaveCookie(request.Headers.Cookie),
            RowRequest = collection,
            Parameter = temp,
            Method = request.Method,
            ContentType = MyContentType.XFormData
        };
    }

    internal static async Task Get(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        HttpReturn httpReturn;
        var name = context.GetRouteValue("name") as string;
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
            response.ContentType = httpReturn.ContentType;
            response.StatusCode = httpReturn.ReCode;
            if (httpReturn.Head != null)
                foreach (var Item in httpReturn.Head)
                {
                    response.Headers.Add(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                foreach (var item in httpReturn.Cookie)
                {
                    response.Cookies.Append(item.Key, item.Value);
                }
            switch (httpReturn.Res)
            {
                case ResType.String:
                    await response.WriteAsync(httpReturn.Data as string, httpReturn.Encoding);
                    break;
                case ResType.Byte:
                    var bytes = httpReturn.Data as byte[];
                    await response.BodyWriter.WriteAsync(bytes);
                    break;
                case ResType.Json:
                    await response.WriteAsync(JsonConvert.SerializeObject(httpReturn.Data));
                    break;
                case ResType.Stream:
                    var stream = httpReturn.Data as Stream;
                    if (stream == null)
                    {
                        response.StatusCode = 500;
                        await response.WriteAsync("stream in null", httpReturn.Encoding);
                    }
                    else
                    {
                        stream.Seek(httpReturn.Pos, SeekOrigin.Begin);
                        response.StatusCode = 206;
                        await stream.CopyToAsync(response.Body);
                    }
                    break;
            }
        }
        else
        {
            await HttpUtils.Static(name, request, response);
        }
    }

    internal static async Task RouteGet(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        var name = context.GetRouteValue("name") as string;
        if (name == null)
            return;
        var arg = name.Split('/');
        try
        {
            if (ASPServer.Config.UrlRoutes.TryGetValue(request.Host.Host, out var rote1))
            {
                await HttpRoute.RouteDo(request, arg, rote1, response, 0);
            }
            else if (ASPServer.Config.Routes.TryGetValue(arg[0], out var rote))
            {
                await HttpRoute.RouteDo(request, arg, rote, response);
            }
            else
            {
                await Get(context);
            }
        }
        catch (Exception e)
        {
            DllRunLog.PutError("Server Route", e.ToString());
            var httpReturn = HttpReturnSave.ResError;
            response.ContentType = httpReturn.ContentType;
            response.StatusCode = httpReturn.ReCode;
            await response.WriteAsync(JsonConvert.SerializeObject(httpReturn.Data));
        }
    }
}
