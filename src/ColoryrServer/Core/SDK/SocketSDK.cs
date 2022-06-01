using ColoryrServer.Core.IoT;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.SDK;

public class TcpSocketRequest
{
    public int Port { get; init; }
    public byte[] Data { get; init; }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="Port">Socket端口</param>
    /// <param name="Data">Socket发送的数据</param>
    public TcpSocketRequest(int Port, byte[] Data)
    {
        this.Port = Port;
        this.Data = Data;
    }
    /// <summary>
    /// 向Socket设备发送字符串
    /// </summary>
    /// <param name="Data">字符串</param>
    public void Send(string Data)
        => SocketServer.TcpSendData(Port, Encoding.UTF8.GetBytes(Data));
    public void Send(int Port, string Data)
        => SocketServer.TcpSendData(Port, Encoding.UTF8.GetBytes(Data));
    /// <summary>
    /// 向Socket设备发送数据
    /// </summary>
    /// <param name="Data">数据</param>
    public void Send(byte[] Data)
        => SocketServer.TcpSendData(Port, Data);
    public void Send(int Port, byte[] Data)
       => SocketServer.TcpSendData(Port, Data);
}
public class UdpSocketRequest
{
    public int Port { get; init; }
    public byte[] Data { get; init; }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="Port">Socket端口</param>
    /// <param name="Data">Socket发送的数据</param>
    public UdpSocketRequest(int Port, byte[] Data)
    {
        this.Port = Port;
        this.Data = Data;
    }
    /// <summary>
    /// 向Socket设备发送字符串
    /// </summary>
    /// <param name="Data">字符串</param>
    public void Send(string Data)
        => SocketServer.UdpSendData(Port, Encoding.UTF8.GetBytes(Data));
    public void Send(int Port, string Data)
        => SocketServer.UdpSendData(Port, Encoding.UTF8.GetBytes(Data));
    /// <summary>
    /// 向Socket设备发送数据
    /// </summary>
    /// <param name="Data">数据</param>
    public void Send(byte[] Data)
       => SocketServer.UdpSendData(Port, Data);
    public void Send(int Port, byte[] Data)
       => SocketServer.UdpSendData(Port, Data);
}

public class TcpSocket
{
    /// <summary>
    /// 获取Socket设备列表
    /// </summary>
    /// <returns>设备列表</returns>
    public static List<int> GetList()
        => SocketServer.GetTcpList();
}

public class UdpSocket
{
    /// <summary>
    /// 获取Socket设备列表
    /// </summary>
    /// <returns>设备列表</returns>
    public static List<int> GetList()
        => SocketServer.GetUdpList();
}
