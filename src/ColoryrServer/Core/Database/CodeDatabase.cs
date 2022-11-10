using ColoryrServer.Core.Utils;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ColoryrServer.Core.Database;

public class QCodeObj
{
    public string uuid { get; set; }
    public string text { get; set; }
    public int version { get; set; }
    public string code { get; set; }
    public string createtime { get; set; }
    public string updatetime { get; set; }
    public CSFileCode ToCode()
    {
        return new()
        {
            UUID = uuid,
            Text = text,
            Version = version,
            CreateTime = createtime,
            UpdateTime = updatetime,
            Code = code
        };
    }
}

internal static class CodeDatabase
{
    private static readonly string DBLocal = ServerMain.RunLocal + "Codes";
    private static readonly string CodeDB = ServerMain.RunLocal + "Codes/Code.db";
    private static readonly string CodeLogDB = ServerMain.RunLocal + "Codes/CodeLog.db";

    private static string CodeConnStr;
    private static string CodeLogConnStr;

    public static void Start()
    {
        if (!Directory.Exists(DBLocal))
        {
            Directory.CreateDirectory(DBLocal);
        }

        CodeConnStr = new SqliteConnectionStringBuilder("Data Source=" + CodeDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        using var codeSQL = new SqliteConnection(CodeConnStr);

        //dll
        codeSQL.Execute(@"create table if not exists dll (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `createtime` text,
  `updatetime` text
);");
        //class
        codeSQL.Execute(@"create table if not exists class (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `version` integer,
  `createtime` text,
  `updatetime` text
);");
        codeSQL.Execute(@"create table if not exists classcode (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `code` text
);");

        //robot
        codeSQL.Execute(@"create table if not exists robot (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `createtime` text,
  `updatetime` text
);");

        //socket
        codeSQL.Execute(@"create table if not exists socket (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `createtime` text,
  `updatetime` text
);");

        //service
        codeSQL.Execute(@"create table if not exists service (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `createtime` text,
  `updatetime` text
);");

        //websocket
        codeSQL.Execute(@"create table if not exists websocket (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `createtime` text,
  `updatetime` text
);");

        //mqtt
        codeSQL.Execute(@"create table if not exists mqtt (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `createtime` text,
  `updatetime` text
);");
        if (ServerMain.Config.CodeSetting.CodeLog)
        {
            CodeLogConnStr = new SqliteConnectionStringBuilder("Data Source=" + CodeLogDB)
            {
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();
            using var codeLogSQL = new SqliteConnection(CodeLogConnStr);

            codeLogSQL.Execute(@"create table if not exists dll (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `user` text,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists class (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `version` integer,
  `user` text,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists classcode (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `code` text,
  `user` text,
  `time` text
);");

            codeLogSQL.Execute(@"create table if not exists robot (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `user` text,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists socket (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `user` text,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists service (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `user` text,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists websocket (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `user` text,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists mqtt (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `user` text,
  `time` text
);");
        }
    }

    public static void SaveCode(CSFileCode obj, string user, string name = null, string code = null)
    {
        using var codeSQL = new SqliteConnection(CodeConnStr);

        if (obj.Type == CodeType.Class)
        {
            QCodeObj old = null;
            var list = codeSQL.Query<QCodeObj>($"SELECT uuid,text,version,createtime,updatetime FROM class WHERE uuid=@uuid",
            new { uuid = obj.UUID });
            if (list.Any())
            {
                old = list.First();
                codeSQL.Execute($"UPDATE class SET text=@text,version=@version,createtime=@createtime,updatetime=@updatetime WHERE uuid=@uuid",
                    new
                    {
                        text = obj.Text,
                        version = obj.Version,
                        createtime = obj.CreateTime,
                        updatetime = obj.UpdateTime,
                        uuid = obj.UUID
                    });
            }
            else
            {
                codeSQL.Execute($"INSERT INTO class (uuid,text,version,createtime,updatetime) VALUES(@uuid,@text,@version,@createtime,@updatetime)",
                    new
                    {
                        text = obj.Text,
                        version = obj.Version,
                        createtime = obj.CreateTime,
                        updatetime = obj.UpdateTime,
                        uuid = obj.UUID
                    });
            }

            var list1 = codeSQL.Query<ClassCodeObj>("SELECT code FROM classcode WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, name });
            if (list1.Any())
            {
                old ??= obj.ToQCode();
                var obj2 = list1.First();
                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    using var codeLogSQL = new SqliteConnection(CodeLogConnStr);
                    codeLogSQL.Execute($"INSERT INTO classcode (uuid,name,code,user,time) VALUES(@uuid,@name,@code,@user,@time)", new
                    {
                        old.uuid,
                        old.code,
                        name,
                        user,
                        time = DateTime.Now.ToString()
                    });
                }

                codeSQL.Execute($"UPDATE classcode SET code=@code WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, code, name });
            }
            else
            {
                codeSQL.Execute($"INSERT INTO classcode (uuid,code,name) VALUES(@uuid,@code,@name)", new { uuid = obj.UUID, code, name });
            }
        }
        else
        {
            string type = obj.Type switch
            {
                CodeType.Dll => "dll",
                CodeType.Socket => "socket",
                CodeType.WebSocket => "websocket",
                CodeType.Robot => "robot",
                CodeType.Mqtt => "mqtt",
                CodeType.Service => "service",
                _ => throw new ErrorDump("code type error")
            };

            var list = codeSQL.Query<QCodeObj>($"SELECT uuid,text,version,code,createtime,updatetime FROM {type} WHERE uuid=@uuid",
                new { uuid = obj.UUID });
            if (list.Any())
            {
                var obj1 = list.First();
                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    using var codeLogSQL = new SqliteConnection(CodeLogConnStr);
                    codeLogSQL.Execute($"INSERT INTO {type} (uuid,text,code,version,user,time) VALUES(@uuid,@text,@code,@version,@user,@time)", new
                    {
                        obj1.uuid,
                        obj1.text,
                        obj1.code,
                        obj1.version,
                        user,
                        time = DateTime.Now.ToString()
                    });
                }

                codeSQL.Execute($"UPDATE {type} SET text=@text,code=@code,version=@version,createtime=@createtime,updatetime=@updatetime WHERE uuid=@uuid", new
                {
                    uuid = obj.UUID,
                    text = obj.Text,
                    code = obj.Code,
                    version = obj.Version,
                    createtime = obj.CreateTime,
                    updatetime = obj.UpdateTime
                });
            }
            else
            {
                codeSQL.Execute($"INSERT INTO {type} (uuid,text,code,version,createtime,updatetime) VALUES(@uuid,@text,@code,@version,@createtime,@updatetime)", new
                {
                    uuid = obj.UUID,
                    text = obj.Text,
                    code = obj.Code,
                    version = obj.Version,
                    createtime = obj.CreateTime,
                    updatetime = obj.UpdateTime
                });
            }
        }
    }

    public static void RemoveClassCode(string uuid, string name, string user)
    {
        using var codeSQL = new SqliteConnection(CodeConnStr);
        var arg = new { uuid, name };
        var list = codeSQL.Query<ClassCodeObj>($"SELECT code FROM classcode WHERE uuid=@uuid AND name=@name",
            arg);
        if (list.Any())
        {
            if (ServerMain.Config.CodeSetting.CodeLog)
            {
                ClassCodeObj obj1 = list.First();
                using var codeLogSQL = new SqliteConnection(CodeLogConnStr);
                codeLogSQL.Execute($"INSERT INTO classcode (uuid,name,code,user,time) VALUES(@uuid,@name,@code,@user,@time)", new
                { uuid, name, obj1.code, user, time = DateTime.Now.ToString() });
            }

            codeSQL.Execute("DELETE FROM class WHERE uuid=@uuid AND name=@name", arg);
        }
    }

    public static void RemoveCode(CodeType type, string uuid, string user)
    {
        if (type == CodeType.Class)
        {
            using var codeSQL = new SqliteConnection(CodeConnStr);
            var list = codeSQL.Query<QCodeObj>($"SELECT uuid,text,version,createtime,updatetime FROM class WHERE uuid=@uuid",
                new { uuid });
            if (list.Any())
            {
                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    QCodeObj obj1 = list.First();
                    using var codeLogSQL = new SqliteConnection(CodeLogConnStr);
                    codeLogSQL.Execute($"INSERT INTO class (uuid,text,version,user,time) VALUES(@uuid,@text,@version,@user,@time)", new
                    {
                        obj1.uuid,
                        obj1.text,
                        obj1.version,
                        user,
                        time = DateTime.Now.ToString()
                    });
                }

                codeSQL.Execute($"DELETE FROM class WHERE uuid=@uuid", new { uuid });
                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    var list1 = codeSQL.Query<ClassCodeObj>($"SELECT code,name FROM classcode WHERE uuid=@uuid", new { uuid });
                    using var codeLogSQL = new SqliteConnection(CodeLogConnStr);

                    foreach (var item in list1)
                    {
                        codeLogSQL.Execute($"INSERT INTO classcode (uuid,name,code,user,time) VALUES(@uuid,@name,@code,@user,@time)", new
                        {
                            uuid,
                            item.name,
                            item.code,
                            user,
                            time = DateTime.Now.ToString()
                        });
                    }
                }
                codeSQL.Execute($"DELETE FROM classcode WHERE uuid=@uuid", new { uuid });
            }
        }
        else
        {
            string type1 = type switch
            {
                CodeType.Dll => "dll",
                CodeType.Socket => "socket",
                CodeType.WebSocket => "websocket",
                CodeType.Robot => "robot",
                CodeType.Mqtt => "mqtt",
                CodeType.Service => "service",
                _ => throw new ErrorDump("code type error")
            };

            using var codeSQL = new SqliteConnection(CodeConnStr);
            var list = codeSQL.Query<QCodeObj>($"SELECT uuid,text,version,createtime,updatetime,code FROM {type1} WHERE uuid=@uuid",
                new { uuid });
            if (list.Any())
            {
                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    QCodeObj obj1 = list.First();
                    using var codeLogSQL = new SqliteConnection(CodeLogConnStr);
                    codeLogSQL.Execute($"INSERT INTO {type1} (uuid,text,version,time,user,code) VALUES(@uuid,@text,@version,@time,@user,@code)", new
                    {
                        obj1.uuid,
                        obj1.text,
                        obj1.code,
                        obj1.version,
                        user,
                        time = DateTime.Now.ToString()
                    });
                }

                codeSQL.Execute($"DELETE FROM {type1} WHERE uuid=@uuid", new { uuid });
            }
        }
    }

    public static List<ClassCodeObj> GetClassCode(string uuid)
    {
        using var codeSQL = new SqliteConnection(CodeConnStr);
        var list = codeSQL.Query<ClassCodeObj>("SELECT name,code FROM classcode WHERE uuid=@uuid", new { uuid });

        if (!list.Any())
            return null;

        return list.ToList();
    }
    public static ClassCodeObj CheckClassCode(CSFileCode obj, string name)
    {
        using var codeSQL = new SqliteConnection(CodeConnStr);
        var list = codeSQL.Query<ClassCodeObj>("SELECT name,code FROM classcode WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, name });

        return list.FirstOrDefault();
    }

    public static List<IEnumerable<QCodeObj>> GetCodes()
    {
        try
        {
            var list1 = new List<IEnumerable<QCodeObj>>();
            using var codeSQL = new SqliteConnection(CodeConnStr);
            var list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM dll");
            list1.Add(list);

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,createtime,updatetime FROM class");
            list1.Add(list);

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM socket");
            list1.Add(list);

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM websocket");
            list1.Add(list);

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM robot");
            list1.Add(list);

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM mqtt");
            list1.Add(list);

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM service");
            list1.Add(list);

            return list1;
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }

        return null;
    }
}
