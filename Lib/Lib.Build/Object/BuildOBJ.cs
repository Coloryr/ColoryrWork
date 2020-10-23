namespace Lib.Build.Object
{
    public enum ReType
    {
        BuildDll, BuildClass, BuildIoT, BuildWebSocket, BuildRobot,
        Login, CheckLogin, GetApi,
        GetDll, GetClass, GetIoT, GetWebSocket, GetRobot,
        CodeDll, CodeClass, CodeIoT, CodeWebSocket, CodeRobot,
        RemoveDll, RemoveClass, RemoveIoT, RemoveWebSocket, RemoveRobot
    }
    class BuildOBJ
    {
        public string UUID { get; set; }
        public string Code { get; set; }
        public string Token { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public ReType Mode { get; set; }
        public int Version { get; set; }
    }
}
