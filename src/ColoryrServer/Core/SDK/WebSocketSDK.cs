using ColoryrServer.Core.WebSocket;
using Fleck;

namespace ColoryrServer.SDK;

public static class WebSocketUtils
{
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="info">接口</param>
    /// <param name="data">数据</param>
    public static void Send(IWebSocketConnectionInfo info, string data)
        => ServerWebSocket.Send(info.ClientPort, data);
    public static void Send(IWebSocketConnectionInfo info, byte[] data)
        => ServerWebSocket.Send(info.ClientPort, data);
    public static void Close(IWebSocketConnectionInfo info)
        => ServerWebSocket.Close(info.ClientPort);
}

public class WebSocketMessage
{
    public bool IsAvailable { get; init; }
    public IWebSocketConnectionInfo Info { get; init; }
    public string Data { get; init; }
    /// <summary>
    /// WebSocket传来数据
    /// </summary>
    /// <param name="client">WS客户端</param>
    /// <param name="data">WS本次传来的数据</param>
    public WebSocketMessage(IWebSocketConnection client, string data)
    {
        IsAvailable = client.IsAvailable;
        Info = client.ConnectionInfo;
        Data = data;
    }
}
public class WebSocketOpen
{
    public bool IsAvailable { get; init; }
    public IWebSocketConnectionInfo Info { get; init; }
    /// <summary>
    /// WebSocket连接
    /// </summary>
    /// <param name="client">WS客户端</param>
    public WebSocketOpen(IWebSocketConnection client)
    {
        IsAvailable = client.IsAvailable;
        Info = client.ConnectionInfo;
    }
    public WebSocketOpen(bool isAvailable, IWebSocketConnectionInfo info)
    {
        IsAvailable = isAvailable;
        Info = info;
    }
}
public class WebSocketClose
{
    public IWebSocketConnectionInfo Info { get; init; }
    /// <summary>
    /// WebSocket断开
    /// </summary>
    /// <param name="client">WS客户端</param>
    public WebSocketClose(IWebSocketConnection client)
        => Info = client.ConnectionInfo;
    public WebSocketClose(IWebSocketConnectionInfo info)
        => Info = info;
}
