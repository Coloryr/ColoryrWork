using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.IoT
{
    class IoTSocket
    {
        private static TcpListener ServerSocket;
        private static Dictionary<string, Socket> Clients = new Dictionary<string, Socket>();
        private static bool RunFlag;
        private static ReaderWriterLockSlim Lock1 = new ReaderWriterLockSlim();

        public static void Start()
        {
            Task.Run(() =>
            {
                try
                {
                    ServerMain.LogOut("IoT服务器正在启动");
                    ServerSocket = new TcpListener(IPAddress.Parse(ServerMain.Config.IoT.IP), ServerMain.Config.IoT.Port);
                    ServerSocket.Start();

                    RunFlag = true;

                    Task.Factory.StartNew(async () =>
                    {
                        while (RunFlag)
                        {
                            var socket = await ServerSocket.AcceptSocketAsync();
                            ThreadPool.UnsafeQueueUserWorkItem(OnConnectRequest, socket);
                        }
                    });
                    ServerMain.LogOut("IoT服务器已启动");
                }
                catch (Exception e)
                {
                    ServerMain.LogOut("IoT服务器启动失败");
                    ServerMain.LogError(e);
                }
            });
        }

        public static void Stop()
        {
            ServerMain.LogOut("IoT服务器正在停止");
            RunFlag = false;
            ServerSocket.Stop();
            foreach (var item in Clients.Values)
            {
                if (item != null)
                {
                    item.Dispose();
                }
            }
            Clients.Clear();
            ServerMain.LogOut("IoT服务器已停止");
        }

        //当有客户端连接时的处理
        public static void OnConnectRequest(object temp)
        {
            try
            {
                byte[] Data;
                string Name = "";
                var Client = (Socket)temp;
                if (Client == null || !Client.Connected)
                {
                    Client.Dispose();
                    Remove(Name);
                    return;
                }
                while (Client.Available <= 0)
                {
                    Thread.Sleep(10);
                }
                Data = new byte[Client.Available];
                Client.Receive(Data);
                Name = IoTPack.CheckPack(Data);
                if (Name == null)
                {
                    ServerMain.LogOut("非法的IoT设备连接");
                    return;
                }
                Add(Name, Client);
                IoTPack.SendPack(Name, new byte[0]);
                while (true)
                {
                    if (Client == null)
                    {
                        Remove(Name);
                        return;
                    }
                    else if (!RunFlag || Client.Available == -1)
                    {
                        Client.Dispose();
                        Remove(Name);
                        return;
                    }
                    else if (Client.Available != 0)
                    {
                        Lock1.EnterReadLock();
                        try
                        {
                            if (Clients.TryGetValue(Name, out var socket))
                            {
                                while (socket.Available <= 0)
                                {
                                    Thread.Sleep(10);
                                }
                                Data = new byte[socket.Available];
                                socket.Receive(Data);
                                IoTPack.ReadPack(Name, Data);
                            }
                        }
                        finally
                        {
                            Lock1.ExitReadLock();
                        }
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        private static void Add(string Name, Socket Client)
        {
            Lock1.EnterWriteLock();
            try
            {
                if (Name != null)
                {
                    if (Clients.ContainsKey(Name))
                    {
                        Clients[Name].Dispose();
                        Clients.Remove(Name);
                    }
                    ServerMain.LogOut("IoT:" + Name + " is connected");
                }
                Clients.Add(Name, Client);
            }
            finally
            {
                Lock1.ExitWriteLock();
            }
        }
        private static void Remove(string Name)
        {
            Lock1.EnterWriteLock();
            try
            {
                if (Name != null)
                {
                    Clients.Remove(Name);
                    ServerMain.LogOut("IoT:" + Name + " is disconnect");
                }
            }
            finally
            {
                Lock1.ExitWriteLock();
            }
        }

        public static void SendData(string Name, byte[] data)
        {
            if (Clients.TryGetValue(Name, out var socket))
            {
                if (socket.Connected)
                {
                    socket.Send(data);
                }
                else
                {
                    socket.Dispose();
                    Remove(Name);
                }
            }
        }

        public static List<string> GetList()
        {
            return new List<string>(Clients.Keys);
        }
    }
}
