using ColoryrServer.Core;
using ColoryrServer.Core.Http;
using NetCoreServer;

namespace ColoryrServer.NetCoreServer;

internal class HttpUtils
{
    public static RouteObj? GetUUID(string name, out string funtion)
    {
        funtion = "";
        if (ServerMain.Config.FixMode)
        {
            return HttpReturnSave.Fix;
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            funtion = "";
            return null;
        }
        name = name[1..];
        var args = name.Split('/');
        string temp = "";
        for (int a = 0; a < args.Length; a++)
        {
            temp += args[a];
            var route = HttpInvokeRoute.Get(temp);
            if (route != null)
            {
                if (route.IsReload)
                {
                    return HttpReturnSave.Reload;
                }
                for (a++; a < args.Length; a++)
                {
                    funtion += args[a] + "/";
                }
                if (funtion.Length > 0)
                {
                    funtion = funtion[..^1];
                }
                return route;
            }
            temp += "/";
        }

        return null;
    }

    public static void Static(string name, HttpRequest request, HttpResponse response)
    {
        //if (ASPServer.Config.Requset.Stream)
        //{
        //    var a = name.LastIndexOf('.');
        //    if (a != -1)
        //    {
        //        string type = name[a..];
        //        if (ASPServer.Config.Requset.StreamType.Contains(type))
        //        {
        //            NameValueCollection collection = new();
        //            foreach (var item in request.Headers)
        //            {
        //                collection.Add(item.Key, item.Value);
        //            }

        //            var res = HttpStatic.GetStream(new()
        //            {
        //                Cookie = ASPHttpUtils.HaveCookie(request.Headers.Cookie),
        //                RowRequest = collection
        //            }, name);

        //            if (res == null)
        //            {
        //                response.ContentType = ServerContentType.HTML;
        //                response.StatusCode = 200;
        //                await response.BodyWriter.WriteAsync(WebBinManager.BaseDir.Html404);
        //                return;
        //            }

        //            var stream = res.Data;
        //            if (stream == null)
        //            {
        //                response.StatusCode = 500;
        //                await response.WriteAsync("stream in null", Encoding.UTF8);
        //            }
        //            else
        //            {
        //                stream.Seek(res.Pos, SeekOrigin.Begin);
        //                foreach (var item in res.Head)
        //                {
        //                    response.Headers.Add(item.Key, item.Value);
        //                }
        //                response.StatusCode = 206;
        //                response.ContentType = ServerContentType.GetType(type);
        //                await stream.CopyToAsync(response.Body, 1024);
        //            }

        //            return;
        //        }
        //    }
        //}

        var arg = name.Split('/');
        var httpReturn = HttpStatic.GetStatic(arg);
        response.MakeOkResponse(httpReturn.ReCode);
        response.SetContentType(httpReturn.ContentType);
        response.SetBody(httpReturn.Data as byte[]);
    }
}
