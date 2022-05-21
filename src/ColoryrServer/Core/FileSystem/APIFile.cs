using ColoryrServer.Core;
using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.FileSystem;

internal class APIFile
{
    public static APIFileObj list;
    public static void Start()
    {
        list = new();
        list.List.Add("ColoryrSDK", SDKResource.ColoryrSDK);
        list.List.Add("DatabaseSDK", SDKResource.DatabaseSDK);
        list.List.Add("HtmlSDK", SDKResource.HtmlSDK);
        list.List.Add("HttpSDK", SDKResource.HttpSDK);
        list.List.Add("SocketSDK", SDKResource.SocketSDK);
        list.List.Add("RobotSDK", SDKResource.RobotSDK);
        list.List.Add("WebSocketSDK", SDKResource.WebSocketSDK);
        list.List.Add("TaskSDK", SDKResource.TaskSDK);
        list.List.Add("NotesSDK", SDKResource.NotesSDK);
        list.List.Add("RobotPacks", SDKResource.RobotPacks);
        list.List.Add("MqttSDK", SDKResource.MqttSDK);
        list.List.Add("WebHtmlSDK", SDKResource.WebHtmlSDK);
    }
}
