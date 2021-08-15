using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ColoryrServer.Html
{
    internal enum ClientState
    {
        Ready, Using, Closing, Stop
    }
    class DefauleThing
    {
        public const string Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77";
    }
    class ExClient
    {
        public ClientState State { get; set; }
        private readonly HttpClient Client;
        private readonly HttpClientHandler HttpClientHandler;
        private readonly object l = new();
        private readonly CookieContainer cookie = new();
        private Dictionary<string, string> RequestHeaders;
        public ExClient()
        {
            State = ClientState.Ready;
            HttpClientHandler = new();
            Client = new(HttpClientHandler);
            Client.Timeout = TimeSpan.FromSeconds(5);
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(DefauleThing.Agent);
            HttpClientHandler.CookieContainer = cookie;
        }
        public void Clear()
        {
            lock (l)
            {
                Client.CancelPendingRequests();
            }
        }
        public void Init(ref CookieContainer Cookie, Dictionary<string, string> RequestHeaders)
        {
            lock (l)
            {
                Cookie = cookie;
                this.RequestHeaders = RequestHeaders;
            }
        }
        public byte[] GetByteArray(string url, CancellationTokenSource Cancel)
        {
            if (RequestHeaders != null)
            {
                HttpRequestMessage message = new();
                foreach (var item in RequestHeaders)
                {
                    message.Headers.Add(item.Key, item.Value);
                }
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);
                var res = Client.SendAsync(message, Cancel.Token).Result;
                return res.Content.ReadAsByteArrayAsync(Cancel.Token).Result;
            }
            else
                return Client.GetByteArrayAsync(url, Cancel.Token).Result;
        }
        public string GetString(string url, CancellationTokenSource Cancel)
        {
            if (RequestHeaders != null)
            {
                HttpRequestMessage message = new();
                foreach (var item in RequestHeaders)
                {
                    message.Headers.Add(item.Key, item.Value);
                }
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);
                var res = Client.SendAsync(message, Cancel.Token).Result;
                return res.Content.ReadAsStringAsync(Cancel.Token).Result;
            }
            else
                return Client.GetStringAsync(url, Cancel.Token).Result;
        }
        public string PostAsync(string url, CancellationTokenSource Cancel, Dictionary<string, string> arg)
        {
            if (RequestHeaders != null)
            {
                HttpRequestMessage message = new();
                foreach (var item in RequestHeaders)
                {
                    message.Headers.Add(item.Key, item.Value);
                }
                message.Method = HttpMethod.Post;
                message.RequestUri = new Uri(url);
                message.Content = new FormUrlEncodedContent(arg);
                var res = Client.SendAsync(message, Cancel.Token).Result;
                return res.Content.ReadAsStringAsync(Cancel.Token).Result;
            }
            else
                return Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token).Result.Content.ReadAsStringAsync().Result;
        }
        public HttpResponseMessage Send(HttpRequestMessage httpRequest, CancellationTokenSource Cancel)
        {
            return Client.SendAsync(httpRequest, Cancel.Token).Result;
        }
        public string Put(string url, byte[] data, CancellationTokenSource Cancel)
        {
            if (RequestHeaders != null)
            {
                HttpRequestMessage message = new();
                foreach (var item in RequestHeaders)
                {
                    message.Headers.Add(item.Key, item.Value);
                }
                message.Method = HttpMethod.Put;
                message.RequestUri = new Uri(url);
                message.Content = new ByteArrayContent(data);
                var res = Client.SendAsync(message, Cancel.Token).Result;
                return res.Content.ReadAsStringAsync(Cancel.Token).Result;
            }
            else
                return Client.PutAsync(url, new ByteArrayContent(data), Cancel.Token).Result.Content.ReadAsStringAsync().Result;
        }
        public HttpResponseMessage Post(string url, Dictionary<string, string> arg, CancellationTokenSource Cancel)
        {
            if (RequestHeaders != null)
            {
                HttpRequestMessage message = new();
                foreach (var item in RequestHeaders)
                {
                    message.Headers.Add(item.Key, item.Value);
                }
                message.Method = HttpMethod.Post;
                message.RequestUri = new Uri(url);
                message.Content = new FormUrlEncodedContent(arg);
                return Client.SendAsync(message, Cancel.Token).Result;
            }
            else
                return Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token).Result;
        }
        public HttpResponseMessage Get(string url, CancellationTokenSource Cancel)
        {
            if (RequestHeaders != null)
            {
                HttpRequestMessage message = new();
                foreach (var item in RequestHeaders)
                {
                    message.Headers.Add(item.Key, item.Value);
                }
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);
                return Client.SendAsync(message, Cancel.Token).Result;
            }
            else
                return Client.GetAsync(url, Cancel.Token).Result;
        }
        public void Stop()
        {
            Client.Dispose();
        }
    }
}
