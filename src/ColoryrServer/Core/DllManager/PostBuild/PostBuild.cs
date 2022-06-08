using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.Core.FileSystem.Html;
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

namespace ColoryrServer.Core.DllManager.PostBuild;

public static class PostBuild
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
        public DateTime Time { get; set; }
    }
    private static bool Check(string User, string UUID)
    {
        using var DBSQL = new SqliteConnection(connStr);
        var list = DBSQL.Query<LoginObj>("SELECT User,UUID,Time FROM login WHERE User=@User", new { User });
        if (!list.Any())
            return false;
        LoginObj item = list.First();
        if (item.UUID != UUID)
            return false;
        if (DateTime.Now - item.Time > TimeSpan.FromDays(7))
            return false;

        return true;
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
                    if (CodeFileManager.GetDll(json.UUID) == null)
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
                        CodeFileManager.StorageDll(File);
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
                    resObj = PostBuildClass.Add(json);
                    break;
                case ReType.AddSocket:
                    if (CodeFileManager.GetSocket(json.UUID) == null)
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
                        CodeFileManager.StorageSocket(File);
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
                    if (CodeFileManager.GetWebSocket(json.UUID) == null)
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
                        CodeFileManager.StorageWebSocket(File);
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
                    if (CodeFileManager.GetRobot(json.UUID) == null)
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
                        CodeFileManager.StorageRobot(File);
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
                    if (CodeFileManager.GetMqtt(json.UUID) == null)
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
                        CodeFileManager.StorageMqtt(File);
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
                    if (CodeFileManager.GetTask(json.UUID) == null)
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
                        CodeFileManager.StorageTask(File);
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
                    resObj = PostBuildWeb.Add(json);
                    break;
                case ReType.GetDll:
                    List = new CSFileList();
                    foreach (var item in CodeFileManager.DllFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetClass:
                    resObj = PostBuildClass.GetList();
                    break;
                case ReType.GetSocket:
                    List = new CSFileList();
                    foreach (var item in CodeFileManager.SocketFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetWebSocket:
                    List = new CSFileList();
                    foreach (var item in CodeFileManager.WebSocketFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetRobot:
                    List = new CSFileList();
                    foreach (var item in CodeFileManager.RobotFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetMqtt:
                    List = new CSFileList();
                    foreach (var item in CodeFileManager.MqttFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetTask:
                    List = new CSFileList();
                    foreach (var item in CodeFileManager.TaskFileList)
                    {
                        List.List.Add(item.Key, item.Value);
                    }
                    resObj = List;
                    break;
                case ReType.GetWeb:
                    resObj = PostBuildWeb.GetList();
                    break;
                case ReType.CodeDll:
                    resObj = CodeFileManager.GetDll(json.UUID);
                    break;
                case ReType.CodeClass:
                    resObj = PostBuildClass.GetCode(json);
                    break;
                case ReType.CodeSocket:
                    resObj = CodeFileManager.GetSocket(json.UUID);
                    break;
                case ReType.CodeWebSocket:
                    resObj = CodeFileManager.GetWebSocket(json.UUID);
                    break;
                case ReType.CodeRobot:
                    resObj = CodeFileManager.GetRobot(json.UUID);
                    break;
                case ReType.CodeMqtt:
                    resObj = CodeFileManager.GetMqtt(json.UUID);
                    break;
                case ReType.CodeTask:
                    resObj = CodeFileManager.GetTask(json.UUID);
                    break;
                case ReType.CodeWeb:
                    resObj = PostBuildWeb.GetCode(json);
                    break;
                case ReType.GetApi:
                    resObj = APIFile.list;
                    break;
                case ReType.RemoveDll:
                    CodeFileManager.RemoveFile(CodeType.Dll, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Dll[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveClass:
                    CodeFileManager.RemoveFile(CodeType.Class, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Class[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveSocket:
                    CodeFileManager.RemoveFile(CodeType.Socket, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Socket[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveWebSocket:
                    CodeFileManager.RemoveFile(CodeType.WebSocket, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"WebSocket[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveRobot:
                    CodeFileManager.RemoveFile(CodeType.Robot, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Robot[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveMqtt:
                    CodeFileManager.RemoveFile(CodeType.Mqtt, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Mqtt[{json.UUID}]已删除"
                    };
                    break;
                case ReType.RemoveTask:
                    CodeFileManager.RemoveFile(CodeType.Task, json.UUID);
                    resObj = new ReMessage
                    {
                        Build = true,
                        Message = $"Task[{json.UUID}]已删除"
                    };
                    break;
                case ReType.UpdataDll:
                    File = CodeFileManager.GetDll(json.UUID);
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
                    PostBuildClass.Updata(json);
                    break;
                case ReType.UpdataSocket:
                    File = CodeFileManager.GetSocket(json.UUID);
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
                    File = CodeFileManager.GetRobot(json.UUID);
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
                    File = CodeFileManager.GetWebSocket(json.UUID);
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
                    File = CodeFileManager.GetMqtt(json.UUID);
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
                    File = CodeFileManager.GetTask(json.UUID);
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
                    resObj = PostBuildWeb.Update(json);
                    break;
                case ReType.WebBuild:
                    resObj = PostBuildWeb.Build(json);
                    break;
                case ReType.WebAddCode:
                    resObj = PostBuildWeb.AddCode(json);
                    break;
                case ReType.WebRemoveFile:
                    resObj = PostBuildWeb.RemoveFile(json);
                    break;
                case ReType.WebAddFile:
                    resObj = PostBuildWeb.WebAddFile(json);
                    break;
                case ReType.RemoveWeb:
                    resObj = PostBuildWeb.Remove(json);
                    break;
                case ReType.WebSetIsVue:
                    resObj = PostBuildWeb.SetIsVue(json);
                    break;
                case ReType.AddClassFile:
                    resObj = PostBuildClass.AddFile(json);
                    break;
                case ReType.RemoveClassFile:
                    resObj = PostBuildClass.RemoveFile(json);
                    break;
                case ReType.BuildClass:
                    resObj = PostBuildClass.Build(json);
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
