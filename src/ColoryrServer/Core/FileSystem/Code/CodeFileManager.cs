using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ColoryrServer.Core.FileSystem.Code;

internal static class CodeFileManager
{
    private static readonly string DBLocal = ServerMain.RunLocal + @"Codes";
    private static readonly string CodeDB = ServerMain.RunLocal + @"Codes/Code.db";
    private static readonly string CodeLogDB = ServerMain.RunLocal + @"Codes/CodeLog.db";
    private static readonly string CodeClassDB = ServerMain.RunLocal + @"Codes/CodeClass.db";

    private static string CodeConnStr;
    private static string CodeClassConnStr;
    private static string CodeLogConnStr;

    private static readonly string DllMap = ServerMain.RunLocal + @"DllMap.json";
    private static readonly string RemoveDir = ServerMain.RunLocal + @"Removes/";

    public static readonly ConcurrentDictionary<string, CSFileCode> DllFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> ClassFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> SocketFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> WebSocketFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> RobotFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> MqttFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> TaskFileList = new();

    public static void Start()
    {
        if (!Directory.Exists(DBLocal))
        {
            Directory.CreateDirectory(DBLocal);
        }
        if (!Directory.Exists(RemoveDir))
        {
            Directory.CreateDirectory(RemoveDir);
        }

        CodeConnStr = new SqliteConnectionStringBuilder("Data Source=" + CodeDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        using var codeSQL = new SqliteConnection(CodeConnStr);

        CodeClassConnStr = new SqliteConnectionStringBuilder("Data Source=" + CodeClassDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        using var codeClassSQL = new SqliteConnection(CodeClassConnStr);

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
        codeClassSQL.Execute(@"create table if not exists class (
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

        //task
        codeSQL.Execute(@"create table if not exists task (
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
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists class (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `name` text,
  `version` integer,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists robot (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists socket (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists task (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists websocket (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `time` text
);");
            codeLogSQL.Execute(@"create table if not exists mqtt (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `code` text,
  `version` integer,
  `time` text
);");
        }

        LoadAll();
    }

    private static void SaveCode(CSFileCode obj, string name = null, string code = null)
    {
        using var codeSQL = new SqliteConnection(CodeConnStr);

        if (obj.Type == CodeType.Class)
        {
            using var codeClassSQL = new SqliteConnection(CodeClassConnStr);
            QCodeObj old = null;
            var list = codeSQL.Query<QCodeObj>($"SELECT uuid,text,version,createtime,updatetime FROM class WHERE uuid=@uuid",
            new { uuid = obj.UUID });
            if (list.Any())
            {
                old = list.First();
                codeSQL.Execute($"UPDATE class SET text=@text,version=@version,createtime=@createtime,updatetime=@updatetime WHERE uuid=@uuid", 
                    new { text = obj.Text, version = obj.Version, createtime = obj.CreateTime, updatetime = obj.UpdateTime, uuid = obj.UUID });
            }
            else
            {
                codeSQL.Execute($"INSERT INTO class (uuid,text,version,createtime,updatetime) VALUES(@uuid,@text,@version,@createtime,@updatetime)", obj);
            }

            var list1 = codeClassSQL.Query<ClassCodeObj>("SELECT code FROM class WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, name });
            if (list1.Any())
            {
                if (old == null)
                    old = obj.ToQCode();
                var obj2 = list1.First();
                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    using var codeLogSQL = new SqliteConnection(CodeLogConnStr);
                    codeLogSQL.Execute($"INSERT INTO class (uuid,text,code,name,version,time) VALUES(@uuid,@text,@code,@name,@version,@time)", new
                    {
                        old.uuid,
                        old.text,
                        old.code,
                        name,
                        old.version, 
                        time = DateTime.Now.ToString()
                    });
                }

                codeClassSQL.Execute($"UPDATE class SET code=@code WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, code, name });
            }
            else
            {
                codeClassSQL.Execute($"INSERT INTO class (uuid,code,name) VALUES(@UUID,@Code,@File)", new { uuid = obj.UUID, code, name });
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
                CodeType.Task => "task",
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
                    codeLogSQL.Execute($"INSERT INTO {type} (uuid,text,code,version,time) VALUES(@uuid,@text,@code,@version,@time)", new 
                    {
                        obj1.uuid,
                        obj1.text,
                        obj1.code,
                        obj1.version,
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

    public static void RemoveClassCode(string uuid, string name)
    {
        using var CodeClassSQL = new SqliteConnection(CodeClassConnStr);
        var list = CodeClassSQL.Query<ClassCodeObj>($"SELECT code FROM class WHERE uuid=@uuid AND name=@name",
            new { uuid, name });
        if (list.Any())
        {
            if (ServerMain.Config.CodeSetting.CodeLog)
            {
                ClassCodeObj obj1 = list.First();
                using var codeLogSQL = new SqliteConnection(CodeLogConnStr);
                codeLogSQL.Execute($"INSERT INTO class (uuid,File,Code) VALUES(@UUID,@File,@Code)", new { uuid, name, obj1.code });
            }

            CodeClassSQL.Execute("DELETE FROM class WHERE uuid=@uuid AND name=@name", new { uuid, name });
        }
    }

    private static void RemoveCode(CodeType type, string UUID)
    {
        string type1 = type switch
        {
            CodeType.Dll => "dll",
            CodeType.Class => "class",
            CodeType.Socket => "socket",
            CodeType.WebSocket => "websocket",
            CodeType.Robot => "robot",
            CodeType.Mqtt => "mqtt",
            CodeType.Task => "task",
            _ => throw new ErrorDump("code type error")
        };

        using var CodeSQL = new SqliteConnection(CodeConnStr);
        using var CodeLogSQL = new SqliteConnection(CodeLogConnStr);
        var list = CodeSQL.Query<CSFileCode>($"SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM {type1} WHERE UUID=@UUID",
            new { UUID });
        if (list.Any())
        {
            CSFileCode obj1 = list.First();
            CodeLogSQL.Execute($"INSERT INTO {type1} (UUID,Text,Code,Version,CreateTime,UpdateTime) VALUES(@UUID,@Text,@Code,@Version,@CreateTime,@UpdateTime)", obj1);

            CodeSQL.Execute($"DELETE FROM {type1} WHERE UUID=@UUID", new { UUID });
        }
    }

    public static List<ClassCodeObj> GetClassCode(string uuid)
    {
        using var CodeClassSQL = new SqliteConnection(CodeClassConnStr);
        var list = CodeClassSQL.Query<ClassCodeObj>("SELECT name,code FROM class WHERE uuid=@uuid", new { uuid });

        if (!list.Any())
            return null;

        return list.ToList();
    }
    public static ClassCodeObj CheckClassCode(CSFileCode obj, string name)
    {
        using var CodeClassSQL = new SqliteConnection(CodeClassConnStr);
        var list = CodeClassSQL.Query<ClassCodeObj>("SELECT name,code FROM class WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, name });

        return list.FirstOrDefault();
    }

    public static void StorageDll(CSFileCode obj)
    {
        Task.Run(() => SaveCode(obj));
        if (DllFileList.ContainsKey(obj.UUID))
            DllFileList[obj.UUID] = obj;
        else
            DllFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageClass(CSFileCode obj, string file, string code)
    {
        SaveCode(obj, file, code);
        if (ClassFileList.ContainsKey(obj.UUID))
            ClassFileList[obj.UUID] = obj;
        else
            ClassFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageSocket(CSFileCode obj)
    {
        Task.Run(() => SaveCode(obj));
        if (SocketFileList.ContainsKey(obj.UUID))
            SocketFileList[obj.UUID] = obj;
        else
            SocketFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageWebSocket(CSFileCode obj)
    {
        Task.Run(() => SaveCode(obj));
        if (WebSocketFileList.ContainsKey(obj.UUID))
            WebSocketFileList[obj.UUID] = obj;
        else
            WebSocketFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageRobot(CSFileCode obj)
    {
        Task.Run(() => SaveCode(obj));
        if (RobotFileList.ContainsKey(obj.UUID))
            RobotFileList[obj.UUID] = obj;
        else
            RobotFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageMqtt(CSFileCode obj)
    {
        Task.Run(() => SaveCode(obj));
        if (MqttFileList.ContainsKey(obj.UUID))
            MqttFileList[obj.UUID] = obj;
        else
            MqttFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageTask(CSFileCode obj)
    {
        Task.Run(() => SaveCode(obj));
        if (TaskFileList.ContainsKey(obj.UUID))
            TaskFileList[obj.UUID] = obj;
        else
            TaskFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static CSFileCode GetDll(string uuid)
    {
        if (DllFileList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        return null;
    }
    public static CSFileCode GetClass(string uuid)
    {
        if (ClassFileList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        return null;
    }
    public static CSFileCode GetSocket(string uuid)
    {
        if (SocketFileList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        return null;
    }
    public static CSFileCode GetWebSocket(string uuid)
    {
        if (WebSocketFileList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        return null;
    }
    public static CSFileCode GetRobot(string uuid)
    {
        if (RobotFileList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        return null;
    }
    public static CSFileCode GetMqtt(string uuid)
    {
        if (MqttFileList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        return null;
    }
    public static CSFileCode GetTask(string uuid)
    {
        if (TaskFileList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        return null;
    }

    public static void RemoveFile(CodeType type, string uuid)
    {
        try
        {
            CSFileCode obj = type switch
            {
                CodeType.Dll => GetDll(uuid),
                CodeType.Class => GetClass(uuid),
                CodeType.Socket => GetSocket(uuid),
                CodeType.WebSocket => GetWebSocket(uuid),
                CodeType.Robot => GetRobot(uuid),
                CodeType.Mqtt => GetMqtt(uuid),
                CodeType.Task => GetMqtt(uuid),
                _ => null
            };

            if (obj == null)
            {
                ServerMain.LogOut("无法删除:" + uuid);
                return;
            }

            switch (type)
            {
                case CodeType.Dll:
                    DllFileList.TryRemove(uuid, out var item);
                    DllStongeManager.RemoveDll(uuid);
                    break;
                case CodeType.Class:
                    ClassFileList.TryRemove(uuid, out var item1);
                    DllStongeManager.RemoveClass(uuid);
                    break;
                case CodeType.Socket:
                    SocketFileList.TryRemove(uuid, out var item2);
                    DllStongeManager.RemoveSocket(uuid);
                    break;
                case CodeType.WebSocket:
                    WebSocketFileList.TryRemove(uuid, out var item3);
                    DllStongeManager.RemoveWebSocket(uuid);
                    break;
                case CodeType.Robot:
                    RobotFileList.TryRemove(uuid, out var item4);
                    DllStongeManager.RemoveRobot(uuid);
                    break;
                case CodeType.Mqtt:
                    MqttFileList.TryRemove(uuid, out var item6);
                    DllStongeManager.RemoveMqtt(uuid);
                    break;
                case CodeType.Task:
                    MqttFileList.TryRemove(uuid, out var item7);
                    DllStongeManager.RemoveTask(uuid);
                    break;
            }

            ServerMain.LogOut("删除:" + uuid);
            RemoveCode(type, uuid);
            uuid = uuid.Replace("/", "_");

            string time = string.Format("{0:s}", DateTime.Now).Replace(":", ".");
            string info =
$@"/*
UUID:{obj.UUID},
Text:{obj.Text},
Version:{obj.Version},
Type:{obj.Type}
*/
";
            File.WriteAllText(RemoveDir + $"{obj.Type}[{uuid}]-{time}.cs", info + obj.Code);
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }

    public static void LoadAll()
    {
        try
        {
            using var codeSQL = new SqliteConnection(CodeConnStr);
            var list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM dll");
            foreach (var item in list)
            {
                var obj = item.ToCode();
                obj.Type = CodeType.Dll;
                DllFileList.TryAdd(item.uuid, obj);
            }

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,createtime,updatetime FROM dll");
            foreach (var item in list)
            {
                var obj = item.ToCode();
                obj.Type = CodeType.Class;
                ClassFileList.TryAdd(item.uuid, obj);
            }

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM socket");
            foreach (var item in list)
            {
                var obj = item.ToCode();
                obj.Type = CodeType.Socket;
                SocketFileList.TryAdd(item.uuid, obj);
            }

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM websocket");
            foreach (var item in list)
            {
                var obj = item.ToCode();
                obj.Type = CodeType.WebSocket;
                WebSocketFileList.TryAdd(item.uuid, obj);
            }

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM robot");
            foreach (var item in list)
            {
                var obj = item.ToCode();
                obj.Type = CodeType.Robot;
                RobotFileList.TryAdd(item.uuid, obj);
            }

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM mqtt");
            foreach (var item in list)
            {
                var obj = item.ToCode();
                obj.Type = CodeType.Mqtt;
                MqttFileList.TryAdd(item.uuid, obj);
            }

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM task");
            foreach (var item in list)
            {
                var obj = item.ToCode();
                obj.Type = CodeType.Task;
                TaskFileList.TryAdd(item.uuid, obj);
            }
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }

        UpdataMAP();
    }
    public static void UpdataMAP()
    {
        Task.Run(() =>
        {
            try
            {
                var MAP = new CodeFileMAP
                {
                    ClassList = new(ClassFileList.Values),
                    DllList = new(DllFileList.Values),
                    SocketList = new(SocketFileList.Values),
                    WebSocketList = new(WebSocketFileList.Values),
                    RobotList = new(RobotFileList.Values),
                    MqttList = new(MqttFileList.Values),
                    TaskList = new(TaskFileList.Values)
                };
                File.WriteAllText(DllMap, JsonConvert.SerializeObject(MAP, Formatting.Indented));
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        });
    }
}
