using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using System.Collections.Generic;

namespace ColoryrServer.Core.FileSystem;

public record MainConfig
{
    /// <summary>
    /// Socket配置
    /// </summary>
    public SocketConfig Socket { get; set; }
    /// <summary>
    /// WebSocket配置
    /// </summary>
    public SslConfigObj WebSocket { get; set; }
    /// <summary>
    /// Reboot配置
    /// </summary>
    public RebotConfigObj Robot { get; set; }
    /// <summary>
    /// Mysql配置
    /// </summary>
    public List<SQLConfig> Mysql { get; set; }
    /// <summary>
    /// MS sql设置
    /// </summary>
    public List<SQLConfig> MSsql { get; set; }
    /// <summary>
    /// Oracle配置
    /// </summary>
    public List<SQLConfig> Oracle { get; set; }
    /// <summary>
    /// SQLite配置
    /// </summary>
    public List<SQLConfig> SQLite { get; set; }
    /// <summary>
    /// Redis设置
    /// </summary>
    public List<RedisConfig> Redis { get; set; }
    /// <summary>
    /// MQTT配置
    /// </summary>
    public SslConfigObj MqttConfig { get; set; }
    /// <summary>
    /// 任务配置
    /// </summary>
    public TaskUtilConfigObj TaskConfig { get; set; }
    /// <summary>
    /// HttpClient数量
    /// </summary>
    public int HttpClientNumber { get; set; }
    /// <summary>
    /// 编辑器AES加密
    /// </summary>
    public AESConfig AES { get; set; }
    /// <summary>
    /// 代码设置选项
    /// </summary>
    public CodeConfigObj CodeSetting { get; set; }
    /// <summary>
    /// 集群设置
    /// </summary>
    public PipeConfigObj Pipe { get; set; }
    /// <summary>
    /// 请求选项
    /// </summary>
    public RequsetChooseObj Requset { get; set; }
    /// <summary>
    /// 维护模式
    /// </summary>
    public bool FixMode { get; set; }
}

public record RequsetChooseObj
{
    /// <summary>
    /// 缓存保留时间
    /// </summary>
    public int TempTime { get; set; }
}

public record PipeConfigObj
{ 
    /// <summary>
    /// 是否启用集群
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// 是否为服务器端
    /// </summary>
    public bool Server { get; set; }
    /// <summary>
    /// 服务器IP
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 服务器端口
    /// </summary>
    public int Port { get; set; }
}

public record SslConfigObj
{
    /// <summary>
    /// 使用SSL证书
    /// </summary>
    public bool UseSsl { get; set; }
    /// <summary>
    /// SSL证书存放位置
    /// </summary>
    public string Ssl { get; set; }
    /// <summary>
    /// SSL证书密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 绑定端口
    /// </summary>
    public SocketConfig Socket { get; set; }
}

public record CodeConfigObj
{
    /// <summary>
    /// 不参与动态编译的.dll
    /// </summary>
    public List<string> NotInclude { get; set; }
    /// <summary>
    /// 代码修改信息储存
    /// </summary>
    public bool CodeLog { get; set; }
    /// <summary>
    /// Html代码压缩
    /// </summary>
    public bool MinifyHtml { get; set; }
    /// <summary>
    /// JS代码压缩
    /// </summary>
    public bool MinifyJS { get; set; }
    /// <summary>
    /// CSS代码压缩
    /// </summary>
    public bool MinifyCSS { get; set; }
}

public record RebotConfigObj
{
    /// <summary>
    /// 连接端口
    /// </summary>
    public SocketConfig Socket { get; set; }
}

public record AESConfig
{
    /// <summary>
    /// 加密KEY
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// 加密盐
    /// </summary>
    public string IV { get; set; }
}
public record TaskUtilConfigObj
{
    /// <summary>
    /// 最大运行时间
    /// </summary>
    public int MaxTime { get; set; }
}
public record SQLConfig
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// IP地址
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public string User { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 连接超时
    /// </summary>
    public int TimeOut { get; set; }
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string Conn { get; set; }
}
public record RedisConfig
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// IP地址
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 端口
    /// </summary>
    public short Port { get; set; }
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string Conn { get; set; }
}
internal static class ConfigUtils
{
    private static string FilePath { get; } = ServerMain.RunLocal + "MainConfig.json";

    private static MainConfig MakeNew()
    {
        return new()
        {
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
            HttpClientNumber = 100,
            AES = new()
            {
                Key = "Key",
                IV = "IV"
            },
            FixMode = false
        };
    }

    /// <summary>
    /// 读配置文件
    /// </summary>
    public static void Start()
    {
        ServerMain.Config = ConfigSave.Config(MakeNew(), FilePath);
    }
    /// <summary>
    /// 保存配置文件
    /// </summary>
    public static void Save()
    {
        ConfigSave.Save(ServerMain.Config, FilePath);
    }
}
