namespace ColoryrServer.SDK
{
    public class WebSocketMessage
    {
        public IWebSocketConnection Client { get; private set; }
        public string Data { get; private set; }
        /// <summary>
        /// WebSocket传来数据
        /// </summary>
        /// <param name="Client">WS客户端</param>
        /// <param name="Data">WS本次传来的数据</param>
        public WebSocketMessage(IWebSocketConnection Client, string Data)
        {
            this.Client = Client;
            this.Data = Data;
        }
    }
    public class WebSocketOpen
    {
        public IWebSocketConnection Client { get; private set; }
        /// <summary>
        /// WebSocket连接
        /// </summary>
        /// <param name="Client">WS客户端</param>
        public WebSocketOpen(IWebSocketConnection Client)
        {
            this.Client = Client;
        }
    }
    public class WebSocketClose
    {
        public IWebSocketConnection Client { get; private set; }
        /// <summary>
        /// WebSocket断开
        /// </summary>
        /// <param name="Client">WS客户端</param>
        public WebSocketClose(IWebSocketConnection Client)
        {
            this.Client = Client;
        }
    }
}
