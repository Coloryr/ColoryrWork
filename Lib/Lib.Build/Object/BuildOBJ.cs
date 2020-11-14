namespace Lib.Build.Object
{
    public enum ReType
    {
        AddDll, AddClass, AddIoT, AddWebSocket, AddRobot,
        Login, CheckLogin, GetApi,
        GetDll, GetClass, GetIoT, GetWebSocket, GetRobot,
        CodeDll, CodeClass, CodeIoT, CodeWebSocket, CodeRobot,
        RemoveDll, RemoveClass, RemoveIoT, RemoveWebSocket, RemoveRobot,
        UpdataDll, UpdataClass, UpdataIoT, UpdataWebSocket, UpdataRobot,
        AddApp, RemoveApp, AddAppCS, AddAppXaml, RemoveAppCS, RemoveAppXaml,
        AddMcu, RemoveMcu, AddMcuLua, RemoveMcuLua
    }
    abstract class BuildPackBase
    {
        public string Token { get; set; }
        public string User { get; set; }
        public ReType Mode { get; set; }
    }
    class BuildOBJ : BuildPackBase
    {
        public string UUID { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public int Version { get; set; }
    }
}
