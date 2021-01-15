using ColoryrServer.SDK;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.WebSocket
{
    internal class ServerWebSocket
    {
        private static WebSocketServer Server;
        private static Task PingThread;
        private static CancellationTokenSource source;
        private static bool IsRun;
        private static readonly Dictionary<int, IWebSocketConnection> Clients = new();
        internal static bool IsOnline(string id)
        {
            var data = Clients.Where(a => a.Value.ConnectionInfo.Id.ToString() == id);
            return data.Any();
        }
        internal static void Send(Guid uuid, string data)
        {
            var list = Clients.Where(a => a.Value.ConnectionInfo.Id == uuid);
            foreach (var item in list)
            {
                item.Value.Send(data);
            }
        }
        internal static void Send(int port, string data)
        {
            if (Clients.ContainsKey(port))
            {
                Clients[port].Send(data);
            }
        }
        internal static void Start()
        {
            ServerMain.LogOut("WebScoket正在启动");
            FleckLog.Level = LogLevel.Error;
            Server = new WebSocketServer("ws://" + ServerMain.Config.WebSocket.IP + ":" + ServerMain.Config.WebSocket.Port);
            Server.Start(Socket =>
            {
                Socket.OnOpen = () =>
                {
                    Clients.Add(Socket.ConnectionInfo.ClientPort, Socket);
                    DllManager.DllRun.WebSocketGo(new WebSocketOpen(Socket));
                };
                Socket.OnClose = () =>
                {
                    Clients.Remove(Socket.ConnectionInfo.ClientPort);
                    DllManager.DllRun.WebSocketGo(new WebSocketClose(Socket));
                };
                Socket.OnMessage = message =>
                {
                    DllManager.DllRun.WebSocketGo(new WebSocketMessage(Socket, message));
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
            ServerMain.LogOut("WebScoket已启动");
        }
        internal static void Stop()
        {
            IsRun = false;
            source.Cancel();
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

        internal static void StartPipe()
        {
            
        }
    }
}
