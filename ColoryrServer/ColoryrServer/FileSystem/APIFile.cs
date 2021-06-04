using Lib.Build.Object;

namespace ColoryrServer.FileSystem
{
    internal class APIFile
    {
        public static APIFileObj list;
        public static void Start()
        {
            list = new();
            list.List.Add("ColoryrSDK", ColoryrServer_Resource.ColoryrSDK);
            list.List.Add("DatabaseSDK", ColoryrServer_Resource.DatabaseSDK);
            list.List.Add("HtmlDocumentSDK", ColoryrServer_Resource.HtmlDocumentSDK);
            list.List.Add("HtmlNodeSDK", ColoryrServer_Resource.HtmlNodeSDK);
            list.List.Add("HtmlSDK", ColoryrServer_Resource.HtmlSDK);
            list.List.Add("HttpSDK", ColoryrServer_Resource.HttpSDK);
            list.List.Add("IoTSDK", ColoryrServer_Resource.IoTSDK);
            list.List.Add("IWebSocketConnectionSDK", ColoryrServer_Resource.IWebSocketConnectionSDK);
            list.List.Add("MySqlCommandSDK", ColoryrServer_Resource.MySqlCommandSDK);
            list.List.Add("OracleCommandSDK", ColoryrServer_Resource.OracleCommandSDK);
            list.List.Add("RobotSDK", ColoryrServer_Resource.RobotSDK);
            list.List.Add("SqlCommandSDK", ColoryrServer_Resource.SqlCommandSDK);
            list.List.Add("WebSocketSDK", ColoryrServer_Resource.WebSocketSDK);
            list.List.Add("TaskSDK", ColoryrServer_Resource.TaskSDK);
            list.List.Add("NotesSDK", ColoryrServer_Resource.NotesSDK);
            list.List.Add("RobotPacks", ColoryrServer_Resource.RobotPacks);
            list.List.Add("MqttSDK", ColoryrServer_Resource.MqttSDK);
            list.List.Add("WebHtmlSDK", ColoryrServer_Resource.WebHtmlSDK);
            list.List.Add("ZipOutputStreamSDK", ColoryrServer_Resource.ZipOutputStreamSDK);
            list.List.Add("ZipInputStreamSDK", ColoryrServer_Resource.ZipInputStreamSDK);
            list.List.Add("ZipEntrySDK", ColoryrServer_Resource.ZipEntrySDK);
        }
    }
}
