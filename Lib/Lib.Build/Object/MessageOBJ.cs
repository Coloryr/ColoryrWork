namespace Lib.Build.Object
{
    public class ReMessage
    {
        /// <summary>
        /// 编译结果
        /// </summary>
        public bool Build { get; set; }
        /// <summary>
        /// 编译返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 编译用时
        /// </summary>
        public string UseTime { get; set; }
    }
    public class ReBuild
    {
        /// <summary>
        /// 应用名字
        /// </summary>
        public string Uuid { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }
    }
    public class GetOBJ
    {
        /// <summary>
        /// 应用名字
        /// </summary>
        public string Uuid { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object Get { get; set; }
    }
    public class GetMeesage
    {
        /// <summary>
        /// 结果
        /// </summary>
        public string res { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object data { get; set; }
    }
}
