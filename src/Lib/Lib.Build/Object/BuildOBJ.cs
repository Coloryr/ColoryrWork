namespace ColoryrWork.Lib.Build.Object;

public enum ReType
{
    AddDll, AddClass, AddSocket, AddWebSocket, AddRobot, AddMqtt, AddTask, AddWeb,
    GetDll, GetClass, GetSocket, GetWebSocket, GetRobot, GetMqtt, GetTask, GetWeb,
    CodeDll, CodeClass, CodeSocket, CodeWebSocket, CodeRobot, CodeMqtt, CodeTask, CodeWeb,
    RemoveDll, RemoveClass, RemoveSocket, RemoveWebSocket, RemoveRobot, RemoveMqtt, RemoveTask, RemoveWeb,
    UpdataDll, UpdataClass, UpdataSocket, UpdataWebSocket, UpdataRobot, UpdataMqtt, UpdataTask, UpdataWeb,
    WebRemoveFile, WebAddFile, WebAddCode, WebCodeZIP, WebSetIsVue, WebBuild, WebBuildRes, WebDownloadFile,
    Check, Login, GetApi,
    SetRobot, GetConfig,
    AddClassFile, RemoveClassFile, BuildClass
}
public record BuildOBJ
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