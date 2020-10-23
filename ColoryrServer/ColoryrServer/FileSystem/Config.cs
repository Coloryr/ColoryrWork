using Lib.Build;
using System.Collections.Generic;

namespace ColoryrServer.DllManager
{
    public class MainConfig
    {
        /// <summary>
        /// Socket配置
        /// </summary>
        public SocketConfig Http { get; set; }
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
        public Mysql Mysql { get; set; }
        /// <summary>
        /// MS sql设置
        /// </summary>
        public MSsql MSsql { get; set; }
        /// <summary>
        /// Redis设置
        /// </summary>
        public Redis Redis { get; set; }
        /// <summary>
        /// Oracle配置
        /// </summary>
        public Oracle Oracle { get; set; }
        /// <summary>
        /// 用户路径
        /// </summary>
        public List<User> User { get; set; }
        /// <summary>
        /// ffmpeg
        /// </summary>
        public string MPGE { get; set; }
    }
    public class Oracle
    {
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
    }
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Admin { get; set; }
    }

    public class SocketConfig
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
    public class Mysql
    {
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
    }
    public class MSsql
    {
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
    }
    public class Redis
    {
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
    }
    class Config
    {
        public static string FilePath = ServerMain.RunLocal + @"Mainconfig.json";
        /// <summary>
        /// 读配置文件
        /// </summary>
        public Config()
        {
            ServerMain.Config = ConfigSave.Config(new MainConfig
            {
                Http = new SocketConfig
                {
                    IP = "+",
                    Port = 25555
                },
                IoT = new SocketConfig
                {
                    IP = "0.0.0.0",
                    Port = 25556
                },
                WebSocket = new SocketConfig
                {
                    IP = "0.0.0.0",
                    Port = 25557
                },
                Robot = new SocketConfig
                {
                    IP = "127.0.0.1",
                    Port = 23333
                },
                Include = new List<string>()
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
                NoCode = new List<string>()
                    {
                        "System.IO"
                    },
                Mysql = new Mysql
                {
                    IP = "127.0.0.1",
                    Port = 3306,
                    User = "Root",
                    Password = "MTIzNDU2",
                    ConnCount = 100
                },
                MSsql = new MSsql
                {
                    IP = "127.0.0.1",
                    User = "Root",
                    Password = "MTIzNDU2",
                    ConnCount = 100
                },
                Redis = new Redis
                {
                    IP = "",
                    Port = 0
                },
                Oracle = new Oracle
                {
                    IP = "127.0.0.1",
                    User = "Root",
                    Password = "MTIzNDU2",
                    ConnCount = 20
                },
                User = new List<User>()
                {
                    new User
                    {
                        Username = "Admin",
                        Password = "4e7afebcfbae000b22c7c85e5560f89a2a0280b4",
                        Admin = true
                    }
                }
            }, FilePath);
        }
        public static void Save()
        {
            ConfigSave.Save(ServerMain.Config, FilePath);
        }
    }
}
