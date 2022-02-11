﻿using ColoryrServer.DllManager;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.FileSystem
{
    public record MAP
    {
        public List<CSFileObj> DllList { get; set; }
        public List<CSFileObj> ClassList { get; set; }
        public List<CSFileObj> SocketList { get; set; }
        public List<CSFileObj> WebSocketList { get; set; }
        public List<CSFileObj> RobotList { get; set; }
        public List<CSFileObj> MqttList { get; set; }
        public List<CSFileObj> TaskList { get; set; }
        public List<AppFileObj> AppList { get; set; }
    }
    public class CSFile
    {
        private static readonly string DllFileLocal = ServerMain.RunLocal + @"Codes/Dll/";
        private static readonly string ClassFileLocal = ServerMain.RunLocal + @"Codes/Class/";
        private static readonly string SocketFileLocal = ServerMain.RunLocal + @"Codes/Socket/";
        private static readonly string WebSocketFileLocal = ServerMain.RunLocal + @"/Codes/WebScoket/";
        private static readonly string RobotFileLocal = ServerMain.RunLocal + @"Codes/Robot/";
        private static readonly string MqttFileLocal = ServerMain.RunLocal + @"Codes/Mqtt/";
        private static readonly string TaskFileLocal = ServerMain.RunLocal + @"Codes/Task/";
        private static readonly string AppFileLocal = ServerMain.RunLocal + @"Codes/App/";

        private static readonly string DllMap = ServerMain.RunLocal + @"DllMap.json";
        private static readonly string RemoveDir = ServerMain.RunLocal + @"Removes/";

        public static readonly ConcurrentDictionary<string, CSFileCode> DllFileList = new();
        public static readonly ConcurrentDictionary<string, CSFileCode> ClassFileList = new();
        public static readonly ConcurrentDictionary<string, CSFileCode> SocketFileList = new();
        public static readonly ConcurrentDictionary<string, CSFileCode> WebSocketFileList = new();
        public static readonly ConcurrentDictionary<string, CSFileCode> RobotFileList = new();
        public static readonly ConcurrentDictionary<string, CSFileCode> MqttFileList = new();
        public static readonly ConcurrentDictionary<string, CSFileCode> TaskFileList = new();
        public static readonly ConcurrentDictionary<string, AppFileObj> AppFileList = new();

        public static void Start()
        {
            if (!Directory.Exists(DllFileLocal))
            {
                Directory.CreateDirectory(DllFileLocal);
            }
            if (!Directory.Exists(ClassFileLocal))
            {
                Directory.CreateDirectory(ClassFileLocal);
            }
            if (!Directory.Exists(SocketFileLocal))
            {
                Directory.CreateDirectory(SocketFileLocal);
            }
            if (!Directory.Exists(WebSocketFileLocal))
            {
                Directory.CreateDirectory(WebSocketFileLocal);
            }
            if (!Directory.Exists(RobotFileLocal))
            {
                Directory.CreateDirectory(RobotFileLocal);
            }
            if (!Directory.Exists(RemoveDir))
            {
                Directory.CreateDirectory(RemoveDir);
            }
            if (!Directory.Exists(AppFileLocal))
            {
                Directory.CreateDirectory(AppFileLocal);
            }
            if (!Directory.Exists(MqttFileLocal))
            {
                Directory.CreateDirectory(MqttFileLocal);
            }
            if (!Directory.Exists(TaskFileLocal))
            {
                Directory.CreateDirectory(TaskFileLocal);
            }
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

        public static void StorageDll(CSFileCode obj)
        {
            var url = DllFileLocal + obj.UUID + ".json";
            if (DllFileList.ContainsKey(obj.UUID))
                DllFileList[obj.UUID] = obj;
            else
                DllFileList.TryAdd(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageClass(CSFileCode obj)
        {
            var url = ClassFileLocal + obj.UUID + ".json";
            if (ClassFileList.ContainsKey(obj.UUID))
                ClassFileList[obj.UUID] = obj;
            else
                ClassFileList.TryAdd(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageSocket(CSFileCode obj)
        {
            var url = SocketFileLocal + obj.UUID + ".json";
            if (SocketFileList.ContainsKey(obj.UUID))
                SocketFileList[obj.UUID] = obj;
            else
                SocketFileList.TryAdd(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageWebSocket(CSFileCode obj)
        {
            var url = WebSocketFileLocal + obj.UUID + ".json";
            if (WebSocketFileList.ContainsKey(obj.UUID))
                WebSocketFileList[obj.UUID] = obj;
            else
                WebSocketFileList.TryAdd(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageRobot(CSFileCode obj)
        {
            var url = RobotFileLocal + obj.UUID + ".json";
            if (RobotFileList.ContainsKey(obj.UUID))
                RobotFileList[obj.UUID] = obj;
            else
                RobotFileList.TryAdd(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageMqtt(CSFileCode obj)
        {
            var url = MqttFileLocal + obj.UUID + ".json";
            if (MqttFileList.ContainsKey(obj.UUID))
                MqttFileList[obj.UUID] = obj;
            else
                MqttFileList.TryAdd(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageTask(CSFileCode obj)
        {
            var url = TaskFileLocal + obj.UUID + ".json";
            if (TaskFileList.ContainsKey(obj.UUID))
                TaskFileList[obj.UUID] = obj;
            else
                TaskFileList.TryAdd(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }

        public static bool AddFileApp(AppFileObj item, UploadObj obj, Stream baseStream)
        {
            var file = DllStonge.AppLocal + obj.UUID + "\\" + obj.FileName;
            if (File.Exists(file))
            {
                return false;
            }
            else
            {
                using FileStream data = File.Open(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                baseStream.CopyTo(data);
                data.Flush();
                item.Files.Add(obj.FileName, obj.FileName);
                StorageApp(item);
                return true;
            }
        }
        public static void StorageApp(AppFileObj obj)
        {
            var url = AppFileLocal + obj.UUID + ".json";
            if (AppFileList.ContainsKey(obj.UUID))
                AppFileList[obj.UUID] = obj;
            else
                AppFileList.TryAdd(obj.UUID, obj);
            Storage(url, obj);
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
        public static AppFileObj GetApp(string uuid)
        {
            if (AppFileList.TryGetValue(uuid, out var save))
            {
                return save;
            }
            return null;
        }

        public static void RemoveFile(CodeType type, string uuid)
        {
            try
            {
                string Name = "";
                dynamic obj = null;
                bool IsDir = false;
                switch (type)
                {
                    case CodeType.Dll:
                        Name = DllFileLocal + uuid + ".json";
                        obj = GetDll(uuid);
                        DllFileList.TryRemove(uuid, out var item);
                        DllStonge.RemoveDll(uuid);
                        break;
                    case CodeType.Class:
                        Name = ClassFileLocal + uuid + ".json";
                        obj = GetClass(uuid);
                        ClassFileList.TryRemove(uuid, out var item1);
                        DllStonge.RemoveClass(uuid);
                        break;
                    case CodeType.Socket:
                        Name = SocketFileLocal + uuid + ".json";
                        obj = GetSocket(uuid);
                        SocketFileList.TryRemove(uuid, out var item2);
                        DllStonge.RemoveSocket(uuid);
                        break;
                    case CodeType.WebSocket:
                        Name = WebSocketFileLocal + uuid + ".json";
                        obj = GetWebSocket(uuid);
                        WebSocketFileList.TryRemove(uuid, out var item3);
                        DllStonge.RemoveWebSocket(uuid);
                        break;
                    case CodeType.Robot:
                        Name = RobotFileLocal + uuid + ".json";
                        obj = GetRobot(uuid);
                        RobotFileList.TryRemove(uuid, out var item4);
                        DllStonge.RemoveRobot(uuid);
                        break;
                    case CodeType.Mqtt:
                        Name = MqttFileLocal + uuid + ".json";
                        obj = GetMqtt(uuid);
                        MqttFileList.TryRemove(uuid, out var item6);
                        DllStonge.RemoveMqtt(uuid);
                        break;
                    case CodeType.Task:
                        Name = TaskFileLocal + uuid + ".json";
                        obj = GetMqtt(uuid);
                        MqttFileList.TryRemove(uuid, out var item7);
                        DllStonge.RemoveTask(uuid);
                        break;
                    case CodeType.App:
                        Name = AppFileLocal + uuid + ".json";
                        obj = GetApp(uuid);
                        AppFileList.TryRemove(uuid, out var item5);
                        DllStonge.RemoveApp(uuid);
                        IsDir = true;
                        break;
                }
                if (obj == null)
                {
                    ServerMain.LogOut("无法删除:" + Name);
                    return;
                }
                if (File.Exists(Name))
                {
                    ServerMain.LogOut("删除:" + Name);
                    File.Delete(Name);
                }

                string time = string.Format("{0:s}", DateTime.Now).Replace(":", ".");
                if (!IsDir)
                {
                    string info =
$@"/*
UUID:{obj.UUID},
Text:{obj.Text},
Version:{obj.Version},
Type:{obj.Type.ToString()}
*/
";
                    File.WriteAllText(RemoveDir + $"{obj.Type.ToString()}[{uuid}]-{time}.cs", info + obj.Code);
                }
                else
                {
                    string dir = RemoveDir + $"{obj.Type.ToString()}[{uuid}]-{time}" + "\\";
                    Directory.CreateDirectory(dir);
                    string info =
$@"/*
UUID:{obj.UUID},
Text:{obj.Text},
Version:{obj.Version},
Type:{obj.Type.ToString()},
Key:{obj.Key}
*/
";
                    File.WriteAllText(dir + "info.txt", info);
                    foreach (var item in obj.Xamls)
                    {
                        File.WriteAllText(dir + item.Key + ".xaml", item.Value);
                    }
                    foreach (var item in obj.Codes)
                    {
                        File.WriteAllText(dir + item.Key + ".cs", item.Value);
                    }
                    foreach (var item in obj.Files)
                    {
                        File.Move(DllStonge.AppLocal + obj.uuid + "\\" + item, dir + item);
                    }
                }
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        public static void LoadAll()
        {
            foreach (var item in Function.GetPathFileName(DllFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    DllFileList.TryAdd(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            foreach (var item in Function.GetPathFileName(ClassFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    ClassFileList.TryAdd(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            foreach (var item in Function.GetPathFileName(SocketFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    SocketFileList.TryAdd(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            foreach (var item in Function.GetPathFileName(WebSocketFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    WebSocketFileList.TryAdd(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            foreach (var item in Function.GetPathFileName(RobotFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    RobotFileList.TryAdd(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            foreach (var item in Function.GetPathFileName(MqttFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    MqttFileList.TryAdd(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            foreach (var item in Function.GetPathFileName(TaskFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    TaskFileList.TryAdd(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            foreach (var item in Function.GetPathFileName(AppFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<AppFileObj>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    AppFileList.TryAdd(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
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
                        TaskList = new(TaskFileList.Values),
                        AppList = new(AppFileList.Values)
                    };
                    File.WriteAllText(DllMap, JsonConvert.SerializeObject(MAP, Formatting.Indented));
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            });
        }

        internal static ReMessage RemoveAppFile(string uuid, string text)
        {
            var file = DllStonge.AppLocal + uuid + "\\" + text;
            if (File.Exists(file))
            {
                File.Delete(file);
                return new ReMessage
                {
                    Build = true,
                    Message = "删除"
                };
            }
            else
            {
                return new ReMessage
                {
                    Build = true,
                    Message = "不存在文件"
                };
            }
        }
    }
}
