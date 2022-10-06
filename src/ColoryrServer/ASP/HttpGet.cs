using ColoryrServer.Core;
using ColoryrServer.Core.FileSystem.Database;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using Newtonsoft.Json;
using System.Collections.Specialized;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP;

internal static class HttpGet
{
    private readonly static string[] data1 = Array.Empty<string>();
    internal static async Task RoteGetIndex(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        if (ServerMain.Config.FixMode)
        {
            response.ContentType = ServerContentType.HTML;
            await response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlFixMode);
        }
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
        if (ServerMain.Config.FixMode)
        {
            Response.ContentType = ServerContentType.HTML;
            await Response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlFixMode);
        }
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
        if (ServerMain.Config.FixMode)
        {
            response.ContentType = ServerContentType.HTML;
            await response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlFixMode);
            return;
        }

        var name = context.GetRouteValue("name") as string;
        var route = HttpUtils.GetUUID(name, out string funtion);
        if (route != null)
        {
            HttpReturn httpReturn;
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
                    await response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
                    break;
                case ResType.Json:
                    await response.WriteAsync(JsonConvert.SerializeObject(httpReturn.Data), httpReturn.Encoding);
                    break;
                case ResType.Stream:
                    if (httpReturn.Data is not Stream stream)
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
        if (ServerMain.Config.FixMode)
        {
            response.ContentType = ServerContentType.HTML;
            await response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlFixMode);
            return;
        }
        if (context.GetRouteValue("name") is not string name)
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
            LogDatabsae.PutError("Server Route", e.ToString());
            var httpReturn = HttpReturnSave.ResError;
            response.ContentType = httpReturn.ContentType;
            response.StatusCode = httpReturn.ReCode;
            await response.WriteAsync(JsonConvert.SerializeObject(httpReturn.Data));
        }
    }
}
