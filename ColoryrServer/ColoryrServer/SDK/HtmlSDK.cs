﻿using ColoryrServer.Html;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.SDK
{
    public class HttpHtml : IDisposable
    {
        private ExClient Http;
        public CancellationTokenSource Cancel;
        public CookieContainer Cookie;

        public HttpHtml(CookieContainer Cookie = null,
                    CancellationTokenSource Cancel = null,
                    Dictionary<string, string> Head = null)
        {
            Http = HttpClientUtils.Get();
            this.Cookie = Cookie ?? new();
            this.Cancel = Cancel ?? new();
            Http.Init(this.Cookie, Head);
        }

        public void Dispose()
        {
            HttpClientUtils.Close(Http);
        }
        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>byte</returns>
        public byte[] GetByte(string url)
            => Http.GetByteArray(url, Cancel);
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>字符串</returns>
        public string GetString(string url)
            => Http.GetString(url, Cancel);
        /// <summary>
        /// 发送表单获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">参数</param>
        /// <returns>字符串</returns>
        public string PostString(string url, Dictionary<string, string> arg)
            => Http.PostAsync(url, Cancel, arg);
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
            => Http.Send(httpRequest, Cancel);
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="data">数据</param>
        /// <returns>返回的字符串</returns>
        public string PutString(string url, byte[] data)
            => Http.Put(url, data, Cancel);
        /// <summary>
        /// 发送表单数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns>返回结构</returns>
        public HttpResponseMessage PostData(string url, Dictionary<string, string> arg)
            => Http.Post(url, arg, Cancel);
        /// <summary>
        /// Get获取数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>返回结构</returns>
        public HttpResponseMessage GetData(string url)
            => Http.Get(url, Cancel);
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
            Http.DefaultRequestHeaders.Add("User-Agent", DefauleThing.Agent);
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
