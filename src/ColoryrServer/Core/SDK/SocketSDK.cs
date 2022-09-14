﻿using ColoryrServer.Core.PortServer;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ColoryrServer.SDK;

public partial class SocketTcpRequest
{
    /// <summary>
    /// 端口
    /// </summary>
    public IPEndPoint Port { get; init; }
    /// <summary>
    /// 数据
    /// </summary>
    public byte[] Data { get; init; }
    /// <summary>
    /// 长度
    /// </summary>
    public int Length { get; set; }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="port">Socket端口</param>
    /// <param name="data">Socket发送的数据</param>
    public SocketTcpRequest(IPEndPoint port, byte[] data, int length)
    {
        Port = port;
        Data = data;
        Length = length;
    }
    /// <summary>
    /// 向Socket设备发送字符串
    /// </summary>
    /// <param name="Data">字符串</param>
    public void Send(string Data)
        => PortSocketServer.TcpSendData(Port, Encoding.UTF8.GetBytes(Data));
    public static void Send(IPEndPoint Port, string data)
        => PortSocketServer.TcpSendData(Port, Encoding.UTF8.GetBytes(data));
    /// <summary>
    /// 向Socket设备发送数据
    /// </summary>
    /// <param name="data">数据</param>
    public void Send(byte[] data)
        => PortSocketServer.TcpSendData(Port, data);
    public static void Send(IPEndPoint port, byte[] data)
       => PortSocketServer.TcpSendData(port, data);
}
public partial class SocketUdpRequest
{
    /// <summary>
    /// 端口
    /// </summary>
    public IPEndPoint Port { get; init; }
    /// <summary>
    /// 数据
    /// </summary>
    public byte[] Data { get; init; }
    /// <summary>
    /// 长度
    /// </summary>
    public int Length { get; set; }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="port">Socket端口</param>
    /// <param name="data">Socket发送的数据</param>
    public SocketUdpRequest(IPEndPoint port, byte[] data, int length)
    {
        Port = port;
        Data = data;
        Length = length;
    }
    /// <summary>
    /// 向Socket设备发送字符串
    /// </summary>
    /// <param name="data">字符串</param>
    public void Send(string data)
        => PortSocketServer.UdpSendData(Port, Encoding.UTF8.GetBytes(data));
    public static void Send(IPEndPoint port, string Data)
        => PortSocketServer.UdpSendData(port, Encoding.UTF8.GetBytes(Data));
    /// <summary>
    /// 向Socket设备发送数据
    /// </summary>
    /// <param name="data">数据</param>
    public void Send(byte[] data)
       => PortSocketServer.UdpSendData(Port, data);
    public static void Send(IPEndPoint port, byte[] data)
       => PortSocketServer.UdpSendData(port, data);
}

public static class SocketUtils
{
    /// <summary>
    /// 获取Socket设备列表
    /// </summary>
    /// <returns>设备列表</returns>
    public static List<IPEndPoint> GetTcpList()
        => PortSocketServer.GetTcpList();
    /// <summary>
    /// 获取Socket设备列表
    /// </summary>
    /// <returns>设备列表</returns>
    public static List<IPEndPoint> GetUdpList()
        => PortSocketServer.GetUdpList();
}