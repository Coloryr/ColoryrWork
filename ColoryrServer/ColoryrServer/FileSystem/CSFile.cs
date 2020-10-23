using Lib.Build.Object;
using Lib.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager
{
    class MAP
    {
        public List<CSFileObj> DllList { get; set; }
        public List<CSFileObj> ClassList { get; set; }
        public List<CSFileObj> IoTList { get; set; }
        public List<CSFileObj> WebSocketList { get; set; }
        public List<CSFileObj> RobotList { get; set; }
    }
    class CSFile
    {
        private static readonly string DllFileLocal = ServerMain.RunLocal + @"/CODE/Dll/";
        private static readonly string ClassFileLocal = ServerMain.RunLocal + @"/CODE/Class/";
        private static readonly string IoTFileLocal = ServerMain.RunLocal + @"/CODE/IoT/";
        private static readonly string WebSocketFileLocal = ServerMain.RunLocal + @"/CODE/WebScoket/";
        private static readonly string RobotFileLocal = ServerMain.RunLocal + @"/CODE/Robot/";

        private static readonly string DllMap = ServerMain.RunLocal + @"/DllMap.json";
        private static readonly string RemoveDir = ServerMain.RunLocal + @"/Remove/";

        public static readonly Dictionary<string, CSFileObj> DllFileList = new Dictionary<string, CSFileObj>();
        public static readonly Dictionary<string, CSFileObj> ClassFileList = new Dictionary<string, CSFileObj>();
        public static readonly Dictionary<string, CSFileObj> IoTFileList = new Dictionary<string, CSFileObj>();
        public static readonly Dictionary<string, CSFileObj> WebSocketFileList = new Dictionary<string, CSFileObj>();
        public static readonly Dictionary<string, CSFileObj> RobotFileList = new Dictionary<string, CSFileObj>();
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
            LoadAll();
        }

        public static bool CheckPermission(string uuid, string user)
        {
            if (DllFileList.ContainsKey(uuid))
            {
                return DllFileList[uuid].User == user;
            }
            return false;
        }

        private static void Storage(string Local, CSFileCode obj)
        {
            Task.Factory.StartNew(() =>
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
        public static CSFileCode GetDll(string uuid)
        {
            try
            {
                string Name = DllFileLocal + uuid + ".json";
                if (File.Exists(Name))
                    return JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(Name));
                else
                    return null;
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
            return null;
        }
        public static CSFileCode GetClass(string uuid)
        {
            try
            {
                string Name = ClassFileLocal + uuid + ".json";
                if (File.Exists(Name))
                    return JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(Name));
                else
                    return null;
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
            return null;
        }
        public static CSFileCode GetIoT(string uuid)
        {
            try
            {
                string Name = IoTFileLocal + uuid + ".json";
                if (File.Exists(Name))
                    return JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(Name));
                else
                    return null;
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
            return null;
        }
        public static CSFileCode GetWebSocket(string uuid)
        {
            try
            {
                string Name = WebSocketFileLocal + uuid + ".json";
                if (File.Exists(Name))
                    return JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(Name));
                else
                    return null;
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
            return null;
        }
        public static CSFileCode GetRobot(string uuid)
        {
            try
            {
                string Name = RobotFileLocal + uuid + ".json";
                if (File.Exists(Name))
                    return JsonConvert.DeserializeObject<CSFileCode>(File.ReadAllText(Name));
                else
                    return null;
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
            return null;
        }

        public static void RemoveFile(CodeType type, string uuid)
        {
            try
            {
                string Name = "";
                CSFileCode obj = null;
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
                File.WriteAllText(RemoveDir + uuid + "-" +
                    string.Format("{0:s}", DateTime.Now).Replace(":", ".") + ".cs", info + obj.Code);
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
                    var obj = JsonConvert.DeserializeObject<CSFileObj>(File.ReadAllText(item.FullName));
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
                    var obj = JsonConvert.DeserializeObject<CSFileObj>(File.ReadAllText(item.FullName));
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
                    var obj = JsonConvert.DeserializeObject<CSFileObj>(File.ReadAllText(item.FullName));
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
                    var obj = JsonConvert.DeserializeObject<CSFileObj>(File.ReadAllText(item.FullName));
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
                    var obj = JsonConvert.DeserializeObject<CSFileObj>(File.ReadAllText(item.FullName));
                    string name = item.Name.Replace(".json", "");
                    RobotFileList.Add(name, obj);
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
                    var MAP = new MAP();
                    MAP.ClassList = new List<CSFileObj>(ClassFileList.Values);
                    MAP.DllList = new List<CSFileObj>(DllFileList.Values);
                    MAP.IoTList = new List<CSFileObj>(IoTFileList.Values);
                    MAP.WebSocketList = new List<CSFileObj>(WebSocketFileList.Values);
                    MAP.RobotList = new List<CSFileObj>(RobotFileList.Values);
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
