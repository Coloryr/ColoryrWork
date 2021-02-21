using System.Collections.Generic;

namespace ColoryrServer.SDK
{
    public class INOUT
    {
        /// <summary>
        /// 输入参数
        /// </summary>
        public Dictionary<string, string> Input = new();
        /// <summary>
        /// 输出参数
        /// </summary>
        public Dictionary<string, string> Output = new();
    }
    public class NotesSDK
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 说明
        /// </summary>
        public string Text;
        /// <summary>
        /// 函数说明
        /// </summary>
        public Dictionary<string, INOUT> Function = new();
    }
}
