using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.SDK;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.Core.Http;

public enum ResType
{
    Json, String, Byte, Stream
}
public record HttpReturn
{
    public Encoding Encoding = Encoding.UTF8;
    public Dictionary<string, string> Cookie;
    public string ContentType = ServerContentType.JSON;
    public ResType Res;
    public object Data;
    public Dictionary<string, string> Head;
    public int ReCode = 200;
    public int Pos = 0;
}

public static class HttpReturnSave
{
    public static HttpReturn ResReload = new()
    {
        Res = ResType.String,
        Data = "{\"res\":90,\"text\":\"正在重载\"}"
    };
    public static HttpReturn Res404 = new()
    {
        Data = WebBinManager.BaseDir.Html404,
        ContentType = ServerContentType.HTML,
        Res = ResType.Byte,
        ReCode = 404
    };

    public static LockRoute Reload = new();
    public class LockRoute : RouteObj
    {
        public LockRoute()
        {
            IsDll = false;
            IsReload = false;
        }
        public override HttpReturn Invoke(HttpDllRequest arg, string function)
        {
            return ResReload;
        }
    }
}