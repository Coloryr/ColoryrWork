namespace ColoryrWork.Lib.Build.Object;

public enum PostBuildType
{
    AddDll, AddClass, AddSocket, AddWebSocket, AddRobot, AddMqtt, AddService, AddWeb,
    GetDll, GetClass, GetSocket, GetWebSocket, GetRobot, GetMqtt, GetTask, GetWeb,
    CodeDll, CodeClass, CodeSocket, CodeWebSocket, CodeRobot, CodeMqtt, CodeTask, CodeWeb,
    RemoveDll, RemoveClass, RemoveSocket, RemoveWebSocket, RemoveRobot, RemoveMqtt, RemoveTask, RemoveWeb,
    UpdataDll, UpdataClass, UpdataSocket, UpdataWebSocket, UpdataRobot, UpdataMqtt, UpdataTask, UpdataWeb,
    WebRemoveFile, WebAddFile, WebAddCode, WebCodeZIP, WebSetIsVue, WebBuild, WebBuildRes, WebDownloadFile,
    AddClassFile, RemoveClassFile, BuildClass,
    ConfigGetHttpList, ConfigGetSocket, ConfigGetRobot, ConfigGetUser,
    ConfigAddHttp, ConfigAddHttpRoute, ConfigAddHttpUrlRoute,
    ConfigRemoveHttp, ConfigRemoveHttpRoute, ConfigRemoveHttpUrlRoute,
    ConfigAddUser, ConfigRemoveUser,
    ConfigSetSocket, ConfigSetRobot,
    SetServerEnable, Rebuild, InitLog, GetLog,
    Check, Login, GetApi, ServerReboot, GetPDB
}
public record BuildOBJ
{
    public string Token { get; set; }
    public string User { get; set; }
    public PostBuildType Mode { get; set; }
    public string UUID { get; set; }
    public string Code { get; set; }
    public string Text { get; set; }
    public string Temp { get; set; }
    public int Version { get; set; }
}