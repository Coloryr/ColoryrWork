﻿using ColoryrServer.Core;
using ColoryrServer.Core.Database;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Text.Json.Nodes;
using System.Web;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP;

internal static class HttpGet
{
    private readonly static string[] data1 = [];

    internal static Task<(MyContentType, IDictionary<string, dynamic?>)> GetBody(HttpRequest request, JsonType json)
    {
        var temp = new Dictionary<string, dynamic?>();
        if (request.QueryString.HasValue)
        {
            var b = request.QueryString.ToUriComponent()[1..];
            b = HttpUtility.UrlDecode(b, System.Text.Encoding.UTF8);
            if (b.StartsWith('{') || b.StartsWith('[') && json != JsonType.None)
            {
                if (json == JsonType.SystemJson && JsonNode.Parse(b) is { } obj)
                {
                    if (obj is JsonObject obj1)
                    {
                        foreach (var item in obj1)
                        {
                            temp.Add(item.Key, item.Value);
                        }
                    }
                    else
                    {
                        temp.Add("", obj);
                    }
                }
                else
                {
                    string str = new StreamReader(request.Body).ReadToEnd();
                    if (JToken.Parse(str) is { } obj2)
                    {
                        if (obj2 is JObject obj1)
                        {
                            foreach (var item in obj1)
                            {
                                temp.Add(item.Key, item.Value);
                            }
                        }
                        else
                        {
                            temp.Add("", obj2);
                        }
                    }
                }
            }
            else
            {
                foreach (string a in b.Split('&'))
                {
                    var item = a.Split("=");
                    temp.Add(item[0], item[1]);
                }
            }
        }

        return Task.FromResult<(MyContentType, IDictionary<string, dynamic?>)>
            ((MyContentType.XFormData, temp));
    }
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
        NameValueCollection collection = [];
        foreach (var item in request.Headers)
        {
            collection.Add(item.Key, item.Value);
        }
        return new HttpDllRequest()
        {
            Cookie = ASPHttpUtils.HaveCookie(request.Headers.Cookie),
            RowRequest = collection,
            Method = request.Method,
            ContentType = request.ContentType,
            GetBody = (type) => GetBody(request, type)
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
            CoreHttpReturn httpReturn;
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
                    response.Headers.Append(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                foreach (var item in httpReturn.Cookie)
                {
                    response.Cookies.Append(item.Key, item.Value);
                }
            switch (httpReturn.Res)
            {
                case ResType.String:
                    string str = httpReturn.Data as string ?? "";
                    await response.WriteAsync(str, httpReturn.Encoding);
                    break;
                case ResType.Byte:
                    await response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
                    break;
                case ResType.Json:
                    await response.WriteAsync(JsonUtils.ToString(httpReturn.Data), httpReturn.Encoding);
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
            await response.WriteAsync(JsonUtils.ToString(httpReturn.Data));
        }
    }
}
