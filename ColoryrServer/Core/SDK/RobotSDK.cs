using ColoryrServer.Robot;
using System;
using System.Collections.Generic;

namespace ColoryrServer.SDK
{
    public class RobotAfter
    {
        public enum MessageType
        {
            group, private_, friend
        }
        public long qq { get; private set; }
        public MessageType type { get; private set; }
        public long id { get; private set; }
        public long fid { get; private set; }
        public bool res { get; private set; }
        public string error { get; private set; }
        public string messageId { get; private set; }
        public List<string> message { get; private set; }
        /// <summary>
        /// 机器人发送消息后回调
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="qq">QQ机器人账户</param>
        /// <param name="id">群号</param>
        /// <param name="fid">QQ号</param>
        /// <param name="res">是否发送成功</param>
        /// <param name="message">消息</param>
        public RobotAfter(MessageType type, long qq, long id, long fid, bool res, string error, List<string> message)
        {
            this.qq = qq;
            this.type = type;
            this.id = id;
            this.fid = fid;
            this.res = res;
            this.error = error;
            this.message = message;
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
            group, private_, friend
        }
        public long qq { get; private set; }
        public MessageType type { get; private set; }
        public long id { get; private set; }
        public long fid { get; private set; }
        public string name { get; private set; }
        public List<string> message { get; private set; }
        public string messageId { get; private set; }
        /// <summary>
        /// 机器人请求
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="qq">QQ机器人账户</param>
        /// <param name="id">群号</param>
        /// <param name="fid">QQ号</param>
        /// <param name="name">名字</param>
        /// <param name="message">消息</param>
        public RobotRequest(MessageType type, long qq, long id, long fid, string name, List<string> message)
        {
            this.qq = qq;
            this.type = type;
            this.id = id;
            this.fid = fid;
            this.name = name;
            this.message = message;
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
            switch (type)
            {
                case MessageType.group:
                    RobotUtils.SendGroupMessage(qq, id, message);
                    break;
                case MessageType.private_:
                    RobotUtils.SendGroupPrivateMessage(qq, id, fid, message);
                    break;
                case MessageType.friend:
                    RobotUtils.SendFriendMessage(qq, fid, message);
                    break;
            }
        }
        /// <summary>
        /// 发送图片回应
        /// </summary>
        /// <param name="img">图片二进制</param>
        public void SendImage(byte[] img)
        {
            string data = Convert.ToBase64String(img);
            switch (type)
            {
                case MessageType.group:
                    RobotUtils.SendGroupImage(qq, id, data);
                    break;
                case MessageType.private_:
                    RobotUtils.SendGroupPrivateImage(qq, id, fid, data);
                    break;
                case MessageType.friend:
                    RobotUtils.SendFriendImage(qq, fid, data);
                    break;
            }
        }

        /// <summary>
        /// 发送本地文件回应
        /// </summary>
        /// <param name="file">文件名</param>
        public void SendImageFile(string file)
        {
            switch (type)
            {
                case MessageType.group:
                    RobotUtils.SendGroupImageFile(qq, id, file);
                    break;
                case MessageType.private_:
                    RobotUtils.SendGroupPrivateImageFile(qq, id, fid, file);
                    break;
                case MessageType.friend:
                    RobotUtils.SendFriendImageFile(qq, fid, file);
                    break;
            }
        }

        /// <summary>
        /// 发送声音回应
        /// </summary>
        /// <param name="sound">音频二进制</param>
        public void SendSound(byte[] sound)
        {
            string data = Convert.ToBase64String(sound);
            switch (type)
            {
                case MessageType.group:
                    RobotUtils.SendGroupSound(qq, id, data);
                    break;
            }
        }

        /// <summary>
        /// 发送本地声音文件回复
        /// </summary>
        /// <param name="sound">文件名</param>
        public void SendSoundFile(string file)
        {
            switch (type)
            {
                case MessageType.group:
                    RobotUtils.SendGroupSoundFile(qq, id, file);
                    break;
            }
        }
    }
    public class RobotEvent
    {
        public enum EventType
        {
            GroupMemberJoin, GroupMemberQuit, GroupMemberKick
        };
        public long qq { get; private set; }
        public long id { get; private set; }
        public long fid { get; private set; }
        public string name { get; private set; }
        public string oname { get; private set; }
        public long oid { get; private set; }
        public EventType type { get; private set; }
        private RobotRequest RobotRequest;
        /// <summary>
        /// 机器人事件
        /// </summary>
        /// <param name="qq">QQ机器人账户</param>
        /// <param name="id">群号</param>
        /// <param name="fid">用户QQ号</param>
        /// <param name="name">用户昵称</param>
        /// <param name="oname">管理者昵称</param>
        /// <param name="oid">管理者QQ号</param>
        /// <param name="type">事件类型</param>
        public RobotEvent(EventType type, long qq, long id, long fid, string name, string oname, long oid)
        {
            this.qq = qq;
            this.id = id;
            this.fid = fid;
            this.name = name;
            this.oname = oname;
            this.oid = oid;
            this.type = type;
            RobotRequest = new RobotRequest(RobotRequest.MessageType.group, qq, id, fid, null, null);
        }
        /// <summary>
        /// 发送消息回应
        /// </summary>
        /// <param name="message">消息</param>
        public void SendMessage(List<string> message)
            => RobotRequest.SendMessage(message);
        /// <summary>
        /// 发送图片回应
        /// </summary>
        /// <param name="img">图片二进制</param>
        public void SendImage(byte[] img)
            => RobotRequest.SendImage(img);

        /// <summary>
        /// 发送声音回应
        /// </summary>
        /// <param name="sound">音频二进制</param>
        public void SendSound(byte[] sound)
            => RobotRequest.SendSound(sound);
    }

    public class Robot
    {
        /// <summary>
        /// 向机器人发送一个数据包
        /// </summary>
        /// <param name="data">数据包</param>
        public void AddTask(byte[] data)
            => RobotUtils.AddTask(data);
    }
}
