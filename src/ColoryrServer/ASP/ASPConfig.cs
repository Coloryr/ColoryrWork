using ColoryrServer.Core;
using ColoryrServer.Core.FileSystem;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.ASP;

internal record SslObj
{
    public string Ssl { get; set; }
    public string Password { get; set; }
}

public record RequsetChooseObj
{
    /// <summary>
    /// 放进缓存的文件类型
    /// </summary>
    public List<string> Temp { get; set; }
    /// <summary>
    /// 缓存保留时间
    /// </summary>
    public int TempTime { get; set; }
    /// <summary>
    /// 启用文件流
    /// </summary>
    public bool Stream { get; set; }
    /// <summary>
    /// 文件流的类型
    /// </summary>
    public List<string> StreamType { get; set; }
}

internal record ASPConfig
{
    /// <summary>
    /// Http配置
    /// </summary>
    public List<SocketConfig> Http { get; set; }
    public Dictionary<string, RouteConfigObj> Routes { get; set; }
    public bool UseSsl { get; set; }
    public Dictionary<string, SslObj> Ssls { get; set; }
    public Dictionary<string, RouteConfigObj> UrlRoutes { get; set; }
    /// <summary>
    /// 请求选项
    /// </summary>
    public RequsetChooseObj Requset { get; set; }
    public bool RouteEnable { get; set; }
    public bool NoInput { get; set; }
}

internal static class ASPConfigUtils
{
    private static string FilePath { get; } = ServerMain.RunLocal + "ASPConfig.json";

    private static ASPConfig MakeNew() 
    {
        return new ASPConfig
        {
            Routes = new()
            {
                {
                    "turn",
                    new()
                    {
                        Url = "http://127.0.0.1",
                        Heads = new()
                    }
                }
            },
            UrlRoutes = new()
            {
                {
                    "www.test.com",
                    new()
                    {
                        Url = "http://localhost:81/",
                        Heads = new()
                    }
                }
            },
            Http = new()
            {
                new()
                {
                    IP = "+",
                    Port = 8080
                }
            },
            RouteEnable = false,
            UseSsl = false,
            Ssls = new()
            {
                {
                    "default",
                    new()
                    {
                        Ssl = "./test.sfx",
                        Password = "123456"
                    }
                }
            },
            Requset = new()
            {
                Temp = new()
                {
                    ".jpg",
                    ".png",
                    ".mp4",
                    ".jpge",
                    ".gif"
                },
                TempTime = 1800,
                Stream = true,
                StreamType = new()
                {
                    ".mp4"
                }
            },
            NoInput = false
        };
    }

    public static void Start()
    {
        ASPServer.Config = ConfigSave.Config(MakeNew(), FilePath);
    }

    public static void Save()
    {
        ConfigSave.Save(ASPServer.Config, FilePath);
    }
}
