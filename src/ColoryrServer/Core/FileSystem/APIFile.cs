using ColoryrWork.Lib.Build.Object;
using System.Text;

namespace ColoryrServer.Core.FileSystem;

internal static class APIFile
{
    public static APIFileObj list;
    public static void Start()
    {
        list = new();
        list.List.Add("ColoryrSDK", Encoding.UTF8.GetString(SDKResource.ColoryrSDK));
        list.List.Add("DatabaseSDK", Encoding.UTF8.GetString(SDKResource.DatabaseSDK));
        list.List.Add("HtmlSDK", Encoding.UTF8.GetString(SDKResource.HtmlSDK));
        list.List.Add("MqttSDK", Encoding.UTF8.GetString(SDKResource.MqttSDK));
        list.List.Add("NotesSDK", Encoding.UTF8.GetString(SDKResource.NotesSDK));
        list.List.Add("RobotPacks", Encoding.UTF8.GetString(SDKResource.RobotPacks));
        list.List.Add("RobotSDK", Encoding.UTF8.GetString(SDKResource.RobotSDK));
        list.List.Add("SocketSDK", Encoding.UTF8.GetString(SDKResource.SocketSDK));
        list.List.Add("ServiceSDK", Encoding.UTF8.GetString(SDKResource.ServiceSDK));
        list.List.Add("WebApiSDK", Encoding.UTF8.GetString(SDKResource.WebApiSDK));
        list.List.Add("WebHtmlSDK", Encoding.UTF8.GetString(SDKResource.WebHtmlSDK));
        list.List.Add("WebSocketSDK", Encoding.UTF8.GetString(SDKResource.WebSocketSDK));

        //list.Other.Add("MysqlDll", OtherSDKResource.MysqlDll);
        //list.Other.Add("SqlClientDll", OtherSDKResource.SqlClientDll);
        //list.Other.Add("OracleDll", OtherSDKResource.OracleDll);
        //list.Other.Add("RedisDll", OtherSDKResource.RedisDll);
        //list.Other.Add("WebSocketDll", OtherSDKResource.WebSocketDll);
        //list.Other.Add("HtmlDll", OtherSDKResource.HtmlDll);
        //list.Other.Add("HtmlDll1", OtherSDKResource.HtmlDll1);
        //list.Other.Add("MqttDll", OtherSDKResource.MqttDll);
        //list.Other.Add("MqttDll1", OtherSDKResource.MqttDll1);
        //list.Other.Add("MqttDll2", OtherSDKResource.MqttDll2);
        //list.Other.Add("MqttDll3", OtherSDKResource.MqttDll3);
        //list.Other.Add("MqttDll4", OtherSDKResource.MqttDll4);
        //list.Other.Add("MqttDll5", OtherSDKResource.MqttDll5);
        //list.Other.Add("MqttDll6", OtherSDKResource.MqttDll6);
        //list.Other.Add("MqttDll7", OtherSDKResource.MqttDll7);
        //list.Other.Add("MqttDll8", OtherSDKResource.MqttDll8);
        //list.Other.Add("MqttDll9", OtherSDKResource.MqttDll9);
        //list.Other.Add("MqttDll10", OtherSDKResource.MqttDll10);
        //list.Other.Add("MqttDll11", OtherSDKResource.MqttDll11);
        //list.Other.Add("MqttDll12", OtherSDKResource.MqttDll12);
        //list.Other.Add("MqttDll13", OtherSDKResource.MqttDll13);
        //list.Other.Add("MqttDll14", OtherSDKResource.MqttDll14);
        //list.Other.Add("JsonDll", OtherSDKResource.JsonDll);
        //list.Other.Add("JsonDll1", OtherSDKResource.JsonDll1);
        //list.Other.Add("JsonDll2", OtherSDKResource.JsonDll2);
        //list.Other.Add("JsonDll3", OtherSDKResource.JsonDll3);
        //list.Other.Add("JsonDll4", OtherSDKResource.JsonDll4);
    }
}
