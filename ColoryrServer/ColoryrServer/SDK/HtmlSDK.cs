using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColoryrServer.SDK
{
    public class Html : IDisposable
    {
        private HttpClient Http;
        private CancellationToken Cancel;

        protected Html(TimeSpan timeOut, CookieContainer CookieContainer,
            CancellationToken CancellationToken = new CancellationToken(),
            Dictionary<string, string> Head = null)
        {
            if (CookieContainer == null)
            {
                throw new VarDump("cookie储存不能为空");
            }
            var Handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            };
            Http = new HttpClient(Handler);
            Http.Timeout = timeOut;
            if (Head != null)
            {
                foreach (var Item in Head)
                {
                    Http.DefaultRequestHeaders.Add(Item.Key, Item.Value);
                }
            }
            else
            {
                Http.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");
            }
            Cancel = CancellationToken;
        }

        /// <summary>
        /// http爬虫
        /// </summary>
        /// <param name="cookieContainer">cookie储存对象</param>
        public Html() : this(TimeSpan.FromSeconds(10), new CookieContainer())
        {
        }

        /// <summary>
        /// http爬虫
        /// </summary>
        /// <param name="cookieContainer">cookie储存对象</param>
        public Html(CookieContainer cookieContainer) : this(TimeSpan.FromSeconds(10), cookieContainer)
        {
        }
        ~Html()
        {
            Dispose();
        }
        /// <summary>
        /// Http爬虫
        /// </summary>
        /// <param name="timeOut">请求超时</param>
        /// <param name="cancel">取消请求</param>
        /// <param name="head">请求头</param>
        /// <param name="cookieContainer">cookie储存对象</param>
        public Html(TimeSpan timeOut, CancellationToken cancel,
            Dictionary<string, string> head, CookieContainer cookieContainer) : this(timeOut, cookieContainer, cancel, head)
        {
        }
        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>byte</returns>
        public byte[] GetByte(string url)
        {
            return Http.GetByteArrayAsync(url, Cancel).Result;
        }
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>字符串</returns>
        public string GetString(string url)
        {
            return Http.GetStringAsync(url, Cancel).Result;
        }
        /// <summary>
        /// 发送表单获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">参数</param>
        /// <returns>字符串</returns>
        public string PostString(string url, Dictionary<string, string> arg)
        {
            return Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel).Result.Content.ReadAsStringAsync().Result;
        }
        /// <summary>
        /// 获取解析后的html
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>解析后的html</returns>
        public HtmlDoc GetWebHtml(string url)
        {
            var data = new HtmlDoc(GetString(url));
            return data;
        }
        /// <summary>
        /// 发送表单获取解析后的html
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns></returns>
        public HtmlDoc PostWebHtml(string url, Dictionary<string, string> arg)
        {
            var data = new HtmlDoc(PostString(url, arg));
            return data;
        }
        /// <summary>
        /// 进行一次http请求
        /// </summary>
        /// <param name="httpRequest">请求结构</param>
        /// <returns>返回结构</returns>
        public HttpResponseMessage Do(HttpRequestMessage httpRequest)
        {
            return Http.SendAsync(httpRequest, Cancel).Result;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="data">数据</param>
        /// <returns>返回的字符串</returns>
        public string PutString(string url, byte[] data)
        {
            return Http.PutAsync(url, new ByteArrayContent(data), Cancel).Result.Content.ReadAsStringAsync().Result;
        }
        /// <summary>
        /// 发送表单数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns>返回结构</returns>
        public HttpResponseMessage PostData(string url, Dictionary<string, string> arg)
        {
            return Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel).Result;
        }
        /// <summary>
        /// Get获取数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>返回结构</returns>
        public HttpResponseMessage GetData(string url)
        {
            return Http.GetAsync(url, Cancel).Result;
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Http.Dispose();
        }
    }

    public class HtmlAsync : IDisposable
    {
        private HttpClient Http;
        private CancellationToken Cancel;
        protected HtmlAsync(TimeSpan timeOut, CookieContainer CookieContainer,
            CancellationToken CancellationToken = new CancellationToken(),
            Dictionary<string, string> Head = null)
        {
            if (CookieContainer == null)
            {
                throw new VarDump("cookie储存不能为空");
            }
            var Handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            };
            Http = new HttpClient(Handler);
            Http.Timeout = timeOut;
            if (Head != null)
            {
                foreach (var Item in Head)
                {
                    Http.DefaultRequestHeaders.Add(Item.Key, Item.Value);
                }
            }
            else
            {
                Http.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");
            }
            Cancel = CancellationToken;
        }
        /// <summary>
        /// http爬虫
        /// </summary>
        /// <param name="cookieContainer">cookie储存对象</param>
        public HtmlAsync() : this(TimeSpan.FromSeconds(10), new CookieContainer())
        {
        }

        /// <summary>
        /// http爬虫
        /// </summary>
        /// <param name="cookieContainer">cookie储存对象</param>
        public HtmlAsync(CookieContainer cookieContainer) : this(TimeSpan.FromSeconds(10), cookieContainer)
        {
        }
        ~HtmlAsync()
        {
            Dispose();
        }
        /// <summary>
        /// Http爬虫
        /// </summary>
        /// <param name="timeOut">请求超时</param>
        /// <param name="cancel">取消请求</param>
        /// <param name="head">请求头</param>
        /// <param name="cookieContainer">cookie储存对象</param>
        public HtmlAsync(TimeSpan timeOut, CancellationToken cancel,
            Dictionary<string, string> head, CookieContainer cookieContainer) : this(timeOut, cookieContainer, cancel, head)
        {
        }
        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>byte</returns>
        public Task<byte[]> GetByte(string url)
        {
            return Http.GetByteArrayAsync(url, Cancel);
        }
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>字符串</returns>
        public Task<string> GetString(string url)
        {
            return Http.GetStringAsync(url, Cancel);
        }
        /// <summary>
        /// 发送表单获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">参数</param>
        /// <returns>字符串</returns>
        public async Task<string> PostString(string url, Dictionary<string, string> arg)
        {
            var temp = await Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel);
            return await temp.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// 获取解析后的html
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>解析后的html</returns>
        public async Task<HtmlDoc> GetWebHtml(string url)
        {
            var data = new HtmlDoc(await GetString(url));
            return data;
        }
        /// <summary>
        /// 发送表单获取解析后的html
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns></returns>
        public async Task<HtmlDoc> PostWebHtml(string url, Dictionary<string, string> arg)
        {
            var data = new HtmlDoc(await PostString(url, arg));
            return data;
        }
        /// <summary>
        /// 进行一次http请求
        /// </summary>
        /// <param name="httpRequest">请求结构</param>
        /// <returns>返回结构</returns>
        public async Task<HttpResponseMessage> DoString(HttpRequestMessage httpRequest)
        {
            return await Http.SendAsync(httpRequest, Cancel);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="data">数据</param>
        /// <returns>返回的字符串</returns>
        public async Task<string> PutString(string url, byte[] data)
        {
            var temp = await Http.PutAsync(url, new ByteArrayContent(data), Cancel);
            return await temp.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// 发送表单数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns>返回结构</returns>
        public async Task<HttpResponseMessage> PostData(string url, Dictionary<string, string> arg)
        {
            return await Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel);
        }
        /// <summary>
        /// Get获取数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>返回结构</returns>
        public async Task<HttpResponseMessage> GetData(string url)
        {
            return await Http.GetAsync(url, Cancel);
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Http.Dispose();
        }
    }
    public class HtmlDoc
    {
        public HtmlDocument html;
        /// <summary>
        /// Html解析
        /// </summary>
        /// <param name="data">Html字符串</param>
        public HtmlDoc(string data)
        {
            html = new HtmlDocument();
            html.LoadHtml(data);
        }
        /// <summary>
        /// Html解析
        /// </summary>
        /// <param name="data">Html节点</param>
        public HtmlDoc(HtmlNode data)
        {
            html = new HtmlDocument();
            html.LoadHtml(data.InnerHtml);
        }
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
                throw new VarDump("选择" + NodeName + "_Class:" + ClassName + "出错");
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
                throw new VarDump("选择" + NodeName + "_" + NodeName + ":" + Attributes + "出错", e);
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
                throw new VarDump("选择" + NodeName + "出错", e);
            }
        }
    }
}
