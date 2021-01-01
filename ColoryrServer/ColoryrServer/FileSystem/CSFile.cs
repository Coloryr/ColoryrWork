using ColoryrServer.DllManager;
using Lib.Build.Object;
using Lib.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.FileSystem
{
    internal record MAP
    {
        public List<CSFileObj> DllList { get; set; }
        public List<CSFileObj> ClassList { get; set; }
        public List<CSFileObj> IoTList { get; set; }
        public List<CSFileObj> WebSocketList { get; set; }
        public List<CSFileObj> RobotList { get; set; }
        public List<CSFileObj> AppList { get; set; }
    }
    internal class CSFile
    {
        private static readonly string DllFileLocal = ServerMain.RunLocal + @"/CODE/Dll/";
        private static readonly string ClassFileLocal = ServerMain.RunLocal + @"/CODE/Class/";
        private static readonly string IoTFileLocal = ServerMain.RunLocal + @"/CODE/IoT/";
        private static readonly string WebSocketFileLocal = ServerMain.RunLocal + @"/CODE/WebScoket/";
        private static readonly string RobotFileLocal = ServerMain.RunLocal + @"/CODE/Robot/";
        private static readonly string AppFileLocal = ServerMain.RunLocal + @"/CODE/App/";

        private static readonly string DllMap = ServerMain.RunLocal + @"/DllMap.json";
        private static readonly string RemoveDir = ServerMain.RunLocal + @"/Remove/";

        public static readonly Dictionary<string, CSFileCode> DllFileList = new Dictionary<string, CSFileCode>();
        public static readonly Dictionary<string, CSFileCode> ClassFileList = new Dictionary<string, CSFileCode>();
        public static readonly Dictionary<string, CSFileCode> IoTFileList = new Dictionary<string, CSFileCode>();
        public static readonly Dictionary<string, CSFileCode> WebSocketFileList = new Dictionary<string, CSFileCode>();
        public static readonly Dictionary<string, CSFileCode> RobotFileList = new Dictionary<string, CSFileCode>();

        public static readonly Dictionary<string, AppFileObj> AppFileList = new Dictionary<string, AppFileObj>();

        public CSFile()
        {
            if (!Directory.Exists(DllFileLocal))
            {
                Directory.CreateDirectory(DllFileLocal);
            }
            if (!Directory.Exists(ClassFileLocal))
            {
                Directory.CreateDirectory(ClassFileLocal);
            }
            if (!Directory.Exists(IoTFileLocal))
            {
                Directory.CreateDirectory(IoTFileLocal);
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
            {
                DllFileList.Remove(obj.UUID);
            }
            DllFileList.Add(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageClass(CSFileCode obj)
        {
            var url = ClassFileLocal + obj.UUID + ".json";
            if (ClassFileList.ContainsKey(obj.UUID))
            {
                ClassFileList.Remove(obj.UUID);
            }
            ClassFileList.Add(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageIoT(CSFileCode obj)
        {
            var url = IoTFileLocal + obj.UUID + ".json";
            if (IoTFileList.ContainsKey(obj.UUID))
            {
                IoTFileList.Remove(obj.UUID);
            }
            IoTFileList.Add(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageWebSocket(CSFileCode obj)
        {
            var url = WebSocketFileLocal + obj.UUID + ".json";
            if (WebSocketFileList.ContainsKey(obj.UUID))
            {
                WebSocketFileList.Remove(obj.UUID);
            }
            WebSocketFileList.Add(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageRobot(CSFileCode obj)
        {
            var url = RobotFileLocal + obj.UUID + ".json";
            if (RobotFileList.ContainsKey(obj.UUID))
            {
                RobotFileList.Remove(obj.UUID);
            }
            RobotFileList.Add(obj.UUID, obj);
            Storage(url, obj);
            UpdataMAP();
        }
        public static void StorageApp(AppFileObj obj)
        {
            var url = AppFileLocal + obj.UUID + ".json";
            if (AppFileList.ContainsKey(obj.UUID))
            {
                AppFileList.Remove(obj.UUID);
            }
            AppFileList.Add(obj.UUID, obj);
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
        public static CSFileCode GetIoT(string uuid)
        {
            if (IoTFileList.TryGetValue(uuid, out var save))
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
                        DllFileList.Remove(uuid);
                        DllStonge.RemoveDll(uuid);
                        break;
                    case CodeType.Class:
                        Name = ClassFileLocal + uuid + ".json";
                        obj = GetClass(uuid);
                        ClassFileList.Remove(uuid);
                        DllStonge.RemoveClass(uuid);
                        break;
                    case CodeType.IoT:
                        Name = IoTFileLocal + uuid + ".json";
                        obj = GetIoT(uuid);
                        IoTFileList.Remove(uuid);
                        DllStonge.RemoveIoT(uuid);
                        break;
                    case CodeType.WebSocket:
                        Name = WebSocketFileLocal + uuid + ".json";
                        obj = GetWebSocket(uuid);
                        WebSocketFileList.Remove(uuid);
                        DllStonge.RemoveWebSocket(uuid);
                        break;
                    case CodeType.Robot:
                        Name = RobotFileLocal + uuid + ".json";
                        obj = GetRobot(uuid);
                        RobotFileList.Remove(uuid);
                        DllStonge.RemoveRobot(uuid);
                        break;
                    case CodeType.App:
                        Name = AppFileLocal + uuid + ".json";
                        obj = GetApp(uuid);
                        AppFileList.Remove(uuid);
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

                string info =
@"/*
UUID:{0},
Text:{1},
User:{2},
Version:{3},
Type:{4}
*/
";
                info = string.Format(info, obj.UUID, obj.Text, obj.User, obj.Version, obj.Type.ToString());
                if (!IsDir)
                {
                    File.WriteAllText(RemoveDir + uuid + "-" +
                        string.Format("{0:s}", DateTime.Now).Replace(":", ".") + ".cs", info + obj.Code);
                }
                else
                {
                    string dir = RemoveDir + uuid + "-" +
                        string.Format("{0:s}", DateTime.Now).Replace(":", ".") + "/";
                    Directory.CreateDirectory(dir);
                    File.WriteAllText(dir + "info.txt", info + obj.Code);
                    if (type == CodeType.App)
                        foreach (var item in obj.Xamls)
                        {
                            File.WriteAllText(dir + item.Key + ".xaml", item.Value);
                        }
                    foreach (var item in obj.Codes)
                    {
                        File.WriteAllText(dir + item.Key + ".cs", item.Value);
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
                    DllFileList.Add(name, obj);
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
                    ClassFileList.Add(name, obj);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            foreach (var item in Function.GetPathFileName(IoTFileLocal))
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    IoTFileList.Add(name, obj);
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
                    WebSocketFileList.Add(name, obj);
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
                    RobotFileList.Add(name, obj);
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
                    AppFileList.Add(name, obj);
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
                        ClassList = new List<CSFileObj>(ClassFileList.Values),
                        DllList = new List<CSFileObj>(DllFileList.Values),
                        IoTList = new List<CSFileObj>(IoTFileList.Values),
                        WebSocketList = new List<CSFileObj>(WebSocketFileList.Values),
                        RobotList = new List<CSFileObj>(RobotFileList.Values),
                        AppList = new List<CSFileObj>(AppFileList.Values)
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
}
