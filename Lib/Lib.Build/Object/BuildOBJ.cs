namespace Lib.Build.Object
{
    public enum ReType
    {
        AddDll, AddClass, AddIoT, AddWebSocket, AddRobot, AddMqtt, AddApp,
        GetDll, GetClass, GetIoT, GetWebSocket, GetRobot, GetMqtt, GetApp,
        CodeDll, CodeClass, CodeIoT, CodeWebSocket, CodeRobot, CodeMqtt, CodeApp,
        RemoveDll, RemoveClass, RemoveIoT, RemoveWebSocket, RemoveRobot, RemoveMqtt, RemoveApp,
        UpdataDll, UpdataClass, UpdataIoT, UpdataWebSocket, UpdataRobot, UpdataMqtt,
        AppRemoveFile, AppAddCS, AppAddXaml, RemoveAppCS, RemoveAppXaml,
        AppCsUpdata, AppXamlUpdata, AppUpdata,
        Check, Login, GetApi
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
