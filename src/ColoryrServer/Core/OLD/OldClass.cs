using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.SDK;

//2.6.0删除
public partial class NewHttpHtml : StaticDllHttpClient
{
    public NewHttpHtml(CookieContainer cookie = null,
                CancellationTokenSource cancel = null,
                Dictionary<string, string> head = null) : base(cookie, cancel, head)
    { }
}

public partial class HttpHtml : DllHttpClient
{
    public HttpHtml(CookieContainer cookie = null,
                CancellationTokenSource cancel = null,
                Dictionary<string, string> head = null) : base(cookie, cancel, head)
    { }
}

public partial class HtmlAsync : DllHttpClient
{
    public HtmlAsync(TimeSpan timeOut, CookieContainer cookie = null,
        CancellationTokenSource cancel = null,
        Dictionary<string, string> head = null)
    {
        Time = timeOut;
        Cancel = cancel ?? new();
        cookie ??= new();
        var HttpClientHandler = new HttpClientHandler()
        {
            CookieContainer = cookie
        };
        Client = new HttpClient(HttpClientHandler)
        {
            Timeout = Time
        };
        Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");
        if (head != null)
        {
            foreach (var item in head)
            {
                Client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }
    }

    public new Task<byte[]> GetByte(string url)
        => Client.GetByteArrayAsync(url, Cancel.Token);

    public new Task<string> GetString(string url)
        => Client.GetStringAsync(url, Cancel.Token);

    public new async Task<string> PostString(string url, Dictionary<string, string> arg)
        => await base.PostStringAsync(url, arg);

    public new async Task<HtmlDoc> GetWebHtml(string url)
        => new HtmlDoc(await GetString(url));

    public new async Task<HtmlDoc> PostWebHtml(string url, Dictionary<string, string> arg)
        => new HtmlDoc(await PostString(url, arg));

    public async Task<HttpResponseMessage> DoString(HttpRequestMessage httpRequest)
        => await Client.SendAsync(httpRequest, Cancel.Token);

    public new async Task<string> PutString(string url, byte[] data)
        => await base.PutStringAsync(url, data);

    public new async Task<HttpResponseMessage> PostData(string url, Dictionary<string, string> arg)
        => await Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token);

    public new async Task<HttpResponseMessage> GetData(string url)
       => await Client.GetAsync(url, Cancel.Token);
}