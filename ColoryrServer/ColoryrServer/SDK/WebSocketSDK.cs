using ColoryrServer.WebSocket;
using Fleck;

namespace ColoryrServer.SDK
{
    public class WebSocketMessage
    {
        public bool IsAvailable { get; init; }
        public IWebSocketConnectionInfo Info { get; init; }
        public string Data { get; init; }
        private int Server { get; init; }
        /// <summary>
        /// WebSocket传来数据
        /// </summary>
        /// <param name="Client">WS客户端</param>
        /// <param name="Data">WS本次传来的数据</param>
        public WebSocketMessage(IWebSocketConnection Client, string Data)
        {
            IsAvailable = Client.IsAvailable;
            Info = Client.ConnectionInfo;
            this.Data = Data;
        }
        public WebSocketMessage(bool IsAvailable, IWebSocketConnectionInfo Info, string Data, int Server)
        {
            this.IsAvailable = IsAvailable;
            this.Info = Info;
            this.Data = Data;
            this.Server = Server;
        }
        public void Send(string data)
        {
            ServerWebSocket.Send(Info.ClientPort, data, Server);
        }
        public void Send(byte[] data)
        {
            ServerWebSocket.Send(Info.ClientPort, data, Server);
        }
        public void Close()
        {
            ServerWebSocket.Close(Info.ClientPort, Server);
        }
    }
    public class WebSocketOpen
    {
        private int Server { get; init; }
        public bool IsAvailable { get; init; }
        public IWebSocketConnectionInfo Info { get; init; }
        /// <summary>
        /// WebSocket连接
        /// </summary>
        /// <param name="Client">WS客户端</param>
        public WebSocketOpen(IWebSocketConnection Client)
        {
            IsAvailable = Client.IsAvailable;
            Info = Client.ConnectionInfo;
        }
        public WebSocketOpen(bool IsAvailable, IWebSocketConnectionInfo Info, int Server)
        {
            this.IsAvailable = IsAvailable;
            this.Info = Info;
            this.Server = Server;
        }
        public void Send(string data)
        {
            ServerWebSocket.Send(Info.ClientPort, data, Server);
        }
        public void Send(byte[] data)
        {
            ServerWebSocket.Send(Info.ClientPort, data, Server);
        }
        public void Close()
        {
            ServerWebSocket.Close(Info.ClientPort, Server);
        }
    }
    public class WebSocketClose
    {
        public IWebSocketConnectionInfo Info { get; init; }
        /// <summary>
        /// WebSocket断开
        /// </summary>
        /// <param name="Client">WS客户端</param>
        public WebSocketClose(IWebSocketConnection Client)
        {
            Info = Client.ConnectionInfo;
        }
        public WebSocketClose(IWebSocketConnectionInfo Info)
        {
            this.Info = Info;
        }
    }
}
