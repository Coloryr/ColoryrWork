using ColoryrSDK;
using System.Collections.Generic;

namespace ColoryrServer.SDK;

public record MessageKey
{
    public int[] Ids1 { get; set; }
    public int[] Ids2 { get; set; }
}
/// <summary>
/// 机器人发送消息事件
/// </summary>
public class RobotSend
{
    public long QQ { get; init; }
    public MessageSourceKind Type { get; init; }
    public long Id { get; init; }
    public long Fid { get; init; }
    public bool Res { get; init; }
    public string Error { get; init; }
    public MessageKey MessageId { get; init; }
    public List<string> Message { get; init; }
    public RobotSDK Robot { get; init; }

    /// <summary>
    /// 撤回消息
    /// </summary>
    public void ReCall()
        => Robot.ReCallMessage(QQ, MessageId.Ids1, MessageId.Ids2, Type);
}
/// <summary>
/// 机器人消息事件
/// </summary>
public class RobotMessage
{
    public long QQ { get; init; }
    public MessageSourceKind Type { get; init; }
    public long Id { get; init; }
    public long Fid { get; init; }
    public string Name { get; init; }
    public List<string> Message { get; init; }
    public MessageKey MessageId { get; init; }
    public MemberPermission Permission { get; init; }
    public RobotSDK Robot { get; init; }

    /// <summary>
    /// 撤回消息
    /// </summary>
    public void ReCall()
        => Robot.ReCallMessage(QQ, MessageId.Ids1, MessageId.Ids2, Type);

    /// <summary>
    /// 发送消息回应
    /// </summary>
    /// <param name="message">消息</param>
    public void SendMessage(string message)
    {
        SendMessage(new List<string> { message });
    }
    /// <summary>
    /// 发送消息回应
    /// </summary>
    /// <param name="message">消息</param>
    public void SendMessage(List<string> message)
    {
        switch (Type)
        {
            case MessageSourceKind.GROUP:
                Robot.SendGroupMessage(QQ, Id, message);
                break;
            case MessageSourceKind.TEMP:
                Robot.SendGroupTempMessage(QQ, Id, Fid, message);
                break;
            case MessageSourceKind.FRIEND:
                Robot.SendFriendMessage(QQ, Fid, message);
                break;
            case MessageSourceKind.STRANGER:
                Robot.SendStrangerMessage(QQ, Fid, message);
                break;
        }
    }

    /// <summary>
    /// 发送本地文件回应
    /// </summary>
    /// <param name="file">文件名</param>
    public void SendImageFile(string file)
    {
        switch (Type)
        {
            case MessageSourceKind.GROUP:
                Robot.SendGroupImageFile(QQ, Id, file);
                break;
            case MessageSourceKind.TEMP:
                Robot.SendGroupPrivateImageFile(QQ, Id, Fid, file);
                break;
            case MessageSourceKind.FRIEND:
                Robot.SendFriendImageFile(QQ, Fid, file);
                break;
            case MessageSourceKind.STRANGER:
                Robot.SendStrangerImageFile(QQ, Fid, file, null);
                break;
        }
    }

    /// <summary>
    /// 发送本地声音文件回复
    /// </summary>
    /// <param name="sound">文件名</param>
    public void SendSoundFile(string file)
    {
        switch (Type)
        {
            case MessageSourceKind.FRIEND:
                Robot.SendFriendSoundFile(QQ, Id, file);
                break;
            case MessageSourceKind.GROUP:
                Robot.SendGroupSoundFile(QQ, Id, file);
                break;
            case MessageSourceKind.STRANGER:
                Robot.SendStrangerSoundFile(QQ, Id, file, null);
                break;
            default:
                throw new ErrorDump("不支持的操作");
        }
    }
}
/// <summary>
/// 机器人事件
/// </summary>
public class RobotEvent
{
    public int Type { get; init; }
    public PackBase Pack { get; init; }
    public RobotSDK Robot { get; init; }
}
