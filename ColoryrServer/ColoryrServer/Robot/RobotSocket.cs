using ColoryrServer.SDK;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ColoryrServer.Robot
{
    internal class RobotSocket
    {
        private static Socket Socket;
        private static Thread ReadThread;
        private static Thread DoThread;
        private static bool IsRun;
        private static bool IsConnect;
        public static List<long> QQs;
        private static ConcurrentBag<RobotTask> QueueRead;
        private static ConcurrentBag<byte[]> QueueSend;
        private static bool IsFirst = true;
        private static PackStart PackStart = new PackStart
        {
            Name = "ColoryrSDK",
            Reg = new List<byte>()
            {21, 28, 35, 36, 47, 49, 50, 51 }
        };
        internal static void Start()
        {
            if (QueueRead == null)
                QueueRead = new ConcurrentBag<RobotTask>();
            if (QueueSend == null)
                QueueSend = new ConcurrentBag<byte[]>();
            DoThread = new Thread(() =>
            {
                RobotTask task = new RobotTask();
                while (IsRun)
                {
                    try
                    {
                        if (QueueRead.TryTake(out task))
                        {
                            switch (task.index)
                            {
                                case 35:
                                case 36:
                                    var pack6 = JsonConvert.DeserializeObject<MemberJoinEventAPack>(task.data);
                                    DllManager.DllRun.RobotGo(new RobotEvent(RobotEvent.EventType.GroupMemberJoin, pack6.qq, pack6.id, pack6.fid, pack6.name, null, 0));
                                    break;
                                case 21:
                                    var pack4 = JsonConvert.DeserializeObject<FriendMessagePostSendEventPack>(task.data);
                                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.friend, pack4.qq, 0, pack4.id, pack4.res, pack4.error, pack4.message));
                                    break;
                                case 28:
                                    var pack5 = JsonConvert.DeserializeObject<GroupMessagePostSendEventPack>(task.data);
                                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.group, pack5.qq, pack5.id, 0, pack5.res, pack5.error, pack5.message));
                                    break;
                                case 47:
                                    var pack3 = JsonConvert.DeserializeObject<TempMessagePostSendEventPack>(task.data);
                                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.private_, pack3.qq, pack3.id, pack3.fid, pack3.res, pack3.error, pack3.message));
                                    break;
                                case 49:
                                    var pack = JsonConvert.DeserializeObject<GroupMessageEventPack>(task.data);
                                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.group, pack.qq, pack.id, pack.fid, pack.name, pack.message));
                                    break;
                                case 50:
                                    var pack1 = JsonConvert.DeserializeObject<TempMessageEventPack>(task.data);
                                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.private_, pack1.qq, pack1.id, pack1.fid, pack1.name, pack1.message));
                                    break;
                                case 51:
                                    var pack2 = JsonConvert.DeserializeObject<FriendMessageEventPack>(task.data);
                                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.friend, pack2.qq, 0, pack2.id, pack2.name, pack2.message));
                                    break;
                                case 60:
                                    break;
                            }
                        }
                        if (QueueSend.TryTake(out byte[] Send))
                        {
                            Socket.Send(Send);
                        }
                        Thread.Sleep(50);
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                        ServerMain.LogError(task.data);
                    }
                }
            });

            ReadThread = new Thread(() =>
            {
                while (!IsRun)
                {
                    Thread.Sleep(100);
                }
                DoThread.Start();
                int time = 0;
                while (IsRun)
                {
                    try
                    {
                        if (!IsConnect)
                            ReConnect();
                        else if (Socket.Available > 0)
                        {
                            var data = new byte[Socket.Available];
                            Socket.Receive(data);
                            var type = data[^1];
                            data[^1] = 0;
                            QueueRead.Add(new RobotTask
                            {
                                index = type,
                                data = Encoding.UTF8.GetString(data)
                            });
                        }
                        else if (time >= 100)
                        {
                            time = 0;
                            if (Socket.Poll(1000, SelectMode.SelectRead))
                            {
                                ServerMain.LogOut("机器人连接中断");
                                IsConnect = false;
                                ServerMain.LogOut("机器人10秒后重连");
                                Thread.Sleep(10000);
                            }
                        }
                        time++;
                        Thread.Sleep(10);
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError("机器人连接失败");
                        ServerMain.LogError(e);
                        IsConnect = false;
                        if (IsFirst)
                        {
                            IsRun = false;
                            return;
                        }
                        ServerMain.LogOut("机器人10秒后重连");
                        Thread.Sleep(10000);
                    }
                }
            });
            ReadThread.Start();
            IsRun = true;
        }
        private static void ReConnect()
        {
            ServerMain.LogOut("机器人重连中");
            if (Socket != null)
                Socket.Close();
            Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(IPAddress.Parse(ServerMain.Config.Robot.IP), ServerMain.Config.Robot.Port);

            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(PackStart) + " ");
            data[^1] = 0;

            Socket.Send(data);

            while (Socket.Available == 0)
            {
                Thread.Sleep(10);
            }

            data = new byte[Socket.Available];
            Socket.Receive(data);
            QQs = JArray.Parse(Encoding.UTF8.GetString(data)).ToObject<List<long>>();

            QueueRead.Clear();
            QueueSend.Clear();
            ServerMain.LogOut("机器人已连接");
            IsConnect = true;
        }
        internal static void SendGroupMessage(long qq, long id, List<string> message)
        {
            var data = BuildPack.Build(new SendGroupMessagePack { qq = qq, id = id, message = message }, 52);
            QueueSend.Add(data);
        }
        internal static void SendGroupPrivateMessage(long qq, long id, long fid, List<string> message)
        {
            var data = BuildPack.Build(new SendGroupPrivateMessagePack { qq = qq, id = id, fid = fid, message = message }, 53);
            QueueSend.Add(data);
        }
        internal static void SendFriendMessage(long qq, long id, List<string> message)
        {
            var data = BuildPack.Build(new SendFriendMessagePack { qq = qq, id = id, message = message }, 54);
            QueueSend.Add(data);
        }
        internal static void SendGroupImage(long qq, long id, string img)
        {
            var data = BuildPack.BuildImage(qq, id, 0, img, 61);
            QueueSend.Add(data);
        }
        internal static void SendGroupPrivateImage(long qq, long id, long fid, string img)
        {
            var data = BuildPack.BuildImage(qq, id, fid, img, 62);
            QueueSend.Add(data);
        }
        internal static void SendFriendImage(long qq, long id, string img)
        {
            var data = BuildPack.BuildImage(qq, id, 0, img, 63);
            QueueSend.Add(data);
        }

        internal static void SendGroupSound(long qq, long id, string sound)
        {
            var data = BuildPack.BuildSound(qq, id, sound, 74);
            QueueSend.Add(data);
        }
        internal static void SendGroupImageFile(long qq, long id, string file)
        {
            var data = BuildPack.Build(new LoadFileSendToGroupImagePack { qq = qq, id = id, file = file, }, 75);
            QueueSend.Add(data);
        }
        internal static void SendGroupPrivateImageFile(long qq, long id, long fid, string file)
        {
            var data = BuildPack.Build(new LoadFileSendToGroupPrivateImagePack { qq = qq, id = id, fid = fid, file = file, }, 76);
            QueueSend.Add(data);
        }
        internal static void SendFriendImageFile(long qq, long id, string file)
        {
            var data = BuildPack.Build(new LoadFileSendToFriendImagePack { qq = qq, id = id, file = file, }, 77);
            QueueSend.Add(data);
        }
        internal static void SendGroupSoundFile(long qq, long id, string file)
        {
            var data = BuildPack.Build(new LoadFileSendToGroupSoundPack { qq = qq, id = id, file = file, }, 77);
            QueueSend.Add(data);
        }
        internal static void SendStop()
        {
            var data = BuildPack.Build(new object(), 127);
            Socket.Send(data);
        }
        internal static void Stop()
        {
            ServerMain.LogOut("机器人正在断开");
            IsRun = false;
            SendStop();
            if (Socket != null)
                Socket.Close();
            ServerMain.LogOut("机器人已断开");
        }
        internal static void ReCall(string id)
        {
            var data = BuildPack.Build(new ReCallMessage { id = long.Parse(id) }, 71);
            QueueSend.Add(data);
        }
    }
}
