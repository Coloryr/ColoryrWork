using ColoryrServer.DllManager;
using ColoryrServer.Pipe;
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
        private static bool IsPipe;
        private static readonly Dictionary<int, IWebSocketConnection> Clients = new();
        internal static bool IsOnline(string id)
        {
            var data = Clients.Where(a => a.Value.ConnectionInfo.Id.ToString() == id);
            return data.Any();
        }
        internal static void Send(int port, string data, int Server)
        {
            if (IsPipe)
            {
                PipeServer.WebSocketSendMessage(port, Server, data, false);
            }
            else if (Clients.ContainsKey(port))
            {
                Clients[port].Send(data);
            }
        }
        internal static void Send(int port, byte[] data, int Server)
        {
            if (IsPipe)
            {
                PipeServer.WebSocketSendMessage(port, Server, Convert.ToBase64String(data), true);
            }
            else if (Clients.ContainsKey(port))
            {
                Clients[port].Send(data);
            }
        }
        internal static void Close(int port, int Server)
        {
            if (IsPipe)
            {
                PipeServer.WebSocketSendClose(port, Server);
            }
            else if (Clients.ContainsKey(port))
            {
                Clients[port].Close();
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
                    DllRun.WebSocketGo(new WebSocketOpen(Socket));
                };
                Socket.OnClose = () =>
                {
                    Clients.Remove(Socket.ConnectionInfo.ClientPort);
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
            ServerMain.LogOut("WebScoket正在启动");
            IsPipe = true;
            FleckLog.Level = LogLevel.Error;
            Server = new WebSocketServer("ws://" + ServerMain.Config.WebSocket.IP + ":" + ServerMain.Config.WebSocket.Port);
            Server.Start(Socket =>
            {
                Socket.OnOpen = () =>
                {
                    Clients.Add(Socket.ConnectionInfo.ClientPort, Socket);
                    Task.Run(() => PipeClient.WebSocket(Socket.ConnectionInfo.ClientPort, new PipeWebSocketData
                    {
                        State = WebSocketState.Open,
                        Info = Socket.ConnectionInfo,
                        IsAvailable = Socket.IsAvailable,
                    }));
                };
                Socket.OnClose = () =>
                {
                    Clients.Remove(Socket.ConnectionInfo.ClientPort);
                    Clients.Add(Socket.ConnectionInfo.ClientPort, Socket);
                    Task.Run(() => PipeClient.WebSocket(Socket.ConnectionInfo.ClientPort, new PipeWebSocketData
                    {
                        State = WebSocketState.Close,
                        Info = Socket.ConnectionInfo,
                        IsAvailable = Socket.IsAvailable,
                    }));
                };
                Socket.OnMessage = message =>
                {
                    Clients.Add(Socket.ConnectionInfo.ClientPort, Socket);
                    Task.Run(() => PipeClient.WebSocket(Socket.ConnectionInfo.ClientPort, new PipeWebSocketData
                    {
                        State = WebSocketState.Message,
                        Info = Socket.ConnectionInfo,
                        IsAvailable = Socket.IsAvailable,
                        Data = message
                    }));
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
    }
}
