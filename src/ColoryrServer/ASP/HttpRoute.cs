using ColoryrServer.Core;
using ColoryrWork.Lib.Build.Object;
using Microsoft.Extensions.Primitives;

namespace ColoryrServer.ASP;

internal static class HttpRoute
{
    internal static async Task RouteDo(HttpRequest request, string[] arg,
        RouteConfigObj route, HttpResponse response, int start = 1)
    {
        var client = ASPServer.GetHttpClient();
        var message = new HttpRequestMessage()
        {
            Method = new HttpMethod(request.Method)
        };
        string url = "";
        if (arg.Length >= start)
        {
            for (int a = start; a < arg.Length; a++)
            {
                url += $"/{arg[a]}";
            }
        }

        message.RequestUri = new Uri($"{route.Url}{url}");
        if (request.Method is "GET")
            message.Content = new StringContent("");
        else
            message.Content = new StreamContent(request.Body);

        foreach (var item in request.Headers)
        {
            try
            {
                if (item.Key.StartsWith("Content"))
                {
                    message.Content.Headers.Add(item.Key, item.Value as IEnumerable<string>);
                }
                else
                    message.Headers.Add(item.Key, item.Value as IEnumerable<string>);
            }
            catch
            {

            }
        }
        foreach (var item in route.Heads)
        {
            message.Headers.Add(item.Key, item.Value);
        }

        if (url.EndsWith(".php"))
        {
            var res = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
            response.StatusCode = (int)res.StatusCode;
            if (res.Content.Headers.ContentType != null)
                response.ContentType = res.Content.Headers.ContentType.ToString();

            foreach (var item in res.Headers)
            {
                if (item.Key is "Transfer-Encoding")
                    continue;
                StringValues values = new(item.Value.ToArray());
                response.Headers.Append(item.Key, values);
            }

            await res.Content.CopyToAsync(response.Body);
        }
        else
        {
            var res = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

            response.StatusCode = (int)res.StatusCode;
            if (res.Content == null)
            {
                ServerMain.LogWarn("Content is null");
                return;
            }
            if (res.Content.Headers == null)
            {
                ServerMain.LogWarn("Headers is null");
                return;
            }
            if (res.Content.Headers.ContentType != null)
            {
                response.ContentType = res.Content.Headers.ContentType.ToString();
            }

            foreach (var item in res.Headers)
            {
                StringValues values = new(item.Value.ToArray());
                response.Headers.Append(item.Key, values);
            }
            await res.Content.ReadAsStream().CopyToAsync(response.Body);
        }
    }
}
