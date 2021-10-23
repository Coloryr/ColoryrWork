using ColoryrServer.DllManager;
using ColoryrServer.SDK;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.WebSocket
{
    public class ServerWebSocket
    {
        private static WebSocketServer Server;
        private static Task PingThread;
        private static CancellationTokenSource source;
        private static bool IsRun;
        private static readonly Dictionary<int, IWebSocketConnection> Clients = new();
        private static readonly Dictionary<Guid, IWebSocketConnection> ClientsID = new();
        internal static IWebSocketConnection Get(int port)
        {
            var data = Clients.Where(a => a.Key == port);
            if (data.Any())
                return data.First().Value;
            return null;
        }
        internal static IWebSocketConnection Get(Guid id)
        {
            var data = ClientsID.Where(a => a.Key == id);
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
            if (Clients.ContainsKey(port))
            {
                Clients[port].Send(data);
            }
        }
        internal static void Send(int port, byte[] data)
        {
            if (Clients.ContainsKey(port))
            {
                Clients[port].Send(data);
            }
        }
        internal static void Close(int port)
        {
            if (Clients.ContainsKey(port))
            {
                Clients[port].Close();
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
                    Clients.Add(Socket.ConnectionInfo.ClientPort, Socket);
                    ClientsID.Add(Socket.ConnectionInfo.Id, Socket);
                    DllRun.WebSocketGo(new WebSocketOpen(Socket));
                };
                Socket.OnClose = () =>
                {
                    Clients.Remove(Socket.ConnectionInfo.ClientPort);
                    ClientsID.Remove(Socket.ConnectionInfo.Id);
                    DllRun.WebSocketGo(new WebSocketClose(Socket));
                };
                Socket.OnMessage = message =>
                {
                    DllRun.WebSocketGo(new WebSocketMessage(Socket, message));
                };
            });
            source = new CancellationTokenSource();
            PingThread = new Task(() =>
            {
                while (IsRun)
                {
                    foreach (var item in Clients)
                    {
                        item.Value.Send("{\"type\":\"ping\"}");
                    }
                    Thread.Sleep(30000);
                }
            }, source.Token);
            IsRun = true;
            PingThread.Start();
            ServerMain.LogOut("WebScoket服务器已启动");
        }
        public static void Stop()
        {
            IsRun = false;
            source.Cancel(false);
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
}
