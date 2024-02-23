using ColoryrServer.Core.Utils;
using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColoryrServer.Core.Database;

public record QWebObj
{
    public string uuid { get; set; }
    public string text { get; set; }
    public int version { get; set; }
    public bool vue { get; set; }
    public string createtime { get; set; }
    public string updatetime { get; set; }
}

public record QWebObj1
{
    public string uuid { get; set; }
    public string name { get; set; }
    public byte[] data { get; set; }
    public string time { get; set; }
}

public record CWebObj
{
    public string uuid { get; set; }
    public string name { get; set; }
    public string code { get; set; }
    public string time { get; set; }
}

internal static class WebDatabase
{
    /// <summary>
    /// Web项目储存
    /// </summary>
    private static readonly string WebDataDB = ServerMain.RunLocal + "Codes/WebData.db";
    /// <summary>
    /// Web代码储存
    /// </summary>
    private static readonly string WebCodeDB = ServerMain.RunLocal + "Codes/WebCode.db";
    /// <summary>
    /// Web代码日志储存
    /// </summary>
    private static readonly string WebLogDB = ServerMain.RunLocal + "Codes/WebLog.db";
    /// <summary>
    /// Web文件储存
    /// </summary>
    private static readonly string WebFileDB = ServerMain.RunLocal + "Codes/WebFile.db";

    /// <summary>
    /// Web项目储存
    /// </summary>
    private static string WebDataConnStr;
    /// <summary>
    /// Web代码储存
    /// </summary>
    private static string WebCodeConnStr;
    /// <summary>
    /// Web代码日志储存
    /// </summary>
    private static string WebLogConnStr;
    /// <summary>
    /// Web文件储存
    /// </summary>
    private static string WebFileConnStr;

    public static void Start()
    {
        WebDataConnStr = new SqliteConnectionStringBuilder("Data Source=" + WebDataDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        WebCodeConnStr = new SqliteConnectionStringBuilder("Data Source=" + WebCodeDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        WebFileConnStr = new SqliteConnectionStringBuilder("Data Source=" + WebFileDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        using var dataSQL = new SqliteConnection(WebDataConnStr);
        using var codeSQL = new SqliteConnection(WebCodeConnStr);
        using var fileSQL = new SqliteConnection(WebFileConnStr);

        dataSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `version` integer,
  `vue` integer,
  `createtime` text,
  `updatetime` text
);");

        codeSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `code` text,
  `time` text
);");

        fileSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `data` blob,
  `time` text
);");

        //日志模式
        if (ServerMain.Config.CodeSetting.CodeLog)
        {
            WebLogConnStr = new SqliteConnectionStringBuilder("Data Source=" + WebLogDB)
            {
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();

            using var logSQL = new SqliteConnection(WebLogConnStr);
            //Web设定
            logSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `version` integer,
  `vue` integer,
  `time` text
);");

            logSQL.Execute(@"create table if not exists code (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `code` text,
  `time` text
);");

            logSQL.Execute(@"create table if not exists file (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `data` blob,
  `time` text
);");
        }

    }

    public static List<WebObj> LoadAll()
    {
        using var dataSQL = new SqliteConnection(WebDataConnStr);
        using var codeSQL = new SqliteConnection(WebCodeConnStr);
        using var fileSQL = new SqliteConnection(WebFileConnStr);

        var list1 = new List<WebObj>();
        var list = dataSQL.Query<QWebObj>("SELECT uuid,text,version,vue,createtime,updatetime FROM web");

        foreach (var item in list)
        {
            try
            {
                var obj = item.ToWeb();
                var arg = new { item.uuid };

                var clist = codeSQL.Query<CWebObj>("SELECT name,code FROM web WHERE uuid=@uuid",
                    arg);
                foreach (var item1 in clist)
                {
                    obj.Codes.Add(item1.name, item1.code);
                }

                var wlist = fileSQL.Query<QWebObj1>("SELECT name,data FROM web WHERE uuid=@uuid",
                    arg);
                foreach (var item1 in wlist)
                {
                    obj.Files.Add(item1.name, item1.data);
                }

                list1.Add(obj);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        return list1;
    }

    public static void RemoveCode(WebObj obj, string name)
    {
        using var codeSQL = new SqliteConnection(WebCodeConnStr);
        var arg = new { uuid = obj.UUID, name };
        var list = codeSQL.Query<CWebObj>("SELECT code FROM web WHERE uuid=@uuid AND name=@name", arg);
        if (list.Any())
        {
            codeSQL.Execute("DELETE FROM web WHERE uuid=@uuid AND name=@name", arg);
            if (ServerMain.Config.CodeSetting.CodeLog)
            {
                using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                var old = list.First().code;
                CodeLogSQL.Execute($"INSERT INTO code (uuid,name,code,time) VALUES(@uuid,@name,@code,@time)", new
                {
                    uuid = obj.UUID,
                    name,
                    code = old,
                    time = DateTime.Now.ToString()
                });
            }
        }
    }

    public static void RemoveFile(WebObj obj, string name)
    {
        using var fileSQL = new SqliteConnection(WebFileConnStr);
        var arg = new { uuid = obj.UUID, name };
        var list = fileSQL.Query<QWebObj1>("SELECT data FROM web WHERE uuid=@uuid AND name=@name", arg);
        if (list.Any())
        {
            fileSQL.Execute("DELETE FROM web WHERE uuid=@uuid AND name=@name", arg);
            if (ServerMain.Config.CodeSetting.CodeLog)
            {
                using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                var old = list.First().data;
                CodeLogSQL.Execute($"INSERT INTO file (uuid,name,data,time) VALUES(@uuid,@name,@data,@time)", new
                {
                    uuid = obj.UUID,
                    name,
                    data = old,
                    time = DateTime.Now.ToString()
                });
            }
        }
    }

    /// <summary>
    /// 保存代码文件
    /// </summary>
    /// <param name="obj">项目</param>
    /// <param name="name">文件名</param>
    /// <param name="code">内容</param>
    public static void StorageCode(WebObj obj, string name, string code)
    {
        try
        {
            using var CodeSQL = new SqliteConnection(WebCodeConnStr);
            var list = CodeSQL.Query<CWebObj>("SELECT code FROM web WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, name });
            string time = DateTime.Now.ToString();
            if (list.Any())
            {
                CodeSQL.Execute("UPDATE web SET code=@code,time=@time WHERE uuid=@uuid AND name=@name",
                    new { uuid = obj.UUID, name, code, time });
                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                    CodeLogSQL.Execute($"INSERT INTO code (uuid,name,code,time) VALUES(@uuid,@name,@code,@time)", new
                    {
                        uuid = obj.UUID,
                        name,
                        list.First().code,
                        time
                    });
                }
            }
            else
            {
                CodeSQL.Execute("INSERT INTO web (uuid,name,code,time) VALUES(@uuid,@name,@code,@time)",
                    new { uuid = obj.UUID, name, code, time });
            }
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }

    public static void StorageFile(WebObj obj, string name, byte[] data)
    {
        using var fileSQL = new SqliteConnection(WebFileConnStr);
        string time = DateTime.Now.ToString();
        var list = fileSQL.Query<QWebObj1>("SELECT data FROM web WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, name });
        if (list.Any())
        {
            fileSQL.Execute("UPDATE web SET data=@data,time=@time WHERE uuid=@uuid AND name=@name",
                new { uuid = obj.UUID, data, name, time });

            if (ServerMain.Config.CodeSetting.CodeLog)
            {
                using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                byte[] old = list.First().data;
                CodeLogSQL.Execute($"INSERT INTO file (uuid,name,data,time) VALUES(@uuid,@name,@data,@time)", new
                {
                    uuid = obj.UUID,
                    name,
                    data = old,
                    time
                });
            }
        }
        else
        {
            fileSQL.Execute("INSERT INTO web (uuid,name,data,time) VALUES(@uuid,@name,@data,@time)",
                new { uuid = obj.UUID, name, data, time });
        }
    }

    public static void Storage(WebObj obj)
    {
        try
        {
            using var codeSQL = new SqliteConnection(WebDataConnStr);
            var list = codeSQL.Query<QWebObj>($"SELECT text,version,vue,createtime,updatetime FROM web WHERE uuid=@uuid",
                new { uuid = obj.UUID });

            var obj1 = new
            {
                uuid = obj.UUID,
                text = obj.Text,
                version = obj.Version,
                createtime = obj.CreateTime,
                updatetime = obj.UpdateTime,
                vue = obj.IsVue
            };

            if (list.Any())
            {
                codeSQL.Execute($"UPDATE web SET text=@text,version=@version,createtime=@createtime,updatetime=@updatetime,vue=@vue WHERE uuid=@uuid", obj1);

                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                    var old = list.First();
                    CodeLogSQL.Execute($"INSERT INTO web (uuid,text,version,vue,time) VALUES(@uuid,@text,@version,@vue,@time)",
                    new
                    {
                        uuid = obj.UUID,
                        text = obj.Text,
                        version = obj.Version,
                        time = DateTime.Now.ToString(),
                        vue = obj.IsVue
                    });
                }
            }
            else
            {
                codeSQL.Execute($"INSERT INTO web (uuid,text,version,vue,createtime,updatetime) VALUES(@uuid,@text,@version,@vue,@createtime,@updatetime)", obj1);
            }
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }

    public static void Remove(string uuid)
    {
        using var dataSQL = new SqliteConnection(WebDataConnStr);
        using var fileSQL = new SqliteConnection(WebFileConnStr);
        using var codeSQL = new SqliteConnection(WebCodeConnStr);
        var arg = new { uuid };
        fileSQL.Execute("DELETE FROM web WHERE uuid=@uuid", arg);
        codeSQL.Execute("DELETE FROM web WHERE uuid=@uuid", arg);
        dataSQL.Execute("DELETE FROM web WHERE uuid=@uuid", arg);
    }
}
