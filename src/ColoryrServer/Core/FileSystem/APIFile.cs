using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.Core.FileSystem;

internal static class APIFile
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

        list.List.Add("MysqlDll", OtherSDKResource.MysqlDll);
        list.List.Add("SqlClientDll", OtherSDKResource.SqlClientDll);
        list.List.Add("OracleDll", OtherSDKResource.OracleDll);
        list.List.Add("RedisDll", OtherSDKResource.RedisDll);
        list.List.Add("WebSocketDll", OtherSDKResource.WebSocketDll);
        list.List.Add("HtmlDll", OtherSDKResource.HtmlDll);
        list.List.Add("HtmlDll1", OtherSDKResource.HtmlDll1);
        list.List.Add("MqttDll", OtherSDKResource.MqttDll);
        list.List.Add("MqttDll1", OtherSDKResource.MqttDll1);
        list.List.Add("MqttDll2", OtherSDKResource.MqttDll2);
        list.List.Add("MqttDll3", OtherSDKResource.MqttDll3);
        list.List.Add("MqttDll4", OtherSDKResource.MqttDll4);
        list.List.Add("MqttDll5", OtherSDKResource.MqttDll5);
        list.List.Add("MqttDll6", OtherSDKResource.MqttDll6);
        list.List.Add("MqttDll7", OtherSDKResource.MqttDll7);
        list.List.Add("MqttDll8", OtherSDKResource.MqttDll8);
        list.List.Add("MqttDll9", OtherSDKResource.MqttDll9);
        list.List.Add("MqttDll10", OtherSDKResource.MqttDll10);
        list.List.Add("MqttDll11", OtherSDKResource.MqttDll11);
        list.List.Add("MqttDll12", OtherSDKResource.MqttDll12);
        list.List.Add("MqttDll13", OtherSDKResource.MqttDll13);
        list.List.Add("MqttDll14", OtherSDKResource.MqttDll14);
    }
}
