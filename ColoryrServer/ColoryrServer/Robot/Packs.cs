using System.Collections.Generic;

namespace ColoryrServer.Robot
{
    abstract record PackBase
    {
        public long qq { get; set; }
    }
    record PackStart
    {
        public string Name { get; set; }
        public List<byte> Reg { get; set; }
    }
    record GroupMessageEventPack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string name { get; set; }
        public List<string> message { get; set; }
    }

    record SendGroupMessagePack : PackBase
    {
        public long id { get; set; }
        public List<string> message { get; set; }
    }
    record SendFriendMessagePack : PackBase
    {
        public long id { get; set; }
        public List<string> message { get; set; }
    }
    record SendGroupPrivateMessagePack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public List<string> message { get; set; }
    }
    record TempMessageEventPack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string name { get; set; }
        public List<string> message { get; set; }
        public int time { get; set; }
    }
    record FriendMessageEventPack : PackBase
    {
        public long id { get; set; }
        public string name { get; set; }
        public List<string> message { get; set; }
        public int time { get; set; }
    }
    record SendGroupImagePack : PackBase
    {
        public long id { get; set; }
        public string img { get; set; }
    }
    record SendGroupPrivateImagePack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string img { get; set; }
    }
    record SendFriendImagePack : PackBase
    {
        public long id { get; set; }
        public string img { get; set; }
    }
    record GroupMessagePostSendEventPack : PackBase
    {
        public long id { get; set; }
        public bool res { get; set; }
        public List<string> message { get; set; }
        public string error { get; set; }
    }
    record ReCallMessage
    {
        public long id { get; set; }
    }
    record FriendMessagePostSendEventPack : PackBase
    {
        public List<string> message { get; set; }
        public long id { get; set; }
        public string name { get; set; }
        public bool res { get; set; }
        public string error { get; set; }
    }
    record TempMessagePostSendEventPack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public bool res { get; set; }
        public List<string> message { get; set; }
        public string error { get; set; }
    }
    record MemberJoinEventAPack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string name { get; set; }
    }
    record LoadFileSendToGroupImagePack : PackBase
    {
        public long id { get; set; }
        public string file { get; set; }
    }
    record LoadFileSendToGroupPrivateImagePack : PackBase
    {
        public long id { get; set; }
        public long fid { get; set; }
        public string file { get; set; }
    }
    record LoadFileSendToFriendImagePack : PackBase
    {
        public long id { get; set; }
        public string file { get; set; }
    }
    record LoadFileSendToGroupSoundPack : PackBase
    {
        public long id { get; set; }
        public string file { get; set; }
    }
}
