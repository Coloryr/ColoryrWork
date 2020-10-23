using System.Collections.Generic;

namespace ColoryrServer.Robot
{
    abstract class PackBase
    {
        public long qq { get; set; }
    }
    class PackStart
    {
        public string Name { get; set; }
        public List<byte> Reg { get; set; }
    }
    class GroupMessageEventPack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string name { get; set; }
        public List<string> message { get; set; }
    }

    class SendGroupMessagePack : PackBase
    {
        public long id { get; set; }
        public List<string> message { get; set; }
    }
    class SendFriendMessagePack : PackBase
    {
        public long id { get; set; }
        public List<string> message { get; set; }
    }
    class SendGroupPrivateMessagePack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public List<string> message { get; set; }
    }
    class TempMessageEventPack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string name { get; set; }
        public List<string> message { get; set; }
        public int time { get; set; }
    }
    class FriendMessageEventPack : PackBase
    {
        public long id { get; set; }
        public string name { get; set; }
        public List<string> message { get; set; }
        public int time { get; set; }
    }
    class SendGroupImagePack : PackBase
    {
        public long id { get; set; }
        public string img { get; set; }
    }
    class SendGroupPrivateImagePack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string img { get; set; }
    }
    class SendFriendImagePack : PackBase
    {
        public long id { get; set; }
        public string img { get; set; }
    }
    class GroupMessagePostSendEventPack : PackBase
    {
        public long id { get; set; }
        public bool res { get; set; }
        public List<string> message { get; set; }
        public string error { get; set; }
    }
    class ReCallMessage
    {
        public long id { get; set; }
    }
    class FriendMessagePostSendEventPack : PackBase
    {
        public List<string> message { get; set; }
        public long id { get; set; }
        public string name { get; set; }
        public bool res { get; set; }
        public string error { get; set; }
    }
    class TempMessagePostSendEventPack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public bool res { get; set; }
        public List<string> message { get; set; }
        public string error { get; set; }
    }
    class MemberJoinEventAPack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string name { get; set; }
    }
    class LoadFileSendToGroupImagePack : PackBase
    {
        public long id { get; set; }
        public string file { get; set; }
    }
    class LoadFileSendToGroupPrivateImagePack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string file { get; set; }
    }
    class LoadFileSendToFriendImagePack : PackBase
    {
        public long id { get; set; }
        public string file { get; set; }
    }
    class LoadFileSendToGroupSoundPack : PackBase
    {
        public long id { get; set; }
        public string file { get; set; }
    }
}
