using ColoryrServer.Core.DllManager;
using ColoryrServer.SDK;
using Fleck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ColoryrServer.Core.PortServer;

internal static class PortWebSocket
{
    private record PortWebSocketObj
    {
        public int Port { get; set; }
        public Guid UUID { get; set; }
    }

    private static WebSocketServer Server;
    private static readonly Dictionary<PortWebSocketObj, IWebSocketConnection> Clients = new();
    internal static IWebSocketConnection Get(int port)
    {
        var data = Clients.Where(a => a.Key.Port == port);
        if (data.Any())
            return data.First().Value;
        return null;
    }
    internal static IWebSocketConnection Get(Guid id)
    {
        var data = Clients.Where(a => a.Key.UUID == id);
        if (data.Any())
            return data.First().Value;
        return null;
    }
    internal static bool IsOnline(string id)
    {
        var data = Clients.Where(a => a.Value.ConnectionInfo.Id.ToString() == id);
        return data.Any();
    }
    internal static void Send(int port, string data)
    {
        var item = Clients.Where(a => a.Key.Port == port);
        if (item.Any())
        {
            item.First().Value.Send(data);
        }
    }
    internal static void Send(int port, byte[] data)
    {
        var item = Clients.Where(a => a.Key.Port == port);
        if (item.Any())
        {
            item.First().Value.Send(data);
        }
    }
    internal static void Close(int port)
    {
        var item = Clients.Where(a => a.Key.Port == port);
        if (item.Any())
        {
            item.First().Value.Close();
        }
    }

    public static void Start()
    {
        ServerMain.LogOut("WebScoket服务器正在启动");
        FleckLog.Level = LogLevel.Error;
        string url = ServerMain.Config.WebSocket.Socket.IP + ":" + ServerMain.Config.WebSocket.Socket.Port;
        if (ServerMain.Config.WebSocket.UseSsl && File.Exists(ServerMain.Config.WebSocket.Ssl))
        {
            try
            {
                var ssl = new X509Certificate2(ServerMain.Config.WebSocket.Ssl, ServerMain.Config.WebSocket.Password);
                Server = new WebSocketServer("wss://" + url)
                {
                    EnabledSslProtocols = SslProtocols.Tls13,
                    Certificate = ssl
                };
                ServerMain.LogOut($"WebScoket使用SSL证书{ServerMain.Config.WebSocket.Ssl}");
            }
            catch (CryptographicException e)
            {
                ServerMain.LogError($"WebScoket使用SSL证书{ServerMain.Config.WebSocket.Ssl}错误");
                ServerMain.LogError(e);
            }
        }
        else
        {
            Server = new WebSocketServer("ws://" + url);
        }

        ServerMain.LogOut($"WebScoket监听{url}");
        Server.Start(Socket =>
        {
            Socket.OnOpen = () =>
            {
                Clients.Add(new PortWebSocketObj
                {
                    Port = Socket.ConnectionInfo.ClientPort,
                    UUID = Socket.ConnectionInfo.Id
                }, Socket);
                DllRun.WebSocketGo(new WebSocketOpen(Socket));
            };
            Socket.OnClose = () =>
            {
                var item = Clients.Where(a => a.Key.Port == Socket.ConnectionInfo.ClientPort);
                if (item.Any())
                {
                    var item1 = item.First();
                    Clients.Remove(item1.Key);
                    DllRun.WebSocketGo(new WebSocketClose(Socket));
                }
            };
            Socket.OnMessage = message =>
            {
                DllRun.WebSocketGo(new WebSocketMessage(Socket, message));
            };
        });
        ServerMain.OnStop += Stop;
        ServerMain.LogOut("WebScoket服务器已启动");
    }
    private static void Stop()
    {
        foreach (var item in Clients)
        {
            try
            {
                item.Value.Close();
            }
            catch
            {

            }
        }
        Server.Dispose();
        ServerMain.LogOut("WebScoket已停止");
    }
}
