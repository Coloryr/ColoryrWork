using ColoryrServer.SDK;

namespace ColoryrTest.Run;

[DllIN]
internal class DllDemo
{
    [NotesSDK("一个接口", new string[1] { "输入" }, new string[1] { "输出" })]
    public dynamic Main(HttpDllRequest http)
    {
        DebugMysql mysql = new("test");
        var data = mysql.Execute("select * from test", null);

        return "true:" + http.Get("name");
    }
}
