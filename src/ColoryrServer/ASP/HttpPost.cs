using ColoryrServer.Core;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Server;
using HttpMultipartParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Text;

namespace ColoryrServer.ASP;

internal static class HttpPost
{
    private static async Task<HttpDllRequest?> InitArg(HttpRequest Request)
    {
        MyContentType type = MyContentType.XFormData;
        var temp = new Dictionary<string, dynamic>();
        if (Request.ContentType != null)
        {
            if (Request.ContentType is ServerContentType.POSTXFORM)
            {
                type = MyContentType.XFormData;
                foreach (var item in Request.Form)
                {
                    temp.Add(item.Key, item.Value);
                }
                foreach (var item in Request.Form.Files)
                {
                    temp.Add(item.Name, item);
                }
            }
            else if (Request.ContentType.StartsWith(ServerContentType.POSTFORMDATA))
            {
                try
                {
                    var parser = await MultipartFormDataParser.ParseAsync(Request.Body);
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
                    ServerMain.LogError(e);
                    return null;
                }
            }
            else if (Request.ContentType.StartsWith(ServerContentType.JSON))
            {
                MemoryStream stream = new();
                await Request.Body.CopyToAsync(stream);
                var Str = Encoding.UTF8.GetString(stream.ToArray());
                JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                foreach (var item in obj)
                {
                    temp.Add(item.Key, item.Value);
                }
                type = MyContentType.Json;
            }
            else
            {
                type = MyContentType.Other;
            }
        }
        NameValueCollection collection = new();
        foreach (var item in Request.Headers)
        {
            collection.Add(item.Key, item.Value);
        }

        return new()
        {
            Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
            Parameter = temp,
            RowRequest = collection,
            ContentType = type,
            Method = Request.Method,
            Stream = type == MyContentType.Other ? Request.Body : null
        };
    }
    internal static async Task Post(HttpContext context)
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
                var arg = await InitArg(request);
                if (arg == null)
                {
                    var obj1 = new GetMeesage
                    {
                        Res = 123,
                        Text = "表单解析发生错误，请检查数据"
                    };
                    response.StatusCode = 500;
                    await response.WriteAsync(JsonConvert.SerializeObject(obj1));
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
                    var obj1 = httpReturn.Data;
                    await response.WriteAsync(JsonConvert.SerializeObject(obj1));
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

    internal static async Task RoutePost(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        var name = context.GetRouteValue("name") as string;
        if (name == null)
            return;
        var arg = name.Split('/');
        if (ASPServer.Config.UrlRotes.TryGetValue(request.Host.Host, out var rote1))
        {
            await HttpRoute.RouteDo(request, arg, rote1, response, 0);
        }
        else if (ASPServer.Config.Rotes.TryGetValue(arg[0], out var rote))
        {
            await HttpRoute.RouteDo(request, arg, rote, response);
        }
        else
        {
            await Post(context);
        }
    }
}
