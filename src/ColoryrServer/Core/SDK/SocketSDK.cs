﻿using ColoryrServer.IoTSocket;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.SDK;

public class TcpSocketRequest
{
    private int Server { get; init; }
    public int Port { get; init; }
    public byte[] Data { get; init; }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="Port">Socket端口</param>
    /// <param name="Data">Socket发送的数据</param>
    public TcpSocketRequest(int Port, byte[] Data, int Server)
    {
        this.Port = Port;
        this.Data = Data;
        this.Server = Server;
    }
    /// <summary>
    /// 向Socket设备发送字符串
    /// </summary>
    /// <param name="Name">设备名</param>
    /// <param name="Data">字符串</param>
    public void Send(int Port, string Data)
        => SocketPackDo.SendTcpPack(Port, Encoding.UTF8.GetBytes(Data), Server);
    /// <summary>
    /// 向Socket设备发送数据
    /// </summary>
    /// <param name="Name">设备名</param>
    /// <param name="Data">数据</param>
    public void Send(int Port, byte[] Data)
       => SocketPackDo.SendTcpPack(Port, Data, Server);
}
public class UdpSocketRequest
{
    private int Server { get; init; }
    public int Port { get; init; }
    public byte[] Data { get; init; }
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="Port">Socket端口</param>
    /// <param name="Data">Socket发送的数据</param>
    public UdpSocketRequest(int Port, byte[] Data, int Server)
    {
        this.Port = Port;
        this.Data = Data;
        this.Server = Server;
    }
    /// <summary>
    /// 向Socket设备发送字符串
    /// </summary>
    /// <param name="Name">设备名</param>
    /// <param name="Data">字符串</param>
    public void Send(int Port, string Data)
        => SocketPackDo.SendUdpPack(Port, Encoding.UTF8.GetBytes(Data), Server);
    /// <summary>
    /// 向Socket设备发送数据
    /// </summary>
    /// <param name="Name">设备名</param>
    /// <param name="Data">数据</param>
    public void Send(int Port, byte[] Data)
       => SocketPackDo.SendUdpPack(Port, Data, Server);
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
