using ColoryrServer.Core.DllManager;
using ColoryrServer.SDK;
using System.Collections.Generic;

namespace ColoryrServer.Core.Robot;
public static class RobotUtils
{
    public static RobotSDK Robot { get; private set; }
    private static void Message(byte type, object data)
    {
        switch (type)
        {
            case 21:
                var pack4 = data as FriendMessagePostSendEventPack;
                DllRun.RobotGo(new RobotSend(RobotSend.MessageType.friend, pack4.qq, 0, pack4.id, pack4.res, pack4.error, pack4.message, Robot));
                break;
            case 28:
                var pack5 = data as GroupMessagePostSendEventPack;
                DllRun.RobotGo(new RobotSend(RobotSend.MessageType.group, pack5.qq, pack5.id, 0, pack5.res, pack5.error, pack5.message, Robot));
                break;
            case 47:
                var pack3 = data as TempMessagePostSendEventPack;
                DllRun.RobotGo(new RobotSend(RobotSend.MessageType.private_, pack3.qq, pack3.id, pack3.fid, pack3.res, pack3.error, pack3.message, Robot));
                break;
            case 49:
                var pack = data as GroupMessageEventPack;
                DllRun.RobotGo(new RobotMessage(RobotMessage.MessageType.group, pack.qq, pack.id, pack.fid, pack.name, pack.message, Robot));
                break;
            case 50:
                var pack1 = data as TempMessageEventPack;
                DllRun.RobotGo(new RobotMessage(RobotMessage.MessageType.private_, pack1.qq, pack1.id, pack1.fid, pack1.name, pack1.message, Robot));
                break;
            case 51:
                var pack2 = data as FriendMessageEventPack;
                DllRun.RobotGo(new RobotMessage(RobotMessage.MessageType.friend, pack2.qq, 0, pack2.id, pack2.name, pack2.message, Robot));
                break;
            case 60:
                break;
            case 116:
                var pack8 = data as StrangerMessageEventPack;
                DllRun.RobotGo(new RobotMessage(RobotMessage.MessageType.stranger, pack8.qq, 0, pack8.id, pack8.name, pack8.message, Robot));
                break;
            case 123:
                var pack9 = data as StrangerMessagePostSendEventPack;
                DllRun.RobotGo(new RobotSend(RobotSend.MessageType.stranger, pack9.qq, pack9.id, 0, pack9.res, pack9.error, pack9.message, Robot));
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
            IP = ServerMain.Config.Robot.Socket.IP,
            Port = ServerMain.Config.Robot.Socket.Port,
            Name = "ColoryrServer",
            Pack = new() { 21, 28, 47, 49, 50, 51, 116, 123 },
            RunQQ = 0,
            Time = 10000,
            Check = true,
            CallAction = Message,
            LogAction = Log,
            StateAction = State
        };

        foreach (var item in ServerMain.Config.Robot.Packs)
        {
            byte temp = (byte)item;
            if (!config.Pack.Contains(temp))
                config.Pack.Add(temp);
        }

        Robot = new();
        Robot.Set(config);
        Robot.SetPipe(new ColorMiraiNetty(Robot));
        Robot.Start();

        ServerMain.OnStop += Stop;
    }

    public static List<long> GetQQs()
        => Robot.QQs;

    private static void Stop()
    {
        ServerMain.LogOut("机器人正在断开");
        Robot.Stop();
        ServerMain.LogOut("机器人已断开");
    }
    internal static void ReCall(string id)
    {
        Robot.AddSend(new ReCallMessagePack
        {
            id = int.Parse(id)
        }, 71);
    }
}
