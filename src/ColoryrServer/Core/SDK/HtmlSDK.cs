using ColoryrServer.Core.Html;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.SDK;

public partial class StaticDllHttpClient : IDisposable
{
    private readonly ExHttpClient Client;
    private readonly Dictionary<string, string> Head;

    public CancellationTokenSource Cancel { get; private set; }
    public CookieContainer Cookies { get; private set; }

    public StaticDllHttpClient(CookieContainer cookie = null,
                CancellationTokenSource cancel = null,
                Dictionary<string, string> head = null)
    {
        Cancel = cancel ?? new();
        Cookies = cookie ?? new();
        Head = head ?? new();
        Client = HttpClientUtils.Get();
    }

    public void Dispose()
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
public partial class DllHttpClient : IDisposable
{
    protected HttpClient Client;

    public CancellationTokenSource Cancel { get; protected set; }
    public TimeSpan Time { get; set; } = TimeSpan.FromSeconds(10);

    public DllHttpClient(CookieContainer cookie = null,
                CancellationTokenSource cancel = null,
                Dictionary<string, string> head = null,
                WebProxy proxy = null)
    {
        Cancel = cancel ?? new();
        cookie ??= new();
        var HttpClientHandler = new HttpClientHandler()
        {
            CookieContainer = cookie,
            Proxy = proxy
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

    public void Dispose()
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

    /// <summary>
    /// 获取byte
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>byte</returns>
    public Task<byte[]> GetByteAsync(string url)
        => Client.GetByteArrayAsync(url, Cancel.Token);
    /// <summary>
    /// 获取字符串
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>字符串</returns>
    public Task<string> GetStringAsync(string url)
        => Client.GetStringAsync(url, Cancel.Token);
    /// <summary>
    /// 发送表单获取字符串
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">参数</param>
    /// <returns>字符串</returns>
    public async Task<string> PostStringAsync(string url, Dictionary<string, string> arg)
    {
        var temp = await Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token);
        return await temp.Content.ReadAsStringAsync();
    }
    /// <summary>
    /// 获取解析后的html
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>解析后的html</returns>
    public async Task<HtmlDoc> GetWebHtmlAsync(string url)
        => new HtmlDoc(await GetStringAsync(url));
    /// <summary>
    /// 发送表单获取解析后的html
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">数据</param>
    /// <returns></returns>
    public async Task<HtmlDoc> PostWebHtmlAsync(string url, Dictionary<string, string> arg)
        => new HtmlDoc(await PostStringAsync(url, arg));
    /// <summary>
    /// 进行一次http请求
    /// </summary>
    /// <param name="httpRequest">请求结构</param>
    /// <returns>返回结构</returns>
    public async Task<HttpResponseMessage> DoStringAsync(HttpRequestMessage httpRequest)
        => await Client.SendAsync(httpRequest, Cancel.Token);
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="data">数据</param>
    /// <returns>返回的字符串</returns>
    public async Task<string> PutStringAsync(string url, byte[] data)
    {
        var temp = await Client.PutAsync(url, new ByteArrayContent(data), Cancel.Token);
        return await temp.Content.ReadAsStringAsync();
    }
    /// <summary>
    /// 发送表单数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <param name="arg">数据</param>
    /// <returns>返回结构</returns>
    public async Task<HttpResponseMessage> PostDataAsync(string url, Dictionary<string, string> arg)
        => await Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token);
    /// <summary>
    /// Get获取数据
    /// </summary>
    /// <param name="url">网址</param>
    /// <returns>返回结构</returns>
    public async Task<HttpResponseMessage> GetDataAsync(string url)
        => await Client.GetAsync(url, Cancel.Token);
}

public partial class HtmlDoc
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
    /// <param name="node">标签名</param>
    /// <param name="name">class名字</param>
    /// <returns></returns>
    public List<HtmlNode> Select(string node, string name)
    {
        try
        {
            return html.DocumentNode.Descendants(node)
                    .Where(x => x.Attributes.Contains("class")
                    && x.Attributes["class"].Value == name)
                    .ToList();
        }
        catch
        {
            throw new ErrorDump("选择" + node + "_Class:" + name + "出错");
        }
    }
    /// <summary>
    /// 选择节点
    /// </summary>
    /// <param name="node">标签名</param>
    /// <param name="name">元素名字</param>
    /// <param name="attributes">元素</param>
    /// <returns></returns>
    public List<HtmlNode> Select(string node, string name, string attributes)
    {
        try
        {
            return html.DocumentNode.Descendants(node)
                    .Where(x => x.Attributes.Contains(name)
                    && x.Attributes[name].Value == attributes)
                    .ToList();
        }
        catch (Exception e)
        {
            throw new ErrorDump("选择" + node + "_" + node + ":" + attributes + "出错", e);
        }
    }
    /// <summary>
    /// 选择节点
    /// </summary>
    /// <param name="node">标签名</param>
    /// <returns></returns>
    public List<HtmlNode> Select(string node)
    {
        try
        {
            return html.DocumentNode.Descendants(node).ToList();
        }
        catch (Exception e)
        {
            throw new ErrorDump("选择" + node + "出错", e);
        }
    }
}
