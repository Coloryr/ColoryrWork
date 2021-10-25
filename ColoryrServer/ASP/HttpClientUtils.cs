using ColoryrServer.Html;
using System;
using System.Net.Http;
using System.Threading;

namespace ColoryrServer.ASP
{
    public class HttpClients1
    {
        private static HttpClient[] Clients;
        private static int now = 0;
        private static object _lock = new();
        public static void Start()
        {
            Clients = new HttpClient[ServerMain.Config.HttpClientNumber];
            for (int a = 0; a < ServerMain.Config.HttpClientNumber; a++)
            {
                var handler = new HttpClientHandler() { UseCookies = false };
                var item = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(5)
                };
                Clients[a] = item;
            }
        }

        public static HttpClient GetOne()
        {
            HttpClient item;
            lock (_lock)
            {
                item = Clients[now];
                now++;
                if (now >= Clients.Length)
                    now = 0;
            }
            return item;
        }

        public static void Stop()
        {
            foreach (var item in Clients)
            {
                if (item != null)
                {
                    item.CancelPendingRequests();
                    item.Dispose();
                }
            }
        }
    }
}
