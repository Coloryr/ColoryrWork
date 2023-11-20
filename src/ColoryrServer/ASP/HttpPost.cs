using ColoryrServer.Core;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using HttpMultipartParser;
using ColoryrWork.Lib.Build;
using System.Collections.Specialized;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ColoryrServer.ASP;

internal static class HttpPost
{
    internal static async Task<(MyContentType, IDictionary<string, dynamic?>)> GetBody(HttpRequest request, JsonType json)
    {
        MyContentType type = MyContentType.Error;
        var temp = new Dictionary<string, dynamic?>();
        if (request.ContentType != null)
        {
            if (request.ContentType is ServerContentType.POSTXFORM)
            {
                type = MyContentType.XFormData;
                foreach (var item in request.Form)
                {
                    temp.Add(item.Key, item.Value);
                }
                foreach (var item in request.Form.Files)
                {
                    temp.Add(item.Name, item);
                }
            }
            else if (request.ContentType.StartsWith(ServerContentType.POSTFORMDATA))
            {
                try
                {
                    var parser = await MultipartFormDataParser.ParseAsync(request.Body);
                    foreach (var item in parser.Parameters)
                    {
                        temp.Add(item.Name, item.Data);
                    }
                    foreach (var item in parser.Files)
                    {
                        temp.Add(item.Name, new HttpMultipartFile()
                        {
                            Data = item.Data,
                            FileName = item.FileName,
                            ContentType = item.ContentType,
                            ContentDisposition = item.ContentDisposition
                        });
                    }
                    type = MyContentType.MFormData;
                }
                catch (Exception e)
                {
                    ServerMain.LogError("Post处理出错", e);
                    return (type, temp);
                }
            }
            else if (request.ContentType.StartsWith(ServerContentType.JSON))
            {
                if (json == JsonType.SystemJson)
                {
                    var obj = await JsonNode.ParseAsync(request.Body);
                    if (obj is { })
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
                        type = MyContentType.Json;
                    }
                }
                else if(json == JsonType.NewtonsoftJson)
                {
                    string str = new StreamReader(request.Body).ReadToEnd();
                    var obj = JToken.Parse(str);
                    if (obj is { })
                    {
                        if (obj is JObject obj1)
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
                        type = MyContentType.Json;
                    }
                }
            }
            else
            {
                type = MyContentType.Other;
            }
        }

        return (type, temp);
    }

    private static HttpDllRequest? InitArg(HttpRequest request)
    {
        NameValueCollection collection = [];
        foreach (var item in request.Headers)
        {
            collection.Add(item.Key, item.Value);
        }

        return new()
        {
            Cookie = ASPHttpUtils.HaveCookie(request.Headers.Cookie),
            RowRequest = collection,
            ContentType = request.ContentType,
            Method = request.Method,
            Stream = request.Body,
            GetBody = (type) => GetBody(request, type)
        };
    }

    internal static async Task Post(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        if (ServerMain.Config.FixMode)
        {
            response.ContentType = ServerContentType.HTML;
            await response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlFixMode);
            return;
        }
        CoreHttpReturn httpReturn;
        var name = context.GetRouteValue("name") as string;
        var route = HttpUtils.GetUUID(name, out string funtion);
        if (route != null)
        {
            if (route.IsDll)
            {
                var arg = InitArg(request);
                if (arg == null)
                {
                    response.StatusCode = 500;
                    await response.WriteAsync(HttpReturnSave.FromError.Data as string);
                    return;
                }
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
                    await response.WriteAsync(httpReturn.Data as string, httpReturn.Encoding);
                    break;
                case ResType.Byte:
                    await response.BodyWriter.WriteAsync(httpReturn.Data as byte[]);
                    break;
                case ResType.Json:
                    var obj1 = httpReturn.Data;
                    await response.WriteAsync(JsonUtils.ToString(obj1));
                    break;
                case ResType.Stream:
                    if (httpReturn.Data is not Stream stream)
                    {
                        response.StatusCode = 500;
                        await response.WriteAsync(HttpReturnSave.StreamError.Data as string);
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

    internal static async Task RoutePost(HttpContext context)
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
        {
            return;
        }
        var arg = name.Split('/');
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
            await Post(context);
        }
    }
}
