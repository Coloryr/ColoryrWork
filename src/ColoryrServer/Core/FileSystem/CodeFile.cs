using ColoryrServer.Core.DllManager;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Dapper;
using System.Threading.Tasks;
using System.Linq;
using ColoryrServer.SDK;
using Microsoft.Data.Sqlite;

namespace ColoryrServer.Core.FileSystem;

internal record MAP
{
    public List<CSFileObj> DllList { get; set; }
    public List<CSFileObj> ClassList { get; set; }
    public List<CSFileObj> SocketList { get; set; }
    public List<CSFileObj> WebSocketList { get; set; }
    public List<CSFileObj> RobotList { get; set; }
    public List<CSFileObj> MqttList { get; set; }
    public List<CSFileObj> TaskList { get; set; }
}
internal class CodeFile
{
    private static readonly string DBLocal = ServerMain.RunLocal + @"Codes";
    private static readonly string CodeDB = ServerMain.RunLocal + @"Codes/Code.db";
    private static readonly string CodeLogDB = ServerMain.RunLocal + @"Codes/CodeLog.db";

    private static string CodeConnStr;
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
        using var CodeSQL = new SqliteConnection(CodeConnStr);

        CodeLogConnStr = new SqliteConnectionStringBuilder("Data Source=" + CodeLogDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        using var CodeLogSQL = new SqliteConnection(CodeLogConnStr);

        string sql = @"create table if not exists dll (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeSQL.Execute(sql);
        CodeLogSQL.Execute(sql);

        sql = @"create table if not exists class (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeSQL.Execute(sql);
        CodeLogSQL.Execute(sql);

        sql = @"create table if not exists robot (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeSQL.Execute(sql);
        CodeLogSQL.Execute(sql);

        sql = @"create table if not exists socket (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeSQL.Execute(sql);
        CodeLogSQL.Execute(sql);

        sql = @"create table if not exists task (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeSQL.Execute(sql);
        CodeLogSQL.Execute(sql);

        sql = @"create table if not exists websocket (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeSQL.Execute(sql);
        CodeLogSQL.Execute(sql);

        sql = @"create table if not exists mqtt (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeSQL.Execute(sql);
        CodeLogSQL.Execute(sql);

        LoadAll();
    }

    private static void Storage(string Local, object obj)
    {
        Task.Run(() =>
        {
            try
            {
                File.WriteAllText(Local, JsonConvert.SerializeObject(obj, Formatting.Indented));
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        });
    }

    private static void SaveCode(CSFileCode obj) 
    {
        string type = obj.Type switch
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
        var list = CodeSQL.Query<CSFileCode>($"SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM {type} WHERE UUID=@UUID",
            new { obj.UUID });
        if (list.Any())
        {
            CSFileCode obj1 = list.First();
            CodeLogSQL.Execute($"INSERT INTO {type} (UUID,Text,Code,Version,CreateTime,UpdateTime) VALUES(@UUID,@Text,@Code,@Version,@CreateTime,@UpdateTime)", obj1);

            CodeSQL.Execute($"UPDATE {type} SET Text=@Text,Code=@Code,Version=@Version,CreateTime=@CreateTime,UpdateTime=@UpdateTime WHERE UUID=@UUID", obj);
        }
        else
        {
            CodeSQL.Execute($"INSERT INTO {type} (UUID,Text,Code,Version,CreateTime,UpdateTime) VALUES(@UUID,@Text,@Code,@Version,@CreateTime,@UpdateTime)", obj);
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

    public static void StorageDll(CSFileCode obj)
    {
        Task.Run(() => SaveCode(obj));
        if (DllFileList.ContainsKey(obj.UUID))
            DllFileList[obj.UUID] = obj;
        else
            DllFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageClass(CSFileCode obj)
    {
        Task.Run(() => SaveCode(obj));
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
                    DllStonge.RemoveDll(uuid);
                    break;
                case CodeType.Class:
                    ClassFileList.TryRemove(uuid, out var item1);
                    DllStonge.RemoveClass(uuid);
                    break;
                case CodeType.Socket:
                    SocketFileList.TryRemove(uuid, out var item2);
                    DllStonge.RemoveSocket(uuid);
                    break;
                case CodeType.WebSocket:
                    WebSocketFileList.TryRemove(uuid, out var item3);
                    DllStonge.RemoveWebSocket(uuid);
                    break;
                case CodeType.Robot:
                    RobotFileList.TryRemove(uuid, out var item4);
                    DllStonge.RemoveRobot(uuid);
                    break;
                case CodeType.Mqtt:
                    MqttFileList.TryRemove(uuid, out var item6);
                    DllStonge.RemoveMqtt(uuid);
                    break;
                case CodeType.Task:
                    MqttFileList.TryRemove(uuid, out var item7);
                    DllStonge.RemoveTask(uuid);
                    break;
            }

            ServerMain.LogOut("删除:" + uuid);
            RemoveCode(type, uuid);

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
            using var CodeSQL = new SqliteConnection(CodeConnStr);
            var list = CodeSQL.Query<CSFileCode>("SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM dll");
            foreach (var item in list)
            {
                item.Type = CodeType.Dll;
                DllFileList.TryAdd(item.UUID, item);
            }

            list = CodeSQL.Query<CSFileCode>("SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM class");
            foreach (var item in list)
            {
                item.Type = CodeType.Class;
                ClassFileList.TryAdd(item.UUID, item);
            }

            list = CodeSQL.Query<CSFileCode>("SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM socket");
            foreach (var item in list)
            {
                item.Type = CodeType.Socket;
                SocketFileList.TryAdd(item.UUID, item);
            }

            list = CodeSQL.Query<CSFileCode>("SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM websocket");
            foreach (var item in list)
            {
                item.Type = CodeType.WebSocket;
                WebSocketFileList.TryAdd(item.UUID, item);
            }

            list = CodeSQL.Query<CSFileCode>("SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM robot");
            foreach (var item in list)
            {
                item.Type = CodeType.Robot;
                RobotFileList.TryAdd(item.UUID, item);
            }

            list = CodeSQL.Query<CSFileCode>("SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM mqtt");
            foreach (var item in list)
            {
                item.Type = CodeType.Mqtt;
                MqttFileList.TryAdd(item.UUID, item);
            }

            list = CodeSQL.Query<CSFileCode>("SELECT UUID,Text,Version,CreateTime,UpdateTime,Code FROM task");
            foreach (var item in list)
            {
                item.Type = CodeType.Task;
                TaskFileList.TryAdd(item.UUID, item);
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
                var MAP = new MAP
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
