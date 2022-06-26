using ColoryrServer.Core.DllManager;
using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.IoT;

internal static class SocketServer
{
    private static TcpListener TcpServer;
    private static Socket UdpServer;
    private static readonly Dictionary<IPEndPoint, Socket> TcpClients = new();
    private static readonly Dictionary<IPEndPoint, EndPoint> UdpClients = new();
    private static readonly ReaderWriterLockSlim Lock1 = new();
    private static readonly ReaderWriterLockSlim Lock2 = new();

    private static Thread TcpThread;
    private static Thread UdpThread;

    public static bool RunFlag;

    public static void Start()
    {
        try
        {
            ServerMain.LogOut("Socket服务器正在启动");
            var ip = IPAddress.Parse(ServerMain.Config.Socket.IP);
            TcpServer = new TcpListener(ip, ServerMain.Config.Socket.Port);
            TcpServer.Start();
            UdpServer = new Socket(SocketType.Dgram, ProtocolType.Udp);
            UdpServer.Bind(new IPEndPoint(ip, ServerMain.Config.Socket.Port));
            ServerMain.LogOut($"Socket服务器监听{ServerMain.Config.Socket.IP}:{ServerMain.Config.Socket.Port}");

            RunFlag = true;

            TcpThread = new(() =>
            {
                while (RunFlag)
                {
                    try
                    {
                        var socket = TcpServer.AcceptSocket();
                        ThreadPool.UnsafeQueueUserWorkItem(OnConnectRequest, socket);
                    }
                    catch (Exception e)
                    {
                        if (RunFlag)
                            ServerMain.LogError(e);
                    }
                }
            })
            {
                Name = "TcpThread"
            };
            UdpThread = new(() =>
            {
                while (RunFlag)
                {
                    try
                    {
                        EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                        byte[] buffer = new byte[2048];
                        int length = UdpServer.ReceiveFrom(buffer, ref point);//接收数据报
                        if (point is IPEndPoint temp)
                        {
                            UdpClients.Remove(temp);
                            UdpClients.Add(temp, point);
                            ServerMain.LogOut("Socket|Udp:" + temp + " 已连接");
                            Task.Run(() =>
                            {
                                DllRun.SocketGo(new UdpSocketRequest(temp, buffer, length));
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        if (RunFlag)
                            ServerMain.LogError(e);
                    }
                }
            })
            {
                Name = "UdpThread"
            };
            TcpThread.Start();
            UdpThread.Start();
            ServerMain.OnStop += Stop;
            ServerMain.LogOut("Socket服务器已启动");
        }
        catch (Exception e)
        {
            ServerMain.LogOut("Socket服务器启动失败");
            ServerMain.LogError(e);
        }
    }

    private static void Stop()
    {
        ServerMain.LogOut("Socket服务器正在停止");
        RunFlag = false;
        UdpServer.Close();
        TcpServer.Stop();
        foreach (var item in TcpClients.Values)
        {
            if (item != null)
            {
                item.Dispose();
            }
        }
        UdpClients.Clear();
        TcpClients.Clear();
        ServerMain.LogOut("Socket服务器已停止");
    }

    public static void OnConnectRequest(object temp)
    {
        try
        {
            byte[] data;
            var client = temp as Socket;
            IPEndPoint port = null;
            if (client == null || !client.Connected)
            {
                client.Dispose();
                port = client.RemoteEndPoint as IPEndPoint;
                Remove(port);
                return;
            }
            Add(port, client);
            while (true)
            {
                if (!RunFlag || client == null || client.Available == -1)
                {
                    client.Dispose();
                    Remove(port);
                    return;
                }
                else if (client.Available != 0)
                {
                    data = new byte[client.Available];
                    client.Receive(data);
                    Task.Run(() =>
                    {
                        DllRun.SocketGo(new TcpSocketRequest(port, data, client.Available));
                    });
                }
                Thread.Sleep(100);
            }
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }

    private static void Add(IPEndPoint port, Socket client)
    {
        Lock1.EnterWriteLock();
        try
        {
            if (port != null)
            {
                if (TcpClients.ContainsKey(port))
                {
                    TcpClients[port].Dispose();
                    TcpClients.Remove(port);
                }
                TcpClients.Add(port, client);
                ServerMain.LogOut("Socket|Tcp:" + port + " 已连接");
            }
        }
        finally
        {
            Lock1.ExitWriteLock();
        }
    }
    private static void Remove(IPEndPoint port)
    {
        Lock1.EnterWriteLock();
        try
        {
            if (port != null)
            {
                TcpClients.Remove(port);
                ServerMain.LogOut("Socket|Tcp:" + port + " 已断开");
            }
        }
        finally
        {
            Lock1.ExitWriteLock();
        }
    }

    public static void TcpSendData(IPEndPoint port, byte[] data)
    {
        if (TcpClients.TryGetValue(port, out var socket))
        {
            if (socket.Connected)
            {
                socket.Send(data);
            }
            else
            {
                socket.Dispose();
                Remove(port);
            }
        }
    }
    public static void UdpSendData(IPEndPoint port, byte[] data)
    {
        if (UdpClients.TryGetValue(port, out var socket))
        {
            UdpServer.SendTo(data, socket);
        }
    }

    public static List<IPEndPoint> GetTcpList()
    {
        return new List<IPEndPoint>(TcpClients.Keys);
    }

    public static List<IPEndPoint> GetUdpList()
    {
        return new List<IPEndPoint>(UdpClients.Keys);
    }
}
