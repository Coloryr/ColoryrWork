using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.Core.Http;
using ColoryrServer.Core.Http.PostBuild;
using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Linq;

namespace ColoryrServer.Core.DllManager.PostBuild;

public static class PostBuild
{
    private static readonly string DB = ServerMain.RunLocal + "Login.db";
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
        if (json.Mode == PostBuildType.Login)
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
        else if (json.Mode == PostBuildType.Check)
        {
            resObj = new ReMessage
            {
                Build = Check(json.User, json.UUID),
                Message = "自动登录"
            };
        }
        else if (Check(json.User, json.Token))
        {
            resObj = json.Mode switch
            {
                PostBuildType.AddDll => PostBuildDll.Add(json),
                PostBuildType.AddClass => PostBuildClass.Add(json),
                PostBuildType.AddSocket => PostBuildSocket.Add(json),
                PostBuildType.AddWebSocket => PostBuildWebSocket.Add(json),
                PostBuildType.AddRobot => PostBuildRobot.Add(json),
                PostBuildType.AddMqtt => PostBuildMqtt.Add(json),
                PostBuildType.AddTask => PostBuildTask.Add(json),
                PostBuildType.AddWeb => PostBuildWeb.Add(json),
                PostBuildType.GetDll => PostBuildDll.GetList(),
                PostBuildType.GetClass => PostBuildClass.GetList(),
                PostBuildType.GetSocket => PostBuildSocket.GetList(),
                PostBuildType.GetWebSocket => PostBuildWebSocket.GetList(),
                PostBuildType.GetRobot => PostBuildRobot.GetList(),
                PostBuildType.GetMqtt => PostBuildMqtt.GetList(),
                PostBuildType.GetTask => PostBuildTask.GetList(),
                PostBuildType.GetWeb => PostBuildWeb.GetList(),
                PostBuildType.CodeDll => CodeFileManager.GetDll(json.UUID),
                PostBuildType.CodeClass => PostBuildClass.GetCode(json),
                PostBuildType.CodeSocket => CodeFileManager.GetSocket(json.UUID),
                PostBuildType.CodeWebSocket => CodeFileManager.GetWebSocket(json.UUID),
                PostBuildType.CodeRobot => CodeFileManager.GetRobot(json.UUID),
                PostBuildType.CodeMqtt => CodeFileManager.GetMqtt(json.UUID),
                PostBuildType.CodeTask => CodeFileManager.GetTask(json.UUID),
                PostBuildType.CodeWeb => PostBuildWeb.GetCode(json),
                PostBuildType.GetApi => APIFile.list,
                PostBuildType.RemoveDll => PostBuildDll.Remove(json),
                PostBuildType.RemoveClass => PostBuildClass.Remove(json),
                PostBuildType.RemoveSocket => PostBuildSocket.Remove(json),
                PostBuildType.RemoveWebSocket => PostBuildWebSocket.Remove(json),
                PostBuildType.RemoveRobot => PostBuildRobot.Remove(json),
                PostBuildType.RemoveMqtt => PostBuildMqtt.Remove(json),
                PostBuildType.RemoveTask => PostBuildTask.Remove(json),
                PostBuildType.RemoveWeb => PostBuildWeb.Remove(json),
                PostBuildType.UpdataDll => PostBuildDll.Updata(json),
                PostBuildType.UpdataClass => PostBuildClass.Updata(json),
                PostBuildType.UpdataSocket => PostBuildSocket.Updata(json),
                PostBuildType.UpdataRobot => PostBuildRobot.Updata(json),
                PostBuildType.UpdataWebSocket => PostBuildWebSocket.Updata(json),
                PostBuildType.UpdataMqtt => PostBuildMqtt.Updata(json),
                PostBuildType.UpdataTask => PostBuildTask.Updata(json),
                PostBuildType.UpdataWeb => PostBuildWeb.Updata(json),
                PostBuildType.WebBuild => PostBuildWeb.Build(json),
                PostBuildType.WebBuildRes => PostBuildWeb.BuildRes(json),
                PostBuildType.WebAddCode => PostBuildWeb.AddCode(json),
                PostBuildType.WebRemoveFile => PostBuildWeb.RemoveFile(json),
                PostBuildType.WebAddFile => PostBuildWeb.WebAddFile(json),
                PostBuildType.WebSetIsVue => PostBuildWeb.SetIsVue(json),
                PostBuildType.WebCodeZIP => PostBuildWeb.ZIP(json),
                PostBuildType.AddClassFile => PostBuildClass.AddFile(json),
                PostBuildType.RemoveClassFile => PostBuildClass.RemoveFile(json),
                PostBuildType.BuildClass => PostBuildClass.Build(json),
                PostBuildType.WebDownloadFile => PostBuildWeb.Download(json),
                PostBuildType.GetServerHttpConfigList => PostServerConfig.GetHttpConfigList(json),
                PostBuildType.AddServerHttpConfig => PostServerConfig.AddHttpConfig(json),
                PostBuildType.RemoveServerHttpConfig => PostServerConfig.RemoveHttpConfig(json),
                PostBuildType.AddServerHttpUrlRoute => PostServerConfig.AddHttpUrlRouteConfig(json),
                PostBuildType.RemoveServerHttpUrlRoute => PostServerConfig.RemoveHttpUrlRouteConfig(json),
                PostBuildType.GetServerSocketConfig => PostServerConfig.GetSocketConfig(),
                PostBuildType.GetRobotConfig => PostServerConfig.GetRobotConfig(),
                PostBuildType.ServerReboot => PostServerConfig.Reboot(),
                _ => new ReMessage
                {
                    Build = false,
                    Message = "null"
                }
            };
        }
        else
        {
            resObj = HttpReturnSave.Res404;
        }
        return new HttpReturn
        {
            Data = resObj,
            Res = ResType.Json
        };
    }
}
