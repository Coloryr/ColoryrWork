namespace Lib.Build.Object
{
    public enum ReType
    {
        AddDll, AddClass, AddIoT, AddWebSocket, AddRobot, AddMqtt, AddApp, AddTask, AddWeb,
        GetDll, GetClass, GetIoT, GetWebSocket, GetRobot, GetMqtt, GetApp, GetTask, GetWeb,
        CodeDll, CodeClass, CodeIoT, CodeWebSocket, CodeRobot, CodeMqtt, CodeApp, CodeTask, CodeWeb,
        RemoveDll, RemoveClass, RemoveIoT, RemoveWebSocket, RemoveRobot, RemoveMqtt, RemoveApp, RemoveTask, RemoveWeb,
        UpdataDll, UpdataClass, UpdataIoT, UpdataWebSocket, UpdataRobot, UpdataMqtt, UpdataTask, UpdataWeb,
        AppRemoveFile, AppAddCS, AppAddXaml, RemoveAppCS, RemoveAppXaml,
        AppCsUpdata, AppXamlUpdata, AppUpdata,
        WebRemoveFile, WebAddFile, WebAddCode, 
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
