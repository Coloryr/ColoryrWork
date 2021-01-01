using ColoryrServer;
using Fleck;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.WebSocket
{
    class ServerWebSocket
    {
        private static WebSocketServer Server;
        private static Task PingThread;
        private static CancellationTokenSource source;
        private static bool IsRun;
        private static readonly List<IWebSocketConnection> Clients = new List<IWebSocketConnection>();
        internal static bool IsOnline(string id)
        {
            var data = Clients.Where(a => a.ConnectionInfo.Id.ToString() == id);
            return data.Count() != 0;
        }
        internal static void Start()
        {
            FleckLog.Level = LogLevel.Error;
            Server = new WebSocketServer("ws://" + ServerMain.Config.WebSocket.IP + ":" + ServerMain.Config.WebSocket.Port);
            Server.Start(Socket =>
            {
                Socket.OnOpen = () =>
                {
                    Clients.Add(Socket);
                    DllManager.DllRun.WebSocketGo(new WebSocketOpen(Socket));
                };
                Socket.OnClose = () =>
                {
                    Clients.Remove(Socket);
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
                        item.Send("{\"type\":\"ping\"}");
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
                    item.Close();
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
