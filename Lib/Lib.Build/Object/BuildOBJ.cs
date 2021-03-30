namespace Lib.Build.Object
{
    public enum ReType
    {
        AddDll, AddClass, AddIoT, AddWebSocket, AddRobot, AddMqtt, AddApp, AddTask,
        GetDll, GetClass, GetIoT, GetWebSocket, GetRobot, GetMqtt, GetApp, GetTask,
        CodeDll, CodeClass, CodeIoT, CodeWebSocket, CodeRobot, CodeMqtt, CodeApp, CodeTask,
        RemoveDll, RemoveClass, RemoveIoT, RemoveWebSocket, RemoveRobot, RemoveMqtt, RemoveApp, RemoveTask,
        UpdataDll, UpdataClass, UpdataIoT, UpdataWebSocket, UpdataRobot, UpdataMqtt, UpdataTask,
        AppRemoveFile, AppAddCS, AppAddXaml, RemoveAppCS, RemoveAppXaml,
        AppCsUpdata, AppXamlUpdata, AppUpdata,
        Check, Login, GetApi,
        SetAppKey
    }
    class BuildOBJ
    {
        public string Token { get; set; }
        public string User { get; set; }
        public ReType Mode { get; set; }
        public string UUID { get; set; }
        public string Code { get; set; }
        public string Text { get; set; }
        public string Temp { get; set; }
        public int Version { get; set; }
    }
}
