using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.Http;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ColoryrServer.Core.DllManager;

public static class DllBuild
{
    private static readonly string DB = ServerMain.RunLocal + @"Login.db";
    private static string connStr;
    public static void Start() 
    {
        connStr = new SqliteConnectionStringBuilder("Data Source=" + DB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        using var DBSQL = new SqliteConnection(connStr);
        string sql = @"create table if not exists login (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `User` text,
  `UUID` text,
  `Time` datetime
);";
        DBSQL.Execute(sql);
    }
    private record LoginObj
    {
        public string User { get; set; }
        public string UUID { get; set; }
    }
    private static bool Check(string User, string UUID) 
    {
        using var DBSQL = new SqliteConnection(connStr);
        var list = DBSQL.Query<LoginObj>("SELECT User,UUID FROM login WHERE User=@User", new { User });
        if (!list.Any())
            return false;
        return list.First().UUID == UUID;
    }
    public static HttpReturn StartBuild(BuildOBJ json, UserConfig user)
    {
        object resObj = null;
        if (json.Mode == ReType.Login)
        {
            if (json.Code == null)
            {
                resObj = new ReMessage
                {
                    Build = false,
                    Message = "账户或密码错误"
                };
            }
            else if (user.Password.ToLower() == json.Code.ToLower())
            {
                json.UUID = Guid.NewGuid().ToString().Replace("-", "");
                using var DBSQL = new SqliteConnection(connStr);
                var list = DBSQL.Query("SELECT id FROM login WHERE User=@User", new { json.User });
                if (list.Any())
                    DBSQL.Execute("UPDATE login SET UUID=@UUID,Time=@Time WHERE User=@User", new { json.UUID, json.User, Time = DateTime.Now });
                else
                    DBSQL.Execute("INSERT INTO login (UUID,User,Time) VALUES(@UUID,@User,@Time)", new { json.UUID, json.User, Time = DateTime.Now });
                resObj = new ReMessage
                {
                    Build = true,
                    Message = json.UUID
                };
            }
            else
                resObj = new ReMessage
                {
                    Build = false,
                    Message = "账户或密码错误"
                };
        }
        else if (json.Mode == ReType.Check)
        {
            resObj = new ReMessage
            {
                Build = Check(json.User, json.UUID),
                Message = "自动登录"
            };
        }
        else if (Check(json.User, json.Token))
        {
            GenReOBJ BuildBack;
            Stopwatch SW;
            CSFileCode File;
            WebObj File2;
            CSFileList List;
            switch (json.Mode)
            {
                case ReType.AddDll:
                    if (CodeFile.GetDll(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Dll,
                            CreateTime = time,
                            Code = DemoResource.Dll
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CodeFile.StorageDll(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Dll[{json.UUID}]已创建"
                        };
                        GenDll.StartGen(File);
                        ServerMain.LogOut($"Dll[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Dll[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddClass:
                    if (CodeFile.GetClass(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Class,
                            CreateTime = time,
                            Code = DemoResource.Class
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CodeFile.StorageClass(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Class[{json.UUID}]已创建"
                        };
                        GenClass.StartGen(File);
                        ServerMain.LogOut($"Class[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Class[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddSocket:
                    if (CodeFile.GetSocket(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Socket,
                            CreateTime = time,
                            Code = DemoResource.Socket
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CodeFile.StorageSocket(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Socket[{json.UUID}]已创建"
                        };
                        GenSocket.StartGen(File);
                        ServerMain.LogOut($"Socket[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Socket[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddWebSocket:
                    if (CodeFile.GetWebSocket(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.WebSocket,
                            CreateTime = time,
                            Code = DemoResource.WebSocket
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CodeFile.StorageWebSocket(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"WebSocket[{json.UUID}]已创建"
                        };
                        GenWebSocket.StartGen(File);
                        ServerMain.LogOut($"WebSocket[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"WebSocket[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddRobot:
                    if (CodeFile.GetRobot(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Robot,
                            CreateTime = time,
                            Code = DemoResource.Robot
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CodeFile.StorageRobot(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Robot[{json.UUID}]已创建"
                        };
                        GenRobot.StartGen(File);
                        ServerMain.LogOut($"Robot[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Robot[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddMqtt:
                    if (CodeFile.GetMqtt(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Mqtt,
                            CreateTime = time,
                            Code = DemoResource.Mqtt
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CodeFile.StorageMqtt(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Mqtt[{json.UUID}]已创建"
                        };
                        GenMqtt.StartGen(File);
                        ServerMain.LogOut($"Mqtt[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Mqtt[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddTask:
                    if (CodeFile.GetTask(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File = new()
                        {
                            UUID = json.UUID,
                            Type = CodeType.Task,
                            CreateTime = time,
                            Code = DemoResource.Task
                            .Replace(CodeDemo.Name, json.UUID)
                        };
                        CodeFile.StorageTask(File);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Task[{json.UUID}]已创建"
                        };
                        GenTask.StartGen(File);
                        ServerMain.LogOut($"Task[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Task[{json.UUID}]已存在"
                        };
                    break;
                case ReType.AddWeb:
                    if (HtmlUtils.GetHtml(json.UUID) == null)
                    {
                        var time = string.Format("{0:s}", DateTime.Now);
                        File2 = new()
                        {
                            UUID = json.UUID,
                            CreateTime = time,
                            Text = "",
                            Codes = new()
                            {
                                {
                                    "index.html",
                                    DemoResource.Html
                                    .Replace(CodeDemo.Name, json.UUID)
                                },
                                { "js.js", DemoResource.Js }
                            },
                            Files = new()
                        };
                        HtmlUtils.New(File2);
                        resObj = new ReMessage
                        {
                            Build = true,
                            Message = $"Web[{json.UUID}]已创建"
                        };
                        ServerMain.LogOut($"Web[{json.UUID}]创建");
                    }
                    else
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]已存在"
                        };
                    break;
                case ReType.GetDll:
                    List = new CSFileList();
                    foreach (var item in CodeFile.DllFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetClass:
                    List = new CSFileList();
                    foreach (var item in CodeFile.ClassFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetSocket:
                    List = new CSFileList();
                    foreach (var item in CodeFile.SocketFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetWebSocket:
                    List = new CSFileList();
                    foreach (var item in CodeFile.WebSocketFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetRobot:
                    List = new CSFileList();
                    foreach (var item in CodeFile.RobotFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetMqtt:
                    List = new CSFileList();
                    foreach (var item in CodeFile.MqttFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetTask:
                    List = new CSFileList();
                    foreach (var item in CodeFile.TaskFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetWeb:
                    List = new CSFileList();
                    foreach (var item in HtmlUtils.HtmlCodeList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.CodeDll:
                    resObj = CodeFile.GetDll(json.UUID);
                    break;
                case ReType.CodeClass:
                    resObj = CodeFile.GetClass(json.UUID);
                    break;
                case ReType.CodeSocket:
                    resObj = CodeFile.GetSocket(json.UUID);
                    break;
                case ReType.CodeWebSocket:
                    resObj = CodeFile.GetWebSocket(json.UUID);
                    break;
                case ReType.CodeRobot:
                    resObj = CodeFile.GetRobot(json.UUID);
                    break;
                case ReType.CodeMqtt:
                    resObj = CodeFile.GetMqtt(json.UUID);
                    break;
                case ReType.CodeTask:
                    resObj = CodeFile.GetTask(json.UUID);
                    break;
                case ReType.CodeWeb:
                    resObj = HtmlUtils.GetHtml(json.UUID);
                    break;
                case ReType.GetApi:
                    resObj = APIFile.list;
                    break;
                case ReType.RemoveDll:
                    CodeFile.RemoveFile(CodeType.Dll, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Dll[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveClass:
                    CodeFile.RemoveFile(CodeType.Class, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Class[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveSocket:
                    CodeFile.RemoveFile(CodeType.Socket, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Socket[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveWebSocket:
                    CodeFile.RemoveFile(CodeType.WebSocket, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"WebSocket[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveRobot:
                    CodeFile.RemoveFile(CodeType.Robot, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Robot[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveMqtt:
                    CodeFile.RemoveFile(CodeType.Mqtt, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Mqtt[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveTask:
                    CodeFile.RemoveFile(CodeType.Task, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Task[{json.UUID}]已删除"
                    };
                    break;
                case ReType.UpdataDll:
                    File = CodeFile.GetDll(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Dll[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Dll[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenDll.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataClass:
                    File = CodeFile.GetClass(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Class[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Class[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenClass.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataSocket:
                    File = CodeFile.GetSocket(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Socket[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Socket[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenSocket.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataRobot:
                    File = CodeFile.GetRobot(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Robot[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Robot[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenRobot.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataWebSocket:
                    File = CodeFile.GetWebSocket(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个WebSocket[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"WebSocket[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenWebSocket.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataMqtt:
                    File = CodeFile.GetMqtt(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Mqtt[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Mqtt[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenMqtt.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataTask:
                    File = CodeFile.GetTask(json.UUID);
                    if (File == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Task[{json.UUID}]"
                        };
                        break;
                    }
                    if (File.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Task[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    File.Code = FileEdit.StartEdit(File.Code, list);
                    File.Text = json.Text;

                    SW = new Stopwatch();
                    SW.Start();
                    BuildBack = GenTask.StartGen(File);
                    SW.Stop();
                    File.Up();
                    resObj = new ReMessage
                    {
                        Build = BuildBack.Isok,
                        Message = BuildBack.Res,
                        UseTime = SW.ElapsedMilliseconds.ToString(),
                        Time = BuildBack.Time
                    };
                    break;
                case ReType.UpdataWeb:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"没有这个Web[{json.UUID}]"
                        };
                        break;
                    }
                    if (File2.Version != json.Version)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]版本号错误"
                        };
                        break;
                    }

                    list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
                    if (!File2.Codes.TryGetValue(json.Temp, out var code))
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]不存在文件[{json.Temp}]"
                        };
                        break;
                    }
                    code = FileEdit.StartEdit(code, list);
                    File2.Text = json.Text;

                    HtmlUtils.Save(File2, json.Temp, code);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]文件[{json.Temp}]已修改",
                        UseTime = "0",
                        Time = File2.UpdateTime
                    };
                    break;
                case ReType.WebAddCode:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]没有找到"
                        };
                        break;
                    }
                    if (File2.Codes.ContainsKey(json.Code))
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]的源码文件[{json.Code}]已存在"
                        };
                        break;
                    }

                    HtmlUtils.AddCode(File2, json.Code, "");
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]已添加源码文件[{json.Code}]"
                    };
                    break;
                case ReType.WebRemoveFile:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]没有找到"
                        };
                        break;
                    }
                    if (json.Code == "index.html")
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]的主文件不允许删除"
                        };
                        break;
                    }
                    if (!File2.Codes.ContainsKey(json.Code) && !File2.Files.ContainsKey(json.Code))
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]的文件[{json.Code}]不存在"
                        };
                        break;
                    }

                    HtmlUtils.Remove(File2, json.Code);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]已删除文件[{json.Code}]"
                    };
                    break;
                case ReType.WebAddFile:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]没有找到"
                        };
                        break;
                    }
                    if (File2.Files.ContainsKey(json.Code))
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]的文件[{json.Code}]已存在"
                        };
                        break;
                    }

                    HtmlUtils.AddFile(File2, json.Code, BuildUtils.HexStringToByte(json.Temp));
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]已添加文件[{json.Code}]"
                    };
                    break;
                case ReType.RemoveWeb:
                    File2 = HtmlUtils.GetHtml(json.UUID);
                    if (File2 == null)
                    {
                        resObj = new ReMessage
                        {
                            Build = false,
                            Message = $"Web[{json.UUID}]没有找到"
                        };
                        break;
                    }

                    HtmlUtils.DeleteAll(File2);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Web[{json.UUID}]已删除"
                    };
                    break;
            }
        }
        else
        {
            resObj = new ReMessage
            {
                Build = false,
                Message = "233"
            };
        }
        return new HttpReturn
        {
            Data = resObj,
            Res = ResType.Json
        };
    }
}
