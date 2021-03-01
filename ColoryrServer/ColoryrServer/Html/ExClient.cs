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
        public ExClient()
        {
            State = ClientState.Ready;
            HttpClientHandler = new();
            Client = new(HttpClientHandler);
            Client.Timeout = TimeSpan.FromSeconds(5);
            Client.DefaultRequestHeaders.Add("User-Agent", DefauleThing.Agent);
        }
        public void Clear()
        {
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Add("User-Agent", DefauleThing.Agent);
        }
        public void Init(CookieContainer Cookie, Dictionary<string, string> RequestHeaders)
        {
            HttpClientHandler.CookieContainer = Cookie;
            if (RequestHeaders != null)
            {
                foreach (var item in RequestHeaders)
                {
                    Client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
        }
        public byte[] GetByteArray(string url, CancellationTokenSource Cancel)
        {
            return Client.GetByteArrayAsync(url, Cancel.Token).Result;
        }
        public string GetString(string url, CancellationTokenSource Cancel)
        {
            return Client.GetStringAsync(url, Cancel.Token).Result;
        }
        public string PostAsync(string url, CancellationTokenSource Cancel, Dictionary<string, string> arg)
        {
            return Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token).Result.Content.ReadAsStringAsync().Result;
        }
        public HttpResponseMessage Send(HttpRequestMessage httpRequest, CancellationTokenSource Cancel)
        {
            return Client.SendAsync(httpRequest, Cancel.Token).Result;
        }
        public string Put(string url, byte[] data, CancellationTokenSource Cancel)
        {
            return Client.PutAsync(url, new ByteArrayContent(data), Cancel.Token).Result.Content.ReadAsStringAsync().Result;
        }
        public HttpResponseMessage Post(string url, Dictionary<string, string> arg, CancellationTokenSource Cancel)
        {
            return Client.PostAsync(url, new FormUrlEncodedContent(arg), Cancel.Token).Result;
        }
        public HttpResponseMessage Get(string url, CancellationTokenSource Cancel)
        {
            return Client.GetAsync(url, Cancel.Token).Result;
        }
        public void Stop()
        {
            Client.Dispose();
        }
    }
}
