using ColoryrServer.Core;
using ColoryrServer.Core.FileSystem;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.NetCoreServer;

internal record SslObj
{
    public string Local { get; set; }
    public string Password { get; set; }
}

internal record CoreConfig : MainConfig
{
    /// <summary>
    /// Http配置
    /// </summary>
    public SocketConfig Http { get; set; }
    public bool UseSsl { get; set; }
    public bool NoInput { get; set; }
    public SslObj Ssl { get; set; }
    //public Dictionary<string, RouteConfigObj> Routes { get; set; }
    //public Dictionary<string, SslObj> Ssls { get; set; }
    //public Dictionary<string, RouteConfigObj> UrlRoutes { get; set; }
    //public bool RouteEnable { get; set; }
}

internal class CoreConfigUtils
{
    public void Start()
    {
        ServerMain.Config = CoreServer.Config = ConfigSave.Config(new CoreConfig
        {
            //Routes = new()
            //{
            //    {
            //        "turn",
            //        new()
            //        {
            //            Url = "http://127.0.0.1/",
            //            Heads = new()
            //        }
            //    }
            //},
            //UrlRoutes = new()
            //{
            //    {
            //        "www.test.com",
            //        new()
            //        {
            //            Url = "http://localhost:81/",
            //            Heads = new()
            //        }
            //    }
            //},
            //RouteEnable = false,
            //NoInput = false,
            UseSsl = false,
            Ssl = new()
            {
                Local = "xxx.pfx",
                Password = "123456"
            },
            Http = new()
            {
                IP = "0.0.0.0",
                Port = 80
            },
            CodeSetting = new()
            {
                NotInclude = new()
                {
                    "sni.dll"
                },
                CodeLog = true,
                MinifyCSS = true,
                MinifyHtml = true,
                MinifyJS = true
            },
            Socket = new()
            {
                IP = "0.0.0.0",
                Port = 25556
            },
            WebSocket = new()
            {
                Ssl = "",
                Password = "",
                UseSsl = false,
                Socket = new()
                {
                    IP = "0.0.0.0",
                    Port = 25557
                }
            },
            Robot = new()
            {
                Socket = new()
                {
                    IP = "127.0.0.1",
                    Port = 23333
                }
            },
            Mysql = new()
            {
                new()
                {
                    Enable = false,
                    IP = "127.0.0.1",
                    Port = 3306,
                    User = "root",
                    Password = "MTIzNDU2",
                    TimeOut = 1000,
                    Conn = "SslMode=none;Server={0};Port={1};User ID={2};Password={3};Charset=utf8;"
                }
            },
            MSsql = new()
            {
                new()
                {
                    Enable = false,
                    IP = "127.0.0.1",
                    User = "root",
                    Password = "MTIzNDU2",
                    TimeOut = 1000,
                    Conn = "Server={0};UID={1};PWD={2};"
                }
            },
            Redis = new()
            {
                new()
                {
                    Enable = false,
                    IP = "127.0.0.1",
                    Port = 6379,
                    Conn = "{0}:{1}"
                }
            },
            Oracle = new()
            {
                new()
                {
                    Enable = false,
                    IP = "",
                    User = "",
                    Password = "",
                    TimeOut = 1000,
                    Conn = "Data Source=MyDatabase.db;Mode=ReadWriteCreate"
                }
            },
            SQLite = new()
            {
                new()
                {
                    Enable = false,
                    IP = "127.0.0.1",
                    User = "root",
                    Password = "MTIzNDU2",
                    TimeOut = 1000,
                    Conn = "User Id={2};Password={3};Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME=test)))"
                }
            },
            MqttConfig = new()
            {
                Ssl = "",
                Password = "",
                Socket = new()
                {
                    IP = "0.0.0.0",
                    Port = 12345
                }
            },
            TaskConfig = new()
            {
                MaxTime = 30
            },
            Requset = new()
            {
                //Temp = new()
                //{
                //    ".jpg",
                //    ".png",
                //    ".mp4",
                //    ".jpge",
                //    ".gif"
                //},
                TempTime = 1800,
                //Stream = true,
                //StreamType = new()
                //{
                //    ".mp4"
                //}
            },
            HttpClientNumber = 100,
            AES = new()
            {
                Key = "Key",
                IV = "IV"
            }
        }, "config.json");
    }

    public void Save()
    {
        ConfigSave.Save(CoreServer.Config, "config.json");
    }
}
