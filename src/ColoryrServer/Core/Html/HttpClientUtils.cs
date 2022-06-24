using System;
using System.Net.Http;
using System.Threading;

namespace ColoryrServer.Core.Html;

internal class HttpClientUtils
{
    private static ExHttpClient[] Clients;
    private static int now = 0;
    private static object _lock = new();
    public static void Start()
    {
        ServerMain.OnStop += Stop;
        Clients = new ExHttpClient[ServerMain.Config.HttpClientNumber];
        for (int a = 0; a < ServerMain.Config.HttpClientNumber; a++)
        {
            var handler = new HttpClientHandler() { UseCookies = false };
            var item = new ExHttpClient
            {
                Client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                },
                State = ClientState.Ready
            };
            item.Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");
            Clients[a] = item;
        }
    }

    public static void Close(ExHttpClient client)
    {
        client.State = ClientState.Close;
        client.Client.CancelPendingRequests();
        client.State = ClientState.Ready;
    }

    public static ExHttpClient Get()
    {
        ExHttpClient item;
        while (true)
        {
            lock (_lock)
            {
                item = Clients[now];
                now++;
                if (now >= Clients.Length)
                    now = 0;
            }
            if (item.State == ClientState.Ready)
            {
                item.State = ClientState.Using;
                return item;
            }
            Thread.Sleep(1);
        }
    }

    private static void Stop()
    {
        foreach (var item in Clients)
        {
            if (item != null)
            {
                item.State = ClientState.Close;
                item.Client.CancelPendingRequests();
                item.Client.Dispose();
            }
        }
    }
}
