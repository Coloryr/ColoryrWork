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

namespace ColoryrServer.Core.FileSystem.Code;

internal static class CodeFileManager
{
    private static readonly string DBLocal = ServerMain.RunLocal + "Codes";
    private static readonly string CodeDB = ServerMain.RunLocal + "Codes/Code.db";
    private static readonly string CodeLogDB = ServerMain.RunLocal + "Codes/CodeLog.db";

    private static string CodeConnStr;
    private static string CodeLogConnStr;

    private static readonly string DllMap = ServerMain.RunLocal + "DllMap.json";
    private static readonly string RemoveDir = ServerMain.RunLocal + "Removes/";

    public static readonly ConcurrentDictionary<string, CSFileCode> DllFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> ClassFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> SocketFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> WebSocketFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> RobotFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> MqttFileList = new();
    public static readonly ConcurrentDictionary<string, CSFileCode> ServiceFileList = new();

    private static void Stop()
    {
        DllFileList.Clear();
        ClassFileList.Clear();
        SocketFileList.Clear();
        WebSocketFileList.Clear();
        RobotFileList.Clear();
        MqttFileList.Clear();
        ServiceFileList.Clear();
    }

    public static void Start()
    {
        ServerMain.OnStop += Stop;
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

        LoadAll();
    }

    private static void SaveCode(CSFileCode obj, string user, string name = null, string code = null)
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

    private static void RemoveCode(CodeType type, string uuid, string user)
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

    public static void StorageDll(CSFileCode obj, string user)
    {
        Task.Run(() => SaveCode(obj, user));
        if (DllFileList.ContainsKey(obj.UUID))
            DllFileList[obj.UUID] = obj;
        else
            DllFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageClass(CSFileCode obj, string file, string code, string user)
    {
        SaveCode(obj, user, file, code);
        if (ClassFileList.ContainsKey(obj.UUID))
            ClassFileList[obj.UUID] = obj;
        else
            ClassFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageSocket(CSFileCode obj, string user)
    {
        Task.Run(() => SaveCode(obj, user));
        if (SocketFileList.ContainsKey(obj.UUID))
            SocketFileList[obj.UUID] = obj;
        else
            SocketFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageWebSocket(CSFileCode obj, string user)
    {
        Task.Run(() => SaveCode(obj, user));
        if (WebSocketFileList.ContainsKey(obj.UUID))
            WebSocketFileList[obj.UUID] = obj;
        else
            WebSocketFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageRobot(CSFileCode obj, string user)
    {
        Task.Run(() => SaveCode(obj, user));
        if (RobotFileList.ContainsKey(obj.UUID))
            RobotFileList[obj.UUID] = obj;
        else
            RobotFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageMqtt(CSFileCode obj, string user)
    {
        Task.Run(() => SaveCode(obj, user));
        if (MqttFileList.ContainsKey(obj.UUID))
            MqttFileList[obj.UUID] = obj;
        else
            MqttFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageService(CSFileCode obj, string user)
    {
        Task.Run(() => SaveCode(obj, user));
        if (ServiceFileList.ContainsKey(obj.UUID))
            ServiceFileList[obj.UUID] = obj;
        else
            ServiceFileList.TryAdd(obj.UUID, obj);
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
    public static CSFileCode GetService(string uuid)
    {
        if (ServiceFileList.TryGetValue(uuid, out var save))
        {
            return save;
        }
        return null;
    }

    public static void RemoveFile(CodeType type, string uuid, string user)
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
                CodeType.Service => GetService(uuid),
                _ => null
            };

            if (obj == null)
            {
                string name = type switch
                {
                    CodeType.Dll => "Dll",
                    CodeType.Class => "Class",
                    CodeType.Socket => "Socket",
                    CodeType.WebSocket => "WebSocket",
                    CodeType.Robot => "Robot",
                    CodeType.Mqtt => "Mqtt",
                    CodeType.Service => "Service",
                    _ => null
                };
                ServerMain.LogWarn($"无法删除{name}[{uuid}]");
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
                case CodeType.Service:
                    ServiceFileList.TryRemove(uuid, out var item7);
                    DllStongeManager.RemoveService(uuid);
                    break;
            }

            ServerMain.LogOut($"[{user}]删除{type}[{uuid}]");
            RemoveCode(type, uuid, user);
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

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,createtime,updatetime FROM class");
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

            list = codeSQL.Query<QCodeObj>("SELECT uuid,text,version,code,createtime,updatetime FROM service");
            foreach (var item in list)
            {
                var obj = item.ToCode();
                obj.Type = CodeType.Service;
                ServiceFileList.TryAdd(item.uuid, obj);
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
                    ServiceList = new(ServiceFileList.Values)
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
