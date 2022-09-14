using ColoryrServer.Core;
using ColoryrServer.Core.BuilderPost;
using ColoryrServer.Core.FileSystem.Web;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Server;
using HttpMultipartParser;
using NetCoreServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace ColoryrServer.NetCoreServer;

internal class HttpPost
{
    private static HttpDllRequest InitArg(HttpRequest request, Dictionary<string, string> headers)
    {
        MyContentType type = MyContentType.XFormData;
        var temp = new Dictionary<string, dynamic>();
        string? contentType;
        if (!headers.TryGetValue("Content-Type", out contentType))
        {
            contentType = null;
        }
        if (contentType != null)
        {
            if (contentType is ServerContentType.POSTXFORM)
            {
                type = MyContentType.XFormData;

                foreach (string item in request.Body.Split('&'))
                {
                    if (item.Contains('='))
                    {
                        string[] KV = item.Split('=');
                        temp.Add(KV[0], KV[1]);
                    }
                }
            }
            else if (contentType.StartsWith(ServerContentType.POSTFORMDATA))
            {
                try
                {
                    var parser = MultipartFormDataParser.Parse(
                        new MemoryStream(request.BodyBytes));
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
            else if (contentType.StartsWith(ServerContentType.JSON))
            {
                JObject obj = JObject.Parse(Function.GetSrings(request.Body, "{"));
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
        foreach (var item in headers)
        {
            collection.Add(item.Key, item.Value);
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

        return new()
        {
            Cookie = cookie,
            Parameter = temp,
            RowRequest = collection,
            ContentType = type,
            Method = request.Method,
            Data = request.Body,
            Data1 = request.BodyBytes
        };
    }

    public static void Post(IHttp session, HttpRequest request)
    {
        Dictionary<string, string> headers = new();
        for (int a = 0; a < request.Headers; a++)
        {
            var item = request.Header(a);
            headers.Add(item.Item1, item.Item2);
        }
        if (request.Url == "/")
        {
            if (headers.TryGetValue(BuildKV.BuildK, out var item))
            {
                var obj1 = PostDo.StartBuild(request.BodyBytes, item);
                session.Response.MakeGetResponse(JsonConvert.SerializeObject(obj1));
            }
            else
            {
                session.Response.MakeGetResponse(WebBinManager.BaseDir.HtmlIndex, ServerContentType.HTML);
            }
        }
        else
        {
            HttpReturn httpReturn;
            string url = Uri.UnescapeDataString(request.Url);
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
                    var arg = InitArg(request, headers);
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
