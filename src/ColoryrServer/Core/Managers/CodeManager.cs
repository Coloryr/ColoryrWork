using ColoryrServer.Core.Database;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Build;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.Core.Managers;

internal static class CodeManager
{
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
        CodeDatabase.Start();

        ServerMain.OnStop += Stop;
        if (!Directory.Exists(RemoveDir))
        {
            Directory.CreateDirectory(RemoveDir);
        }

        LoadAll();
    }

    public static void StorageDll(CSFileCode obj, string user)
    {
        CodeDatabase.SaveCode(obj, user);
        if (DllFileList.ContainsKey(obj.UUID))
            DllFileList[obj.UUID] = obj;
        else
            DllFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageClass(CSFileCode obj, string file, string code, string user)
    {
        CodeDatabase.SaveCode(obj, user, file, code);
        if (ClassFileList.ContainsKey(obj.UUID))
            ClassFileList[obj.UUID] = obj;
        else
            ClassFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageSocket(CSFileCode obj, string user)
    {
        CodeDatabase.SaveCode(obj, user);
        if (SocketFileList.ContainsKey(obj.UUID))
            SocketFileList[obj.UUID] = obj;
        else
            SocketFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageWebSocket(CSFileCode obj, string user)
    {
        CodeDatabase.SaveCode(obj, user);
        if (WebSocketFileList.ContainsKey(obj.UUID))
            WebSocketFileList[obj.UUID] = obj;
        else
            WebSocketFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageRobot(CSFileCode obj, string user)
    {
        CodeDatabase.SaveCode(obj, user);
        if (RobotFileList.ContainsKey(obj.UUID))
            RobotFileList[obj.UUID] = obj;
        else
            RobotFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageMqtt(CSFileCode obj, string user)
    {
        CodeDatabase.SaveCode(obj, user);
        if (MqttFileList.ContainsKey(obj.UUID))
            MqttFileList[obj.UUID] = obj;
        else
            MqttFileList.TryAdd(obj.UUID, obj);
        UpdataMAP();
    }
    public static void StorageService(CSFileCode obj, string user)
    {
        CodeDatabase.SaveCode(obj, user);
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
            var obj = type switch
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
                var name = type switch
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
                    DllFileManager.RemoveDll(uuid);
                    break;
                case CodeType.Class:
                    ClassFileList.TryRemove(uuid, out var item1);
                    DllFileManager.RemoveClass(uuid);
                    break;
                case CodeType.Socket:
                    SocketFileList.TryRemove(uuid, out var item2);
                    DllFileManager.RemoveSocket(uuid);
                    break;
                case CodeType.WebSocket:
                    WebSocketFileList.TryRemove(uuid, out var item3);
                    DllFileManager.RemoveWebSocket(uuid);
                    break;
                case CodeType.Robot:
                    RobotFileList.TryRemove(uuid, out var item4);
                    DllFileManager.RemoveRobot(uuid);
                    break;
                case CodeType.Mqtt:
                    MqttFileList.TryRemove(uuid, out var item6);
                    DllFileManager.RemoveMqtt(uuid);
                    break;
                case CodeType.Service:
                    ServiceFileList.TryRemove(uuid, out var item7);
                    DllFileManager.RemoveService(uuid);
                    break;
            }

            ServerMain.LogOut($"[{user}]删除{type}[{uuid}]");
            string code = "";
            if (type == CodeType.Class)
            {
                var list = CodeDatabase.GetClassCode(uuid);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        code += $"/* {item.name} */" + Environment.NewLine +
                            item.code + Environment.NewLine;
                    }
                }
            }
            else
            {
                code = obj.Code;
            }
            CodeDatabase.RemoveCode(type, uuid, user);
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

            File.WriteAllText(RemoveDir + $"{obj.Type}[{uuid}]-{time}.cs", info + code);
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }

    public static void LoadAll()
    {
        var list = CodeDatabase.GetCodes();
        if (list == null)
            return;

        foreach (var item in list[0])
        {
            var obj = item.ToCode();
            obj.Type = CodeType.Dll;
            DllFileList.TryAdd(item.uuid, obj);
        }

        foreach (var item in list[1])
        {
            var obj = item.ToCode();
            obj.Type = CodeType.Class;
            ClassFileList.TryAdd(item.uuid, obj);
        }

        foreach (var item in list[2])
        {
            var obj = item.ToCode();
            obj.Type = CodeType.Socket;
            SocketFileList.TryAdd(item.uuid, obj);
        }

        foreach (var item in list[3])
        {
            var obj = item.ToCode();
            obj.Type = CodeType.WebSocket;
            WebSocketFileList.TryAdd(item.uuid, obj);
        }

        foreach (var item in list[4])
        {
            var obj = item.ToCode();
            obj.Type = CodeType.Robot;
            RobotFileList.TryAdd(item.uuid, obj);
        }

        foreach (var item in list[5])
        {
            var obj = item.ToCode();
            obj.Type = CodeType.Mqtt;
            MqttFileList.TryAdd(item.uuid, obj);
        }

        foreach (var item in list[6])
        {
            var obj = item.ToCode();
            obj.Type = CodeType.Service;
            ServiceFileList.TryAdd(item.uuid, obj);
        }

        UpdataMAP();
    }

    public static void UpdataMAP()
    {
        Task.Run(() =>
        {
            try
            {
                File.WriteAllText(DllMap, JsonUtils.ToString(new CodeFileMAP
                {
                    ClassList = new(ClassFileList.Values),
                    DllList = new(DllFileList.Values),
                    SocketList = new(SocketFileList.Values),
                    WebSocketList = new(WebSocketFileList.Values),
                    RobotList = new(RobotFileList.Values),
                    MqttList = new(MqttFileList.Values),
                    ServiceList = new(ServiceFileList.Values)
                }));
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        });
    }
}

internal record CodeFileMAP
{
    public List<CSFileObj> DllList { get; set; }
    public List<CSFileObj> ClassList { get; set; }
    public List<CSFileObj> SocketList { get; set; }
    public List<CSFileObj> WebSocketList { get; set; }
    public List<CSFileObj> RobotList { get; set; }
    public List<CSFileObj> MqttList { get; set; }
    public List<CSFileObj> ServiceList { get; set; }
}
