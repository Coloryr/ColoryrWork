using Lib.Build;
using System.Collections.Generic;

namespace ColoryrServer.FileSystem
{
    internal record MainConfig
    {
        /// <summary>
        /// Http处理线程
        /// </summary>
        public int HttpThreadNumber { get; set; }
        /// <summary>
        /// Http配置
        /// </summary>
        public List<HttpConfig> Http { get; set; }
        /// <summary>
        /// IoT配置
        /// </summary>
        public SocketConfig IoT { get; set; }
        /// <summary>
        /// WebSocket配置
        /// </summary>
        public SocketConfig WebSocket { get; set; }
        /// <summary>
        /// Reboot配置
        /// </summary>
        public SocketConfig Robot { get; set; }
        /// <summary>
        /// 非法的代码
        /// </summary>
        public List<string> NoCode { get; set; }
        /// <summary>
        /// Mysql配置
        /// </summary>
        public List<MysqlConfig> Mysql { get; set; }
        /// <summary>
        /// MS sql设置
        /// </summary>
        public List<MSsqlConfig> MSsql { get; set; }
        /// <summary>
        /// Redis设置
        /// </summary>
        public List<RedisConfig> Redis { get; set; }
        /// <summary>
        /// Oracle配置
        /// </summary>
        public List<OracleConfig> Oracle { get; set; }
        /// <summary>
        /// 用户路径
        /// </summary>
        public List<UserConfig> User { get; set; }
        /// <summary>
        /// 不参与动态编译的dll
        /// </summary>
        public List<string> NotInclude { get; set; }
        /// <summary>
        /// 分离管道设置
        /// </summary>
        public Pipe Pipe { get; set; }
        /// <summary>
        /// MQTT配置
        /// </summary>
        public MQTTConfig MQTTConfig { get; set; }
        /// <summary>
        /// 任务配置
        /// </summary>
        public TaskUtilConfig TaskConfig { get; set; }
        /// <summary>
        /// ffmpeg
        /// </summary>
        public string MPGE { get; set; }
        /// <summary>
        /// HttpClient数量
        /// </summary>
        public int HttpClientNumber { get; set; }
        /// <summary>
        /// 请求选项
        /// </summary>
        public RequsetChoose Requset { get; set; }
        /// <summary>
        /// AES加密
        /// </summary>
        public AESConfig AES { get; set; }
    }
    internal record AESConfig
    { 
        public string Key { get; set; }
        public string IV { get; set; }
    }
    internal record RequsetChoose
    {
        /// <summary>
        /// 前端
        /// </summary>
        public string Web { get; set; }
        /// <summary>
        /// 后端
        /// </summary>
        public string Back { get; set; }
        /// <summary>
        /// 放进缓存的文件类型
        /// </summary>
        public List<string> Temp { get; set; }
        /// <summary>
        /// 缓存保留时间
        /// </summary>
        public int TempTime { get; set; }
        /// <summary>
        /// 启用主页
        /// </summary>
        public bool EnableIndex { get; set; }
    }
    internal record MQTTConfig
    {
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
    internal record TaskUtilConfig
    {
        /// <summary>
        /// 线程数量
        /// </summary>
        public int ThreadNumber { get; set; }
        /// <summary>
        /// 最大运行时间
        /// </summary>
        public int MaxTime { get; set; }
    }
    internal record OracleConfig
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
        /// 连接数
        /// </summary>
        public int ConnCount { get; set; }
        /// <summary>
        /// 连接超时
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Conn { get; set; }
    }
    internal record UserConfig
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码SHA1
        /// </summary>
        public string Password { get; set; }
    }
    internal record SocketConfig
    {
        /// <summary>
        /// socket设置的IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
    internal record HttpConfig : SocketConfig
    {
    }
    internal record MysqlConfig
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
        /// 连接数
        /// </summary>
        public int ConnCount { get; set; }
        /// <summary>
        /// 连接超时
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Conn { get; set; }
    }
    internal record MSsqlConfig
    {
        /// <summary>
        /// <summary>
        /// 启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 连接数
        /// </summary>
        public int ConnCount { get; set; }
        /// <summary>
        /// 连接超时
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// 连接的字符串
        /// </summary>
        public string Conn { get; set; }
    }
    internal record RedisConfig
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
        /// 连接数
        /// </summary>
        public int ConnCount { get; set; }
        /// <summary>
        /// 连接超时
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Conn { get; set; }
    }
    internal record Pipe
    {
        /// <summary>
        /// 管道端口
        /// </summary>
        public SocketConfig Socket { get; set; }
        /// <summary>
        /// 启用管道
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 服务器核心
        /// </summary>
        public bool ServerCore { get; set; }
        /// <summary>
        /// Http服务器
        /// </summary>
        public bool HttpServer { get; set; }
        /// <summary>
        /// WebSocket服务器
        /// </summary>
        public bool WebSocketServer { get; set; }
        /// <summary>
        /// Iot服务器
        /// </summary>
        public bool IotServer { get; set; }
        /// <summary>
        /// Mqtt服务器
        /// </summary>
        public bool MqttServer { get; set; }
    }
    internal class ConfigUtil
    {
        public static string FilePath = ServerMain.RunLocal + @"MainConfig.json";
        /// <summary>
        /// 读配置文件
        /// </summary>
        public static void Start()
        {
            ServerMain.Config = ConfigSave.Config(new MainConfig
            {
                NotInclude = new()
                {
                    "sni.dll"
                },
                HttpThreadNumber = 200,
                Http = new()
                {
                    new()
                    {
                        IP = "127.0.0.1",
                        Port = 25555
                    }
                },
                IoT = new()
                {
                    IP = "0.0.0.0",
                    Port = 25556
                },
                WebSocket = new()
                {
                    IP = "0.0.0.0",
                    Port = 25557
                },
                Robot = new()
                {
                    IP = "127.0.0.1",
                    Port = 23333
                },
                NoCode = new()
                {
                    "System.IO"
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
                        ConnCount = 50,
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
                        ConnCount = 50,
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
                        TimeOut = 1000,
                        Conn = "{0}:{1}",
                        ConnCount = 20
                    }
                },
                Oracle = new()
                {
                    new()
                    {
                        Enable = false,
                        IP = "127.0.0.1",
                        User = "root",
                        Password = "MTIzNDU2",
                        ConnCount = 20,
                        TimeOut = 1000,
                        Conn = "User Id={2};Password={3};Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME=test)))"
                    }
                },
                Pipe = new()
                {
                    Enable = false,
                    ServerCore = true,
                    HttpServer = false,
                    IotServer = false,
                    WebSocketServer = false,
                    MqttServer = false,
                    Socket = new()
                    {
                        IP = "127.0.0.1",
                        Port = 23334
                    }
                },
                User = new()
                {
                    new()
                    {
                        Username = "Admin",
                        Password = "4e7afebcfbae000b22c7c85e5560f89a2a0280b4"
                    }
                },
                MQTTConfig = new()
                {
                    Port = 12345
                },
                TaskConfig = new()
                {
                    MaxTime = 30,
                    ThreadNumber = 50
                },
                Requset = new()
                {
                    Web = "/Web",
                    Back = "/Back",
                    Temp = new()
                    {
                        ".jpg",
                        ".png",
                        ".mp4",
                        ".jpge",
                        ".gif"
                    },
                    TempTime = 1800,
                    EnableIndex = true
                },
                HttpClientNumber = 100,
                AES = new()
                {
                    Key = "Key",
                    IV = "IV"
                }
            }, FilePath);
        }
    }
}
