using ColoryrServer.Html;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.SDK;

public class NewHttpHtml
{
    public CancellationTokenSource Cancel;
    public CookieContainer Cookies { get; private set; }

    private ExHttpClient Client;
    private Dictionary<string, string> Head;

    public NewHttpHtml(CookieContainer Cookie = null,
                CancellationTokenSource Cancel = null,
                Dictionary<string, string> Head = null)
    {
        this.Cancel = Cancel ?? new();
        this.Cookies = Cookie ?? new();
        this.Head = Head ?? new();
        Client = HttpClientUtils.Get();
    }

    ~NewHttpHtml()
    {
        HttpClientUtils.Close(Client);
    }
    /// <summary>
    /// 获取byte
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>byte</returns>
    public byte[] GetByte(string url)
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Get, url);
        foreach (var item in Head)
        {
            requestMessage.Headers.Add(item.Key, item.Value);
        }
        var data = Cookies.GetCookieHeader(requestMessage.RequestUri);
        requestMessage.Headers.TryAddWithoutValidation("Cookie", data);
        var result = Client.Client.SendAsync(requestMessage, Cancel.Token).Result;
        var uri = result.RequestMessage.RequestUri;
        var cookie = result.Headers.GetValues("Set-Cookie");
        foreach (var item in cookie)
        {
            Cookies.SetCookies(uri, item);
        }
        return result.Content.ReadAsByteArrayAsync(Cancel.Token).Result;
    }
    /// <summary>
    /// 获取字符串
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>字符串</returns>
    public string GetString(string url)
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Get, url);
        foreach (var item in Head)
        {
            requestMessage.Headers.Add(item.Key, item.Value);
        }
        var data = Cookies.GetCookieHeader(requestMessage.RequestUri);
        requestMessage.Headers.TryAddWithoutValidation("Cookie", data);
        var result = Client.Client.SendAsync(requestMessage, Cancel.Token).Result;
        var uri = result.RequestMessage.RequestUri;
        var cookie = result.Headers.GetValues("Set-Cookie");
        foreach (var item in cookie)
        {
            Cookies.SetCookies(uri, item);
        }
        return result.Content.ReadAsStringAsync(Cancel.Token).Result;
    }
    /// <summary>
    /// 发送表单获取字符串
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">参数</param>
    /// <returns>字符串</returns>
    public string PostString(string url, Dictionary<string, string> arg)
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Post, url);
        foreach (var item in Head)
        {
            requestMessage.Headers.Add(item.Key, item.Value);
        }
        var data = Cookies.GetCookieHeader(requestMessage.RequestUri);
        requestMessage.Headers.TryAddWithoutValidation("Cookie", data);
        requestMessage.Content = new FormUrlEncodedContent(arg);
        var result = Client.Client.SendAsync(requestMessage, Cancel.Token).Result;
        var uri = result.RequestMessage.RequestUri;
        var cookie = result.Headers.GetValues("Set-Cookie");
        foreach (var item in cookie)
        {
            Cookies.SetCookies(uri, item);
        }
        return result.Content.ReadAsStringAsync(Cancel.Token).Result;
    }
    /// <summary>
    /// 获取解析后的html
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>解析后的html</returns>
    public HtmlDoc GetWebHtml(string url)
        => new(GetString(url));
    /// <summary>
    /// 发送表单获取解析后的html
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">数据</param>
    /// <returns></returns>
    public HtmlDoc PostWebHtml(string url, Dictionary<string, string> arg)
        => new(PostString(url, arg));
    /// <summary>
    /// 进行一次http请求
    /// </summary>
    /// <param name="httpRequest">请求结构</param>
    /// <returns>返回结构</returns>
    public HttpResponseMessage Send(HttpRequestMessage httpRequest)
        => Client.Client.SendAsync(httpRequest, Cancel.Token).Result;
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="data">数据</param>
    /// <returns>返回的字符串</returns>
    public string PutString(string url, byte[] data)
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Put, url);
        foreach (var item in Head)
        {
            requestMessage.Headers.Add(item.Key, item.Value);
        }
        var data1 = Cookies.GetCookieHeader(requestMessage.RequestUri);
        requestMessage.Headers.TryAddWithoutValidation("Cookie", data1);
        requestMessage.Content = new ByteArrayContent(data);
        var result = Client.Client.SendAsync(requestMessage, Cancel.Token).Result;
        var uri = result.RequestMessage.RequestUri;
        var cookie = result.Headers.GetValues("Set-Cookie");
        foreach (var item in cookie)
        {
            Cookies.SetCookies(uri, item);
        }
        return result.Content.ReadAsStringAsync(Cancel.Token).Result;
    }
    /// <summary>
    /// 发送表单数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">数据</param>
    /// <returns>返回结构</returns>
    public HttpResponseMessage PostData(string url, Dictionary<string, string> arg)
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Post, url);
        foreach (var item in Head)
        {
            requestMessage.Headers.Add(item.Key, item.Value);
        }
        var data1 = Cookies.GetCookieHeader(requestMessage.RequestUri);
        requestMessage.Headers.TryAddWithoutValidation("Cookie", data1);
        requestMessage.Content = new FormUrlEncodedContent(arg);
        var result = Client.Client.SendAsync(requestMessage, Cancel.Token).Result;
        var uri = result.RequestMessage.RequestUri;
        var cookie = result.Headers.GetValues("Set-Cookie");
        foreach (var item in cookie)
        {
            Cookies.SetCookies(uri, item);
        }
        return result;
    }
    /// <summary>
    /// Get获取数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>返回结构</returns>
    public HttpResponseMessage GetData(string url)
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Get, url);
        foreach (var item in Head)
        {
            requestMessage.Headers.Add(item.Key, item.Value);
        }
        var data1 = Cookies.GetCookieHeader(requestMessage.RequestUri);
        requestMessage.Headers.TryAddWithoutValidation("Cookie", data1);
        var result = Client.Client.SendAsync(requestMessage, Cancel.Token).Result;
        var uri = result.RequestMessage.RequestUri;
        var cookie = result.Headers.GetValues("Set-Cookie");
        foreach (var item in cookie)
        {
            Cookies.SetCookies(uri, item);
        }
        return result;
    }
}
public class HttpHtml
{
    public CancellationTokenSource Cancel;
    private HttpClient Client;

    public WebProxy proxy { get; init; }

    public HttpHtml(CookieContainer Cookie = null,
                CancellationTokenSource Cancel = null,
                Dictionary<string, string> Head = null)
    {
        this.Cancel = Cancel ?? new();
        Cookie ??= new();
        var HttpClientHandler = new HttpClientHandler()
        {
            CookieContainer = Cookie,
            Proxy = proxy
        };
        Client = new HttpClient(HttpClientHandler)
        {
            Timeout = TimeSpan.FromSeconds(5)
        };
        Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");
        if (Head != null)
        {
            foreach (var item in Head)
            {
                Client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }
    }

    ~HttpHtml()
    {
        Client.CancelPendingRequests();
        Client.Dispose();
    }
    /// <summary>
    /// 获取byte
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>byte</returns>
    public byte[] GetByte(string url)
        => Client.GetByteArrayAsync(url, Cancel.Token).Result;
    /// <summary>
    /// 获取字符串
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>字符串</returns>
    public string GetString(string url)
        => Client.GetStringAsync(url, Cancel.Token).Result;
    /// <summary>
    /// 发送表单获取字符串
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">参数</param>
    /// <returns>字符串</returns>
    public string PostString(string url, Dictionary<string, string> arg)
        => Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token).Result.Content.ReadAsStringAsync().Result;
    /// <summary>
    /// 获取解析后的html
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>解析后的html</returns>
    public HtmlDoc GetWebHtml(string url)
        => new(GetString(url));
    /// <summary>
    /// 发送表单获取解析后的html
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">数据</param>
    /// <returns></returns>
    public HtmlDoc PostWebHtml(string url, Dictionary<string, string> arg)
        => new(PostString(url, arg));
    /// <summary>
    /// 进行一次http请求
    /// </summary>
    /// <param name="httpRequest">请求结构</param>
    /// <returns>返回结构</returns>
    public HttpResponseMessage Send(HttpRequestMessage httpRequest)
        => Client.SendAsync(httpRequest, Cancel.Token).Result;
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="data">数据</param>
    /// <returns>返回的字符串</returns>
    public string PutString(string url, byte[] data)
        => Client.PutAsync(url, new ByteArrayContent(data), Cancel.Token).Result.Content.ReadAsStringAsync().Result;
    /// <summary>
    /// 发送表单数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">数据</param>
    /// <returns>返回结构</returns>
    public HttpResponseMessage PostData(string url, Dictionary<string, string> arg)
        => Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token).Result;
    /// <summary>
    /// Get获取数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>返回结构</returns>
    public HttpResponseMessage GetData(string url)
        => Client.GetAsync(url, Cancel.Token).Result;
}

public class HtmlAsync
{
    private HttpClient Http;
    public CancellationTokenSource Cancel;
    public CookieContainer Cookie;
    /// <summary>
    /// http爬虫异步
    /// </summary>
    /// <param name="timeOut">请求超时时间</param>
    /// <param name="Cookie">Cookie</param>
    /// <param name="Cancel">取消请求</param>
    /// <param name="Head">请求头</param>
    public HtmlAsync(TimeSpan timeOut, CookieContainer Cookie = null,
        CancellationTokenSource Cancel = null,
        Dictionary<string, string> Head = null)
    {
        this.Cookie = Cookie ?? new();
        this.Cancel = Cancel ?? new();
        var Handler = new HttpClientHandler()
        {
            CookieContainer = Cookie
        };
        Http = new(Handler)
        {
            Timeout = timeOut
        };
        Http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");
        if (Head != null)
        {
            foreach (var Item in Head)
            {
                Http.DefaultRequestHeaders.Add(Item.Key, Item.Value);
            }
        }
    }
    /// <summary>
    /// http爬虫异步
    /// </summary>
    public HtmlAsync() : this(TimeSpan.FromSeconds(10))
    {
    }

    ~HtmlAsync()
        => Http.Dispose();
    /// <summary>
    /// 获取byte
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>byte</returns>
    public Task<byte[]> GetByte(string url)
    => Http.GetByteArrayAsync(url, Cancel.Token);
    /// <summary>
    /// 获取字符串
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>字符串</returns>
    public Task<string> GetString(string url)
    => Http.GetStringAsync(url, Cancel.Token);
    /// <summary>
    /// 发送表单获取字符串
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">参数</param>
    /// <returns>字符串</returns>
    public async Task<string> PostString(string url, Dictionary<string, string> arg)
    {
        var temp = await Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token);
        return await temp.Content.ReadAsStringAsync();
    }
    /// <summary>
    /// 获取解析后的html
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>解析后的html</returns>
    public async Task<HtmlDoc> GetWebHtml(string url)
        => new HtmlDoc(await GetString(url));
    /// <summary>
    /// 发送表单获取解析后的html
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">数据</param>
    /// <returns></returns>
    public async Task<HtmlDoc> PostWebHtml(string url, Dictionary<string, string> arg)
        => new HtmlDoc(await PostString(url, arg));
    /// <summary>
    /// 进行一次http请求
    /// </summary>
    /// <param name="httpRequest">请求结构</param>
    /// <returns>返回结构</returns>
    public async Task<HttpResponseMessage> DoString(HttpRequestMessage httpRequest)
        => await Http.SendAsync(httpRequest, Cancel.Token);
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="data">数据</param>
    /// <returns>返回的字符串</returns>
    public async Task<string> PutString(string url, byte[] data)
    {
        var temp = await Http.PutAsync(url, new ByteArrayContent(data), Cancel.Token);
        return await temp.Content.ReadAsStringAsync();
    }
    /// <summary>
    /// 发送表单数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">数据</param>
    /// <returns>返回结构</returns>
    public async Task<HttpResponseMessage> PostData(string url, Dictionary<string, string> arg)
        => await Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token);
    /// <summary>
    /// Get获取数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>返回结构</returns>
    public async Task<HttpResponseMessage> GetData(string url)
        => await Http.GetAsync(url, Cancel.Token);
}
public class HtmlDoc
{
    public HtmlDocument html = new();
    /// <summary>
    /// Html解析
    /// </summary>
    /// <param name="data">Html字符串</param>
    public HtmlDoc(string data)
       => html.LoadHtml(data);
    /// <summary>
    /// Html解析
    /// </summary>
    /// <param name="data">Html节点</param>
    public HtmlDoc(HtmlNode data)
       => html.LoadHtml(data.InnerHtml);
    /// <summary>
    /// 选择节点
    /// </summary>
    /// <param name="NodeName">标签名</param>
    /// <param name="ClassName">class名字</param>
    /// <returns></returns>
    public List<HtmlNode> Select(string NodeName, string ClassName)
    {
        try
        {
            return html.DocumentNode.Descendants(NodeName)
                    .Where(x => x.Attributes.Contains("class")
                    && x.Attributes["class"].Value == ClassName)
                    .ToList();
        }
        catch
        {
            throw new ErrorDump("选择" + NodeName + "_Class:" + ClassName + "出错");
        }
    }
    /// <summary>
    /// 选择节点
    /// </summary>
    /// <param name="NodeName">标签名</param>
    /// <param name="AttributesName">元素名字</param>
    /// <param name="Attributes">元素</param>
    /// <returns></returns>
    public List<HtmlNode> Select(string NodeName, string AttributesName, string Attributes)
    {
        try
        {
            return html.DocumentNode.Descendants(NodeName)
                    .Where(x => x.Attributes.Contains(AttributesName)
                    && x.Attributes[AttributesName].Value == Attributes)
                    .ToList();
        }
        catch (Exception e)
        {
            throw new ErrorDump("选择" + NodeName + "_" + NodeName + ":" + Attributes + "出错", e);
        }
    }
    /// <summary>
    /// 选择节点
    /// </summary>
    /// <param name="NodeName">标签名</param>
    /// <returns></returns>
    public List<HtmlNode> Select(string NodeName)
    {
        try
        {
            return html.DocumentNode.Descendants(NodeName).ToList();
        }
        catch (Exception e)
        {
            throw new ErrorDump("选择" + NodeName + "出错", e);
        }
    }
}
