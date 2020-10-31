using System.Collections.Generic;

namespace Lib.Build.Object
{
    public enum CodeType
    {
        Dll, Class, IoT, WebSocket, Robot, App, Mcu
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
        public CSFileList()
        {
            list = new Dictionary<string, CSFileObj>();
        }
        public Dictionary<string, CSFileObj> list { get; set; }
    }

    public class AppFileObj : CSFileObj
    {
        public Dictionary<string, string> Codes { get; set; }
        public Dictionary<string, string> Xamls { get; set; }
        public string Key { get; set; }
        public CodeType Type { get; }
        public AppFileObj(CSFileObj obj)
        {
            UUID = obj.UUID;
            Text = obj.Text;
            User = obj.User;
            Codes = new Dictionary<string, string>();
            Xamls = new Dictionary<string, string>();
            Type = CodeType.App;
        }
        public AppFileObj()
        {
            Codes = new Dictionary<string, string>();
            Xamls = new Dictionary<string, string>();
            Type = CodeType.App;
        }
    }

    public class McuFileObj : CSFileObj
    {
        public Dictionary<string, string> Codes { get; set; }
        public string Key { get; set; }
        public CodeType Type { get; }
        public McuFileObj(CSFileObj obj)
        {
            UUID = obj.UUID;
            Text = obj.Text;
            User = obj.User;
            Codes = new Dictionary<string, string>();
            Type = CodeType.Mcu;
        }
        public McuFileObj()
        {
            Codes = new Dictionary<string, string>();
            Type = CodeType.Mcu;
        }
    }
}
