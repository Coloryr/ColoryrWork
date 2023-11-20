using ColoryrSDK;
using ColoryrServer.Core.Dll;
using ColoryrServer.SDK;
using System.Collections.Generic;

namespace ColoryrServer.Core.Robot;

public static class RobotUtils
{
    public static RobotSDK Robot { get; private set; }
    private static void Message(int type, object data)
    {
        if (ServerMain.Config.FixMode)
            return;
        switch (type)
        {
            case 21:
                {
                    var pack = data as FriendMessagePostSendEventPack;
                    DllRun.RobotGo(new RobotSend
                    {
                        Type = MessageSourceKind.FRIEND,
                        QQ = pack.qq,
                        Id = 0,
                        Fid = pack.id,
                        Res = pack.res,
                        Error = pack.error,
                        Message = pack.message,
                        Robot = Robot,
                        MessageId = new MessageKey
                        {
                            Ids1 = pack.ids1,
                            Ids2 = pack.ids2
                        }
                    });
                    break;
                }
            case 28:
                {
                    var pack = data as GroupMessagePostSendEventPack;
                    DllRun.RobotGo(new RobotSend
                    {
                        Type = MessageSourceKind.GROUP,
                        QQ = pack.qq,
                        Id = pack.id,
                        Fid = 0,
                        Res = pack.res,
                        Error = pack.error,
                        Message = pack.message,
                        Robot = Robot,
                        MessageId = new MessageKey
                        {
                            Ids1 = pack.ids1,
                            Ids2 = pack.ids2
                        }
                    });
                    break;
                }
            case 47:
                {
                    var pack = data as TempMessagePostSendEventPack;
                    DllRun.RobotGo(new RobotSend
                    {
                        Type = MessageSourceKind.TEMP,
                        QQ = pack.qq,
                        Id = pack.id,
                        Fid = pack.fid,
                        Res = pack.res,
                        Error = pack.error,
                        Message = pack.message,
                        Robot = Robot,
                        MessageId = new MessageKey
                        {
                            Ids1 = pack.ids1,
                            Ids2 = pack.ids2
                        }
                    });
                    break;
                }
            case 49:
                {
                    var pack = data as GroupMessageEventPack;
                    DllRun.RobotGo(new RobotMessage
                    {
                        Type = MessageSourceKind.GROUP,
                        QQ = pack.qq,
                        Id = pack.id,
                        Fid = pack.fid,
                        Name = pack.name,
                        Message = pack.message,
                        Robot = Robot,
                        MessageId = new MessageKey
                        {
                            Ids1 = pack.ids1,
                            Ids2 = pack.ids2
                        },
                        Permission = pack.permission
                    });
                    break;
                }
            case 50:
                {
                    var pack = data as TempMessageEventPack;
                    DllRun.RobotGo(new RobotMessage
                    {
                        Type = MessageSourceKind.TEMP,
                        QQ = pack.qq,
                        Id = pack.id,
                        Fid = pack.fid,
                        Name = pack.name,
                        Message = pack.message,
                        Robot = Robot,
                        MessageId = new MessageKey
                        {
                            Ids1 = pack.ids1,
                            Ids2 = pack.ids2
                        },
                        Permission = pack.permission
                    });
                    break;
                }
            case 51:
                {
                    var pack = data as FriendMessageEventPack;
                    DllRun.RobotGo(new RobotMessage
                    {
                        Type = MessageSourceKind.FRIEND,
                        QQ = pack.qq,
                        Id = 0,
                        Fid = pack.id,
                        Name = pack.name,
                        Message = pack.message,
                        Robot = Robot,
                        MessageId = new MessageKey
                        {
                            Ids1 = pack.ids1,
                            Ids2 = pack.ids2
                        }
                    });
                    break;
                }
            case 60:
                break;
            case 116:
                {
                    var pack = data as StrangerMessageEventPack;
                    DllRun.RobotGo(new RobotMessage
                    {
                        Type = MessageSourceKind.STRANGER,
                        QQ = pack.qq,
                        Id = 0,
                        Fid = pack.id,
                        Name = pack.name,
                        Message = pack.message,
                        Robot = Robot,
                        MessageId = new MessageKey
                        {
                            Ids1 = pack.ids1,
                            Ids2 = pack.ids2
                        }
                    });
                    break;
                }
            case 123:
                {
                    var pack = data as StrangerMessagePostSendEventPack;
                    DllRun.RobotGo(new RobotSend
                    {
                        Type = MessageSourceKind.STRANGER,
                        QQ = pack.qq,
                        Id = pack.id,
                        Fid = 0,
                        Res = pack.res,
                        Error = pack.error,
                        Message = pack.message,
                        Robot = Robot,
                        MessageId = new MessageKey
                        {
                            Ids1 = pack.ids1,
                            Ids2 = pack.ids2
                        }
                    });
                }
                break;
            default:
                DllRun.RobotGo(new RobotEvent
                {
                    Type = type,
                    Pack = data as PackBase,
                    Robot = Robot
                });
                break;
        }
    }

    private static void Log(LogType type, string data)
    {
        switch (type)
        {
            case LogType.Log:
                ServerMain.LogOut($"[ColorMirai]{data}");
                break;
            case LogType.Error:
                ServerMain.LogWarn($"[ColorMirai]{data}");
                break;
        }

    }

    private static void State(StateType type)
    {
        ServerMain.LogOut($"机器人链接状态切换至{type}");
    }

    public static void Start()
    {
        RobotConfig config = new()
        {
            IP = ServerMain.Config.Robot.Socket.IP,
            Port = ServerMain.Config.Robot.Socket.Port,
            Name = "ColoryrServer",
            Pack = new() { },
            RunQQ = 0,
            Time = 10000,
            Check = true,
            CallAction = Message,
            LogAction = Log,
            StateAction = State
        };

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
}
