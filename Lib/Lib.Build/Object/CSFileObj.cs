using System.Collections.Generic;

namespace Lib.Build.Object
{
    public enum CodeType
    {
        Dll, Class, IoT, WebSocket, Robot
    }
    public class CSFileObj
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string UUID { get; set; }
        /// <summary>
        /// 注释
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

    }
    public class CSFileCode : CSFileObj
    {
        public CSFileCode(CSFileObj obj)
        {
            UUID = obj.UUID;
            Text = obj.Text;
            User = obj.User;
        }
        public CSFileCode()
        {
        }
        public string Code { get; set; }
        public CodeType Type { get; set; }
    }
    public class CSFileList
    {
        public Dictionary<string, CSFileObj> list { get; set; }
    }
}
