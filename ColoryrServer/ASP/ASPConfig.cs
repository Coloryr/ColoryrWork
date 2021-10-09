﻿using ColoryrServer.FileSystem;
using Lib.Build;

namespace ColoryrServer.ASP
{
    internal record ASPConfig : MainConfig
    {
        public bool Ssl { get; set; }
        public string SslLocal { get; set; }
        public string SslPassword { get; set; }
        public bool NoInput { get; set; }
    }

    public class ASPConfigUtils : ConfigUtil
    {
        public override void Start()
        {
            ServerMain.Config = ASPServer.Config = ConfigSave.Config(new ASPConfig
            {
                Ssl = false,
                SslLocal = "",
                SslPassword = "",
                NoInput = false,

                NotInclude = new()
                {
                    "sni.dll"
                },
                Http = new()
                {
                    new()
                    {
                        IP = "127.0.0.1",
                        Port = 25555
                    }
                },
                Socket = new()
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
                    WebAPI = "/WebAPI",
                    Temp = new()
                    {
                        ".jpg",
                        ".png",
                        ".mp4",
                        ".jpge",
                        ".gif"
                    },
                    TempTime = 1800,
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
