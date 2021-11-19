﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ColoryrServer.Socket
{
    public class SocketServer
    {
        private static TcpListener TcpServer;
        private static System.Net.Sockets.Socket UdpServer;
        private static readonly Dictionary<int, System.Net.Sockets.Socket> TcpClients = new();
        private static readonly Dictionary<int, EndPoint> UdpClients = new();
        private static readonly ReaderWriterLockSlim Lock1 = new();
        private static readonly ReaderWriterLockSlim Lock2 = new();
        private static bool IsPipe;

        private static Thread thread1;
        private static Thread thread2;

        public static bool RunFlag;

        public static void Start()
        {
            try
            {
                ServerMain.LogOut("Socket服务器正在启动");
                var ip = IPAddress.Parse(ServerMain.Config.Socket.IP);
                TcpServer = new TcpListener(ip, ServerMain.Config.Socket.Port);
                TcpServer.Start();
                ServerMain.LogOut($"Socket服务器监听{ServerMain.Config.Socket.IP}:{ServerMain.Config.Socket.Port}");
                UdpServer = new System.Net.Sockets.Socket(SocketType.Dgram, ProtocolType.Udp);
                UdpServer.Bind(new IPEndPoint(ip, ServerMain.Config.Socket.Port));

                RunFlag = true;

                thread1 = new(() =>
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
                });
                thread2 = new(() =>
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
                                UdpClients.Add(temp.Port, temp);
                                SocketPackDo.ReadUdpPack(temp.Port, buffer);
                            }
                        }
                        catch (Exception e)
                        {
                            if (RunFlag)
                                ServerMain.LogError(e);
                        }
                    }
                });
                thread1.Start();
                thread2.Start();
                ServerMain.LogOut("Socket服务器已启动");
            }
            catch (Exception e)
            {
                ServerMain.LogOut("Socket服务器启动失败");
                ServerMain.LogError(e);
            }
        }

        public static void Stop()
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
                byte[] Data;
                var Client = (System.Net.Sockets.Socket)temp;
                int Port = 0;
                if (Client == null || !Client.Connected)
                {
                    Client.Dispose();
                    var ip = (IPEndPoint)Client.RemoteEndPoint;
                    Port = ip.Port;
                    Remove(Port);
                    return;
                }
                while (Client.Available == 0)
                {
                    var pack = new byte[Client.Available];
                    if (SocketPackDo.CheckPack(pack))
                    {
                        return;
                    }
                }
                Add(Port, Client);
                while (true)
                {
                    if (!RunFlag || Client == null || Client.Available == -1)
                    {
                        Client.Dispose();
                        Remove(Port);
                        return;
                    }
                    else if (Client.Available != 0)
                    {
                        Data = new byte[Client.Available];
                        Client.Receive(Data);
                        SocketPackDo.ReadTcpPack(Port, Data);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        public static void PipeOnConnectRequest(object temp)
        {
            try
            {
                IsPipe = true;
                byte[] Data;
                var Client = (System.Net.Sockets.Socket)temp;
                int Port = 0;
                if (Client == null || !Client.Connected)
                {
                    Client.Dispose();
                    var ip = (IPEndPoint)Client.RemoteEndPoint;
                    Port = ip.Port;
                    Remove(Port);
                    return;
                }
                while (Client.Available == 0)
                {
                    var pack = new byte[Client.Available];
                    if (SocketPackDo.CheckPack(pack))
                    {
                        return;
                    }
                }
                Add(Port, Client);
                while (true)
                {
                    if (!RunFlag || Client == null || Client.Available == -1)
                    {
                        Client.Dispose();
                        Remove(Port);
                        return;
                    }
                    else if (Client.Available != 0)
                    {
                        Data = new byte[Client.Available];
                        Client.Receive(Data);
                        SocketPackDo.ReadTcpPack(Port, Data);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        private static void Add(int Port, System.Net.Sockets.Socket Client)
        {
            if (Client.ProtocolType == ProtocolType.Tcp)
            {
                Lock1.EnterWriteLock();
                try
                {
                    if (Port != 0)
                    {
                        if (TcpClients.ContainsKey(Port))
                        {
                            TcpClients[Port].Dispose();
                            TcpClients.Remove(Port);
                        }
                        ServerMain.LogOut("Socket|Tcp:" + Port + " is connected");
                    }
                    TcpClients.Add(Port, Client);
                }
                finally
                {
                    Lock1.ExitWriteLock();
                }
            }
            else if (Client.ProtocolType == ProtocolType.Udp)
            {
                Lock2.EnterWriteLock();
                try
                {
                    if (Port != 0)
                    {
                        if (TcpClients.ContainsKey(Port))
                        {
                            TcpClients[Port].Dispose();
                            TcpClients.Remove(Port);
                        }
                        ServerMain.LogOut("Socket|Udp:" + Port + " is connected");
                    }
                    TcpClients.Add(Port, Client);
                }
                finally
                {
                    Lock2.ExitWriteLock();
                }
            }
        }
        private static void Remove(int port)
        {
            Lock1.EnterWriteLock();
            try
            {
                if (port != 0)
                {
                    TcpClients.Remove(port);
                    ServerMain.LogOut("Socket|Tcp:" + port + " is disconnect");
                }
            }
            finally
            {
                Lock1.ExitWriteLock();
            }
        }

        public static void TcpSendData(int port, byte[] data, int Server)
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
        public static void UdpSendData(int port, byte[] data, int Server)
        {
            if (UdpClients.TryGetValue(port, out var socket))
            {
                UdpServer.SendTo(data, socket);
            }
        }

        public static List<int> GetTcpList()
        {
            if (IsPipe)
                return null;
            return new List<int>(TcpClients.Keys);
        }

        public static List<int> GetUdpList()
        {
            if (IsPipe)
                return null;
            return new List<int>(UdpClients.Keys);
        }
    }
}
