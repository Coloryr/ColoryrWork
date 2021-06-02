using ColoryrServer.FileSystem;
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
    internal class RobotUtils
    {
        private static Robot robot = new();
        private static void Message(byte type, string data)
        {
            switch (type)
            {
                case 35:
                    var pack7 = JsonConvert.DeserializeObject<InviteMemberJoinEventPack>(data);
                    DllManager.DllRun.RobotGo(new RobotEvent(RobotEvent.EventType.GroupMemberJoin, pack7.qq, pack7.id, pack7.fid, pack7.name, null, 0));
                    break;
                case 36:
                    var pack6 = JsonConvert.DeserializeObject<MemberJoinEventPack>(data);
                    DllManager.DllRun.RobotGo(new RobotEvent(RobotEvent.EventType.GroupMemberJoin, pack6.qq, pack6.id, pack6.fid, pack6.name, null, 0));
                    break;
                case 21:
                    var pack4 = JsonConvert.DeserializeObject<FriendMessagePostSendEventPack>(data);
                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.friend, pack4.qq, 0, pack4.id, pack4.res, pack4.error, pack4.message));
                    break;
                case 28:
                    var pack5 = JsonConvert.DeserializeObject<GroupMessagePostSendEventPack>(data);
                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.group, pack5.qq, pack5.id, 0, pack5.res, pack5.error, pack5.message));
                    break;
                case 47:
                    var pack3 = JsonConvert.DeserializeObject<TempMessagePostSendEventPack>(data);
                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.private_, pack3.qq, pack3.id, pack3.fid, pack3.res, pack3.error, pack3.message));
                    break;
                case 49:
                    var pack = JsonConvert.DeserializeObject<GroupMessageEventPack>(data);
                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.group, pack.qq, pack.id, pack.fid, pack.name, pack.message));
                    break;
                case 50:
                    var pack1 = JsonConvert.DeserializeObject<TempMessageEventPack>(data);
                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.private_, pack1.qq, pack1.id, pack1.fid, pack1.name, pack1.message));
                    break;
                case 51:
                    var pack2 = JsonConvert.DeserializeObject<FriendMessageEventPack>(data);
                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.friend, pack2.qq, 0, pack2.id, pack2.name, pack2.message));
                    break;
                case 60:
                    break;
            }
        }

        private static void Log(LogType type, string data)
        {

            Console.WriteLine($"日志:{type} {data}");
        }

        private static void State(StateType type)
        {
            Console.WriteLine($"日志:{type}");
        }

        internal static void Start()
        {
            RobotConfig config = new()
            {
                IP = ServerMain.Config.Robot.IP,
                Port = ServerMain.Config.Robot.Port,
                Name = "ColoryrServer",
                Pack = new() { 21, 28, 35, 36, 47, 49, 50, 51 },
                RunQQ = 0,
                Time = 10000,
                CallAction = Message,
                LogAction = Log,
                StateAction = State
            };

            robot.Set(config);
            robot.Start();
        }
        
        internal static void SendGroupMessage(long qq, long id, List<string> message)
        {
            var data = BuildPack.Build(new SendGroupMessagePack 
            { 
                qq = qq, 
                id = id, 
                message = message 
            }, 52);
            robot.AddTask(data);
        }
        internal static void SendGroupPrivateMessage(long qq, long id, long fid, List<string> message)
        {
            var data = BuildPack.Build(new SendGroupPrivateMessagePack 
            { 
                qq = qq, 
                id = id, 
                fid = fid, 
                message = message 
            }, 53);
            robot.AddTask(data);
        }
        internal static void SendFriendMessage(long qq, long id, List<string> message)
        {
            var data = BuildPack.Build(new SendFriendMessagePack 
            { 
                qq = qq, 
                id = id, 
                message = message 
            }, 54);
            robot.AddTask(data);
        }
        internal static void SendGroupImage(long qq, long id, string img)
        {
            var data = BuildPack.BuildImage(qq, id, 0, img, 61);
            robot.AddTask(data);
        }
        internal static void SendGroupPrivateImage(long qq, long id, long fid, string img)
        {
            var data = BuildPack.BuildImage(qq, id, fid, img, 62);
            robot.AddTask(data);
        }
        internal static void SendFriendImage(long qq, long id, string img)
        {
            var data = BuildPack.BuildImage(qq, id, 0, img, 63);
            robot.AddTask(data);
        }

        internal static void SendGroupSound(long qq, long id, string sound)
        {
            var data = BuildPack.BuildSound(qq, id, sound, 74);
            robot.AddTask(data);
        }
        internal static void SendGroupImageFile(long qq, long id, string file)
        {
            var data = BuildPack.Build(new LoadFileSendToGroupImagePack 
            { 
                qq = qq, 
                id = id, 
                file = file, 
            }, 75);
            robot.AddTask(data);
        }
        internal static void SendGroupPrivateImageFile(long qq, long id, long fid, string file)
        {
            var data = BuildPack.Build(new LoadFileSendToGroupPrivateImagePack 
            { 
                qq = qq, 
                id = id, 
                fid = fid, 
                file = file, 
            }, 76);
            robot.AddTask(data);
        }
        internal static void SendFriendImageFile(long qq, long id, string file)
        {
            var data = BuildPack.Build(new LoadFileSendToFriendImagePack 
            { 
                qq = qq, 
                id = id, 
                file = file,
            }, 77);
            robot.AddTask(data);
        }
        internal static void SendGroupSoundFile(long qq, long id, string file)
        {
            var data = BuildPack.Build(new LoadFileSendToGroupSoundPack 
            { 
                qq = qq, 
                id = id, 
                file = file, 
            }, 78);
            robot.AddTask(data);
        }
        internal static void Stop()
        {
            ServerMain.LogOut("机器人正在断开");
            robot.Stop();
            ServerMain.LogOut("机器人已断开");
        }
        internal static void ReCall(string id)
        {
            var data = BuildPack.Build(new ReCallMessagePack 
            { 
                id = int.Parse(id) 
            }, 71);
            robot.AddTask(data);
        }
    }
}
