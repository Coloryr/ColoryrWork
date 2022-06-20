using ColoryrServer.Core;
using Microsoft.Extensions.Primitives;

namespace ColoryrServer.ASP;

internal static class HttpRoute
{
    internal static async Task RouteDo(HttpRequest Request, string[] arg, Rote rote, HttpResponse Response, int start = 1)
    {
        HttpClient ProxyRequest = ASPServer.Clients.GetOne();
        HttpRequestMessage message = new();
        message.Method = new HttpMethod(Request.Method);
        string url = "";
        if (arg.Length >= start)
        {
            for (int a = start; a < arg.Length; a++)
            {
                url += $"/{arg[a]}";
            }
        }

        message.RequestUri = new Uri($"{rote.Url}{url}");
        if (Request.Method is "GET")
            message.Content = new StringContent("");
        else
            message.Content = new StreamContent(Request.Body);

        foreach (var item in Request.Headers)
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
        foreach (var item in rote.Heads)
        {
            message.Headers.Add(item.Key, item.Value);
        }

        if (url.EndsWith(".php"))
        {
            var res = await ProxyRequest.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
            Response.StatusCode = (int)res.StatusCode;
            if (res.Content.Headers.ContentType != null)
                Response.ContentType = res.Content.Headers.ContentType.ToString();

            foreach (var item in res.Headers)
            {
                if (item.Key is "Transfer-Encoding")
                    continue;
                StringValues values = new(item.Value.ToArray());
                Response.Headers.Add(item.Key, values);
            }

            await res.Content.CopyToAsync(Response.Body);
        }
        else
        {
            var res = await ProxyRequest.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

            Response.StatusCode = (int)res.StatusCode;
            if (res.Content == null)
            {
                ServerMain.LogError("Content is null");
                return;
            }
            if (res.Content.Headers == null)
            {
                ServerMain.LogError("Headers is null");
                return;
            }
            if (res.Content.Headers.ContentType != null)
            {
                Response.ContentType = res.Content.Headers.ContentType.ToString();
            }

            foreach (var item in res.Headers)
            {
                StringValues values = new(item.Value.ToArray());
                Response.Headers.Add(item.Key, values);
            }
            await HttpClientUtils.CopyToAsync(res.Content.ReadAsStream(), Response.Body, CancellationToken.None);
        }
    }
}
