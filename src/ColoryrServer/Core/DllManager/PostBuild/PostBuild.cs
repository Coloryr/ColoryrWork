using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.Core.Http;
using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
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
            resObj = json.Mode switch
            {
                ReType.AddDll => PostBuildDll.Add(json),
                ReType.AddClass => PostBuildClass.Add(json),
                ReType.AddSocket => PostBuildSocket.Add(json),
                ReType.AddWebSocket => PostBuildWebSocket.Add(json),
                ReType.AddRobot => PostBuildRobot.Add(json),
                ReType.AddMqtt => PostBuildMqtt.Add(json),
                ReType.AddTask => PostBuildTask.Add(json),
                ReType.AddWeb => PostBuildWeb.Add(json),
                ReType.GetDll => PostBuildDll.GetList(),
                ReType.GetClass => PostBuildClass.GetList(),
                ReType.GetSocket => PostBuildSocket.GetList(),
                ReType.GetWebSocket => PostBuildWebSocket.GetList(),
                ReType.GetRobot => PostBuildRobot.GetList(),
                ReType.GetMqtt => PostBuildMqtt.GetList(),
                ReType.GetTask => PostBuildTask.GetList(),
                ReType.GetWeb => PostBuildWeb.GetList(),
                ReType.CodeDll => CodeFileManager.GetDll(json.UUID),
                ReType.CodeClass => PostBuildClass.GetCode(json),
                ReType.CodeSocket => CodeFileManager.GetSocket(json.UUID),
                ReType.CodeWebSocket => CodeFileManager.GetWebSocket(json.UUID),
                ReType.CodeRobot => CodeFileManager.GetRobot(json.UUID),
                ReType.CodeMqtt => CodeFileManager.GetMqtt(json.UUID),
                ReType.CodeTask => CodeFileManager.GetTask(json.UUID),
                ReType.CodeWeb => PostBuildWeb.GetCode(json),
                ReType.GetApi => APIFile.list,
                ReType.RemoveDll => PostBuildDll.Remove(json),
                ReType.RemoveClass => PostBuildClass.Remove(json),
                ReType.RemoveSocket => PostBuildSocket.Remove(json),
                ReType.RemoveWebSocket => PostBuildWebSocket.Remove(json),
                ReType.RemoveRobot => PostBuildRobot.Remove(json),
                ReType.RemoveMqtt => PostBuildMqtt.Remove(json),
                ReType.RemoveTask => PostBuildTask.Remove(json),
                ReType.RemoveWeb => PostBuildWeb.Remove(json),
                ReType.UpdataDll => PostBuildDll.Updata(json),
                ReType.UpdataClass => PostBuildClass.Updata(json),
                ReType.UpdataSocket => PostBuildSocket.Updata(json),
                ReType.UpdataRobot => PostBuildRobot.Updata(json),
                ReType.UpdataWebSocket => PostBuildWebSocket.Updata(json),
                ReType.UpdataMqtt => PostBuildMqtt.Updata(json),
                ReType.UpdataTask => PostBuildTask.Updata(json),
                ReType.UpdataWeb => PostBuildWeb.Updata(json),
                ReType.WebBuild => PostBuildWeb.Build(json),
                ReType.WebBuildRes => PostBuildWeb.BuildRes(json),
                ReType.WebAddCode => PostBuildWeb.AddCode(json),
                ReType.WebRemoveFile => PostBuildWeb.RemoveFile(json),
                ReType.WebAddFile => PostBuildWeb.WebAddFile(json),
                ReType.WebSetIsVue => PostBuildWeb.SetIsVue(json),
                ReType.WebCodeZIP => PostBuildWeb.ZIP(json),
                ReType.AddClassFile => PostBuildClass.AddFile(json),
                ReType.RemoveClassFile => PostBuildClass.RemoveFile(json),
                ReType.BuildClass => PostBuildClass.Build(json),
                ReType.WebDownloadFile => PostBuildWeb.Download(json),
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
