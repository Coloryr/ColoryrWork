using System.Collections.Generic;

namespace Lib.Build.Object
{
    public enum CodeType
    {
        Dll, Class, IoT, WebSocket, Robot, App, Mqtt, Task, Web
    }
    public record CSFileObj
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
        /// 版本
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public string UpdataTime { get; set; }

    }
    public record CSFileCode : CSFileObj
    {
        public CSFileCode(CSFileObj obj)
        {
            UUID = obj.UUID;
            Text = obj.Text;
        }
        public CSFileCode()
        {
        }
        public string Code { get; set; }
        public CodeType Type { get; set; }
    }
    public record WebObj : CSFileObj
    { 
        public Dictionary<string, string> Codes { get; set; }
        public Dictionary<string, string> Files { get; set; }
    }
    public record AppTempFileObj : CSFileObj
    {
        public string Key { get; set; }
    }
    public record CSFileList
    {
        public Dictionary<string, CSFileObj> List { get; set; } = new();
    }
    public record AppFileList
    {
        public Dictionary<string, AppTempFileObj> List { get; set; } = new();
    }

    public record AppFileObj : AppTempFileObj
    {
        public Dictionary<string, string> Codes { get; set; }
        public Dictionary<string, string> Xamls { get; set; }
        public Dictionary<string, string> Files { get; set; }
        public CodeType Type { get; set; }
        public AppFileObj(CSFileObj obj)
        {
            UUID = obj.UUID;
            Text = obj.Text;
            Codes = new();
            Xamls = new();
            Files = new();
            Type = CodeType.App;
        }
        public AppFileObj()
        {
            Codes = new();
            Xamls = new();
            Files = new();
            Type = CodeType.App;
        }
    }
}
