using ColoryrServer.Core.FileSystem.Web;
using ColoryrServer.SDK;
using Newtonsoft.Json;
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
    public static HttpReturn FromError { get; } = new()
    {
        Res = ResType.String,
        ContentType = ServerContentType.JSON,
        Data = JsonConvert.SerializeObject(new
        {
            res = 500,
            text = "表单解析发生错误，请检查数据"
        })
    };

    public static HttpReturn StreamError { get; } = new()
    {
        Res = ResType.String,
        ContentType = ServerContentType.JSON,
        Data = JsonConvert.SerializeObject(new
        {
            res = 500,
            text = "流数据错误"
        })
    };

    public static HttpReturn ResReload { get; } = new()
    {
        Res = ResType.String,
        ContentType = ServerContentType.JSON,
        Data = JsonConvert.SerializeObject(new
        {
            res = 90,
            text = "服务器正在重载"
        })
    };
    public static HttpReturn ResError { get; } = new()
    {
        Res = ResType.String,
        ReCode = 400,
        ContentType = ServerContentType.JSON,
        Data = JsonConvert.SerializeObject(new
        {
            res = 400,
            text = "服务器内部错误"
        })
    };
    public static HttpReturn Res404 { get; } = new()
    {
        Data = WebBinManager.BaseDir.Html404,
        ContentType = ServerContentType.HTML,
        Res = ResType.Byte,
        ReCode = 404
    };
    public static HttpReturn ResFixMode { get; } = new()
    {
        Data = WebBinManager.BaseDir.HtmlFixMode,
        ContentType = ServerContentType.HTML,
        Res = ResType.Byte,
        ReCode = 200
    };

    public static LockRoute Reload { get; } = new();
    public static FixRoute Fix { get; } = new();
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

    public class FixRoute : RouteObj
    {
        public FixRoute()
        {
            IsDll = false;
            IsReload = false;
        }
        override public HttpReturn Invoke(HttpDllRequest arg, string function)
        {
            return ResFixMode;
        }
    }
}