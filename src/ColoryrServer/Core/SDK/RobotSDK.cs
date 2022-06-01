using ColoryrServer.Core.Robot;
using System.Collections.Generic;

namespace ColoryrServer.SDK;

public class RobotAfter
{
    public enum MessageType
    {
        group, private_, friend, stranger
    }
    public long qq { get; private set; }
    public MessageType type { get; private set; }
    public long id { get; private set; }
    public long fid { get; private set; }
    public bool res { get; private set; }
    public string error { get; private set; }
    public string messageId { get; private set; }
    public List<string> message { get; private set; }
    public RobotSDK robot { get; private set; }
    /// <summary>
    /// 机器人发送消息后回调
    /// </summary>
    /// <param name="type">消息类型</param>
    /// <param name="qq">QQ机器人账户</param>
    /// <param name="id">群号</param>
    /// <param name="fid">QQ号</param>
    /// <param name="res">是否发送成功</param>
    /// <param name="message">消息</param>
    /// <param name="robot">机器人</param>
    public RobotAfter(MessageType type, long qq, long id, long fid, bool res, string error, List<string> message, RobotSDK robot)
    {
        this.qq = qq;
        this.type = type;
        this.id = id;
        this.fid = fid;
        this.res = res;
        this.error = error;
        this.message = message;
        this.robot = robot;
        messageId = Tools.GetString(message[0], "source:", ",");
    }
    /// <summary>
    /// 撤回消息
    /// </summary>
    public void ReCall()
        => RobotUtils.ReCall(messageId);
}
public class RobotRequest
{
    public enum MessageType
    {
        group, private_, friend, stranger
    }
    public long qq { get; private set; }
    public MessageType mtype { get; private set; }
    public long id { get; private set; }
    public long fid { get; private set; }
    public string name { get; private set; }
    public List<string> message { get; private set; }
    public string messageId { get; private set; }
    public RobotSDK robot { get; private set; }
    /// <summary>
    /// 机器人请求
    /// </summary>
    /// <param name="type">消息类型</param>
    /// <param name="qq">QQ机器人账户</param>
    /// <param name="id">群号</param>
    /// <param name="fid">QQ号</param>
    /// <param name="name">名字</param>
    /// <param name="message">消息</param>
    /// <param name="robot">机器人</param>
    public RobotRequest(MessageType type, long qq, long id, long fid, string name, List<string> message, RobotSDK robot)
    {
        this.qq = qq;
        this.mtype = type;
        this.id = id;
        this.fid = fid;
        this.name = name;
        this.message = message;
        this.robot = robot;
        if (message != null && message.Count != 0)
            messageId = Tools.GetString(message[0], "source:", ",");
    }
    /// <summary>
    /// 撤回消息
    /// </summary>
    public void ReCall()
        => RobotUtils.ReCall(messageId);
    /// <summary>
    /// 发送消息回应
    /// </summary>
    /// <param name="message">消息</param>
    public void SendMessage(List<string> message)
    {
        switch (mtype)
        {
            case MessageType.group:
                robot.SendGroupMessage(qq, id, message);
                break;
            case MessageType.private_:
                robot.SendGroupTempMessage(qq, id, fid, message);
                break;
            case MessageType.friend:
                robot.SendFriendMessage(qq, fid, message);
                break;
            case MessageType.stranger:
                robot.SendStrangerMessage(qq, fid, message);
                break;
        }
    }

    /// <summary>
    /// 发送本地文件回应
    /// </summary>
    /// <param name="file">文件名</param>
    public void SendImageFile(string file)
    {
        switch (mtype)
        {
            case MessageType.group:
                robot.SendGroupImageFile(qq, id, file);
                break;
            case MessageType.private_:
                robot.SendGroupPrivateImageFile(qq, id, fid, file);
                break;
            case MessageType.friend:
                robot.SendFriendImageFile(qq, fid, file);
                break;
            case MessageType.stranger:
                robot.SendStrangerImageFile(qq, fid, file, null);
                break;
        }
    }

    /// <summary>
    /// 发送本地声音文件回复
    /// </summary>
    /// <param name="sound">文件名</param>
    public void SendSoundFile(string file)
    {
        switch (mtype)
        {
            case MessageType.friend:
                robot.SendFriendSoundFile(qq, id, file);
                break;
            case MessageType.group:
                robot.SendGroupSoundFile(qq, id, file);
                break;
            case MessageType.stranger:
                robot.SendStrangerSoundFile(qq, id, file, null);
                break;
        }
    }
}
public class RobotEvent 
{
    public int Type { get; init; }
    public PackBase Pack { get; init; }
    public RobotSDK Robot { get; init; }
    /// <summary>
    /// 机器人事件
    /// </summary>
    /// <param name="type">QQ机器人账户</param>
    /// <param name="pack">群号</param>
    /// <param name="robot">机器人</param>
    public RobotEvent(int type, PackBase pack, RobotSDK robot) 
    {
        Type = type;
        Pack = pack;
        Robot = robot;
    }
}
