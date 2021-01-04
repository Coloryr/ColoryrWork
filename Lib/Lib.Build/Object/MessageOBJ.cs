namespace Lib.Build.Object
{
    public record ReMessage
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
    public record ReBuild
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
    public record GetOBJ
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
    public record GetMeesage
    {
        /// <summary>
        /// 结果
        /// </summary>
        public int Res { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }
}
