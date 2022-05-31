using ColoryrServer.DllManager;
using ColoryrServer.SDK;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ColoryrServer.WebSocket;

internal class ServerWebSocket
{
    private record SocketObj
    {
        public int Port { get; set; }
        public Guid UUID { get; set; }
    }

    private static WebSocketServer Server;
    private static readonly Dictionary<SocketObj, IWebSocketConnection> Clients = new();
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
        Server = new WebSocketServer("ws://" + ServerMain.Config.WebSocket.IP + ":" + ServerMain.Config.WebSocket.Port);
        ServerMain.LogOut($"WebScoket监听{ServerMain.Config.WebSocket.IP}:{ServerMain.Config.WebSocket.Port}");
        Server.Start(Socket =>
        {
            Socket.OnOpen = () =>
            {
                Clients.Add(new SocketObj 
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
        ServerMain.LogOut("WebScoket服务器已启动");
    }
    public static void Stop()
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
