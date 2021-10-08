namespace Lib.Build.Object
{
    public enum ReType
    {
        AddDll, AddClass, AddSocket, AddWebSocket, AddRobot, AddMqtt, AddApp, AddTask, AddWeb,
        GetDll, GetClass, GetSocket, GetWebSocket, GetRobot, GetMqtt, GetApp, GetTask, GetWeb,
        CodeDll, CodeClass, CodeSocket, CodeWebSocket, CodeRobot, CodeMqtt, CodeApp, CodeTask, CodeWeb,
        RemoveDll, RemoveClass, RemoveSocket, RemoveWebSocket, RemoveRobot, RemoveMqtt, RemoveApp, RemoveTask, RemoveWeb,
        UpdataDll, UpdataClass, UpdataSocket, UpdataWebSocket, UpdataRobot, UpdataMqtt, UpdataTask, UpdataWeb,
        AppRemoveFile, AppAddCS, AppAddXaml, RemoveAppCS, RemoveAppXaml,
        AppCsUpdata, AppXamlUpdata, AppUpdata,
        WebRemoveFile, WebAddFile, WebAddCode,
        Check, Login, GetApi,
        SetAppKey
    }
    public class BuildOBJ
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
