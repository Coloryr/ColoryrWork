using Lib.Build;
using System.Collections.Generic;

namespace ColoryrServer.FileSystem
{
    internal record MainConfig
    {
        /// <summary>
        /// Socket配置
        /// </summary>
        public HttpConfig Http { get; set; }
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
        /// 引用的using
        /// </summary>
        public List<string> Include { get; set; }
        /// <summary>
        /// 非法的代码
        /// </summary>
        public List<string> NoCode { get; set; }
        /// <summary>
        /// Mysql配置
        /// </summary>
        public MysqlConfig Mysql { get; set; }
        /// <summary>
        /// MS sql设置
        /// </summary>
        public MSsqlConfig MSsql { get; set; }
        /// <summary>
        /// Redis设置
        /// </summary>
        public RedisConfig Redis { get; set; }
        /// <summary>
        /// Oracle配置
        /// </summary>
        public OracleConfig Oracle { get; set; }
        /// <summary>
        /// 用户路径
        /// </summary>
        public List<UserConfig> User { get; set; }
        /// <summary>
        /// 分离管道设置
        /// </summary>
        public Pipe Pipe { get; set; }
        /// <summary>
        /// ffmpeg
        /// </summary>
        public string MPGE { get; set; }
        /// <summary>
        /// HttpClient数量
        /// </summary>
        public int HttpClientNumber { get; set; }
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
        /// 服务器名
        /// </summary>
        public string ServerName { get; set; }
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
        /// <summary>
        /// 线程数量
        /// </summary>
        public int ThreadNumber { get; set; }
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
    }
    internal class ConfigUtil
    {
        public static string FilePath = ServerMain.RunLocal + @"Mainconfig.json";
        /// <summary>
        /// 读配置文件
        /// </summary>
        public static void Start()
        {
            ServerMain.Config = ConfigSave.Config(new MainConfig
            {
                Http = new()
                {
                    IP = "127.0.0.1",
                    Port = 25555,
                    ThreadNumber = 200
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
                Include = new()
                {
                    "using System;",
                    "using System.Linq",
                    "using System.Reflection;",
                    "using System.Runtime.InteropServices;",
                    "using System.Collections.Generic;",
                    "using System.Runtime.CompilerServices;",
                    "using System.Collections.Specialized;",
                    "using ColoryrSDK;",
                    "using Newtonsoft.Json;",
                    "using Newtonsoft.Json.Linq;",
                    "using HtmlAgilityPack;"
                },
                NoCode = new()
                {
                    "System.IO"
                },
                Mysql = new()
                {
                    Enable = false,
                    IP = "127.0.0.1",
                    Port = 3306,
                    User = "Root",
                    Password = "MTIzNDU2",
                    ConnCount = 50,
                    TimeOut = 1000,
                    Conn = "SslMode=none;Server={0};Port={1};User ID={2};Password={3};Charset=utf8;"
                },
                MSsql = new()
                {
                    Enable = false,
                    IP = "127.0.0.1",
                    User = "Root",
                    Password = "MTIzNDU2",
                    ConnCount = 50,
                    TimeOut = 1000,
                    Conn = "Server={0};UID={1};PWD={2};"
                },
                Redis = new()
                {
                    Enable = false,
                    IP = "127.0.0.1",
                    Port = 6379,
                    TimeOut = 1000,
                    Conn = "{0}:{1}",
                    ConnCount = 20
                },
                Oracle = new()
                {
                    Enable = false,
                    IP = "127.0.0.1",
                    User = "Root",
                    Password = "MTIzNDU2",
                    ConnCount = 20,
                    TimeOut = 1000,
                    Conn = "SslMode=none;Server={0};Port={1};User ID={2};Password={3};Charset=utf8;"
                },
                Pipe = new()
                {
                    Enable = false,
                    ServerCore = true,
                    HttpServer = false,
                    IotServer = false,
                    WebSocketServer = false,
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
                }
            }, FilePath);
        }
    }
}
