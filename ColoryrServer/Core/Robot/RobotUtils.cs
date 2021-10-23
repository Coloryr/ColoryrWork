using ColoryrServer.SDK;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ColoryrServer.Robot
{
    public class RobotUtils
    {
        private static RobotSDK robot = new();
        private static void Message(byte type, object data)
        {
            switch (type)
            {
                case 35:
                    var pack7 = data as InviteMemberJoinEventPack;
                    DllManager.DllRun.RobotGo(new RobotEvent(RobotEvent.EventType.GroupMemberJoin, pack7.qq, pack7.id, pack7.fid, pack7.name, null, 0, robot));
                    break;
                case 36:
                    var pack6 = data as MemberJoinEventPack;
                    DllManager.DllRun.RobotGo(new RobotEvent(RobotEvent.EventType.GroupMemberJoin, pack6.qq, pack6.id, pack6.fid, pack6.name, null, 0, robot));
                    break;
                case 21:
                    var pack4 = data as FriendMessagePostSendEventPack;
                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.friend, pack4.qq, 0, pack4.id, pack4.res, pack4.error, pack4.message, robot));
                    break;
                case 28:
                    var pack5 = data as GroupMessagePostSendEventPack;
                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.group, pack5.qq, pack5.id, 0, pack5.res, pack5.error, pack5.message, robot));
                    break;
                case 47:
                    var pack3 = data as TempMessagePostSendEventPack;
                    DllManager.DllRun.RobotGo(new RobotAfter(RobotAfter.MessageType.private_, pack3.qq, pack3.id, pack3.fid, pack3.res, pack3.error, pack3.message, robot));
                    break;
                case 49:
                    var pack = data as GroupMessageEventPack;
                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.group, pack.qq, pack.id, pack.fid, pack.name, pack.message, robot));
                    break;
                case 50:
                    var pack1 = data as TempMessageEventPack;
                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.private_, pack1.qq, pack1.id, pack1.fid, pack1.name, pack1.message, robot));
                    break;
                case 51:
                    var pack2 = data as FriendMessageEventPack;
                    DllManager.DllRun.RobotGo(new RobotRequest(RobotRequest.MessageType.friend, pack2.qq, 0, pack2.id, pack2.name, pack2.message, robot));
                    break;
                case 60:
                    break;
            }
        }

        private static void Log(LogType type, string data)
        {
            ServerMain.LogOut($"机器人:{type} {data}");
        }

        private static void State(StateType type)
        {
            ServerMain.LogOut($"机器人:{type}");
        }

        public static void Start()
        {
            RobotConfig config = new()
            {
                IP = ServerMain.Config.Robot.IP,
                Port = ServerMain.Config.Robot.Port,
                Name = "ColoryrServer",
                Pack = new() { 21, 28, 35, 36, 47, 49, 50, 51 },
                RunQQ = 0,
                Time = 10000,
                Check = true,
                CallAction = Message,
                LogAction = Log,
                StateAction = State
            };

            robot.Set(config);
            robot.Start();
        }

        public static void Stop()
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
        internal static void AddTask(byte[] data)
        {
            robot.AddTask(data);
        }
    }
}
