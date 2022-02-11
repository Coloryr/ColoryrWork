using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrWork.Lib.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace ColoryrServer.DllManager
{
    public class DllStonge
    {
        private static readonly ConcurrentDictionary<string, DllBuildSave> DllList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> ClassList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> SocketList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> WebSocketList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> RobotList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> MqttList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> TaskList = new();

        private static readonly ConcurrentDictionary<string, AppBuildSave> AppList = new();

        public static readonly string DllLocal = ServerMain.RunLocal + @"Dll/Dll/";
        public static readonly string ClassLocal = ServerMain.RunLocal + @"Dll/Class/";
        public static readonly string SocketLocal = ServerMain.RunLocal + @"Dll/Socket/";
        public static readonly string WebSocketLocal = ServerMain.RunLocal + @"Dll/WebSocket/";
        public static readonly string RobotLocal = ServerMain.RunLocal + @"Dll/Robot/";
        public static readonly string MqttLocal = ServerMain.RunLocal + @"Dll/Mqtt/";
        public static readonly string TaskLocal = ServerMain.RunLocal + @"Dll/Task/";

        public static readonly string AppLocal = ServerMain.RunLocal + @"Dll/App/";

        private static void RemoveAll(string dir)
        {
            if (File.Exists(dir + ".dll"))
            {
                File.Delete(dir + ".dll");
            }
            if (File.Exists(dir + ".pdb"))
            {
                File.Delete(dir + ".pdb");
            }
        }

        public static void AddDll(string uuid, DllBuildSave save)
        {
            if (DllList.TryRemove(uuid, out var v))
            {
                v.Assembly.Unload();
                v.DllType = null;
                v.MethodInfos.Clear();
            }

            DllList.TryAdd(uuid, save);
        }
        public static void RemoveDll(string uuid)
        {
            if (DllList.TryRemove(uuid, out var v))
            {
                v.Assembly.Unload();
                v.DllType = null;
                v.MethodInfos.Clear();
            }
            RemoveAll(DllLocal + uuid);
        }
        public static DllBuildSave GetDll(string uuid)
        {
            if (DllList.TryGetValue(uuid, out var save))
            {
                return save;
            }
            else
                return null;
        }

        public static void AddClass(string uuid, DllBuildSave save)
        {
            if (ClassList.ContainsKey(uuid))
            {
                ClassList[uuid].Assembly.Unload();
                ClassList[uuid].DllType = null;
                ClassList[uuid].MethodInfos.Clear();
                ClassList[uuid] = save;
            }
            else
            {
                ClassList.TryAdd(uuid, save);
            }
        }
        public static void RemoveClass(string uuid)
        {
            if (ClassList.ContainsKey(uuid))
            {
                ClassList[uuid].Assembly.Unload();
                ClassList[uuid].DllType = null;
                ClassList[uuid].MethodInfos.Clear();
                ClassList.TryRemove(uuid, out var item);
            }
            RemoveAll(ClassLocal + uuid);
        }
        public static DllBuildSave GetClass(string uuid)
        {
            if (ClassList.TryGetValue(uuid, out var save))
            {
                return save;
            }
            else
                return null;
        }

        public static void AddSocket(string uuid, DllBuildSave save)
        {
            if (SocketList.ContainsKey(uuid))
            {
                SocketList[uuid].Assembly.Unload();
                SocketList[uuid].DllType = null;
                SocketList[uuid].MethodInfos.Clear();
                SocketList[uuid] = save;
            }
            else
            {
                SocketList.TryAdd(uuid, save);
            }
        }
        public static void RemoveSocket(string uuid)
        {
            if (SocketList.ContainsKey(uuid))
            {
                SocketList[uuid].Assembly.Unload();
                SocketList[uuid].DllType = null;
                SocketList[uuid].MethodInfos.Clear();
                SocketList.TryRemove(uuid, out var item);
            }
            RemoveAll(SocketLocal + uuid);
        }

        public static void AddWebSocket(string uuid, DllBuildSave save)
        {
            if (WebSocketList.ContainsKey(uuid))
            {
                WebSocketList[uuid].Assembly.Unload();
                WebSocketList[uuid].DllType = null;
                WebSocketList[uuid].MethodInfos.Clear();
                WebSocketList[uuid] = save;
            }
            else
            {
                WebSocketList.TryAdd(uuid, save);
            }
        }
        public static void RemoveWebSocket(string uuid)
        {
            if (WebSocketList.ContainsKey(uuid))
            {
                WebSocketList[uuid].Assembly.Unload();
                WebSocketList[uuid].DllType = null;
                WebSocketList[uuid].MethodInfos.Clear();
                WebSocketList.TryRemove(uuid, out var item);
            }
            RemoveAll(WebSocketLocal + uuid);
        }
        public static List<DllBuildSave> GetWebSocket()
        {
            return new List<DllBuildSave>(WebSocketList.Values);
        }

        public static void AddRobot(string uuid, DllBuildSave save)
        {
            if (RobotList.ContainsKey(uuid))
            {
                RobotList[uuid].Assembly.Unload();
                RobotList[uuid].DllType = null;
                RobotList[uuid].MethodInfos.Clear();
                RobotList[uuid] = save;
            }
            else
            {
                RobotList.TryAdd(uuid, save);
            }
        }
        public static void RemoveRobot(string uuid)
        {
            if (RobotList.ContainsKey(uuid))
            {
                RobotList[uuid].Assembly.Unload();
                RobotList[uuid].DllType = null;
                RobotList[uuid].MethodInfos.Clear();
                RobotList.TryRemove(uuid, out var item);
            }
            RemoveAll(RobotLocal + uuid);
        }
        public static List<DllBuildSave> GetRobot()
        {
            return new List<DllBuildSave>(RobotList.Values);
        }

        public static void AddMqtt(string uuid, DllBuildSave save)
        {
            if (MqttList.ContainsKey(uuid))
            {
                MqttList[uuid].Assembly.Unload();
                MqttList[uuid].DllType = null;
                MqttList[uuid].MethodInfos.Clear();
                MqttList[uuid] = save;
            }
            else
            {
                MqttList.TryAdd(uuid, save);
            }
        }
        public static void RemoveMqtt(string uuid)
        {
            if (MqttList.ContainsKey(uuid))
            {
                MqttList[uuid].Assembly.Unload();
                MqttList[uuid].DllType = null;
                MqttList[uuid].MethodInfos.Clear();
                MqttList.TryRemove(uuid, out var item);
            }
            RemoveAll(MqttLocal + uuid);
        }
        public static List<DllBuildSave> GetMqtt()
        {
            return new List<DllBuildSave>(MqttList.Values);
        }

        public static void AddTask(string uuid, DllBuildSave save)
        {
            if (TaskList.ContainsKey(uuid))
            {
                TaskList[uuid].Assembly.Unload();
                TaskList[uuid].DllType = null;
                TaskList[uuid].MethodInfos.Clear();
                TaskList[uuid] = save;
            }
            else
            {
                TaskList.TryAdd(uuid, save);
            }
        }
        public static void RemoveTask(string uuid)
        {
            if (TaskList.ContainsKey(uuid))
            {
                TaskList[uuid].Assembly.Unload();
                TaskList[uuid].DllType = null;
                TaskList[uuid].MethodInfos.Clear();
                TaskList.TryRemove(uuid, out var item);
            }
            RemoveAll(RobotLocal + uuid);
        }
        public static DllBuildSave GetTask(string uuid)
        {
            return TaskList.TryGetValue(uuid, out var dll) ? dll : null;
        }

        public static void AddApp(string uuid, AppBuildSave save)
        {
            if (AppList.ContainsKey(uuid))
            {
                AppList.TryRemove(uuid, out var item);
            }
            AppList.TryAdd(uuid, save);
        }
        public static void RemoveApp(string uuid)
        {
            if (AppList.ContainsKey(uuid))
            {
                AppList.TryRemove(uuid, out var item);
            }
            string local = AppLocal + uuid + "\\";
            if (File.Exists(local + "app.dll"))
            {
                File.Delete(local + "app.dll");
            }
            if (File.Exists(local + "app.pdb"))
            {
                File.Delete(local + "app.pdb");
            }
        }
        public static AppBuildSave GetApp(string uuid)
        {
            if (AppList.TryGetValue(uuid, out var save))
            {
                return save;
            }
            else
                return null;
        }

        public static void Start()
        {
            if (!Directory.Exists(DllLocal))
            {
                Directory.CreateDirectory(DllLocal);
            }
            if (!Directory.Exists(ClassLocal))
            {
                Directory.CreateDirectory(ClassLocal);
            }
            if (!Directory.Exists(SocketLocal))
            {
                Directory.CreateDirectory(SocketLocal);
            }
            if (!Directory.Exists(WebSocketLocal))
            {
                Directory.CreateDirectory(WebSocketLocal);
            }
            if (!Directory.Exists(RobotLocal))
            {
                Directory.CreateDirectory(RobotLocal);
            }
            if (!Directory.Exists(MqttLocal))
            {
                Directory.CreateDirectory(MqttLocal);
            }
            if (!Directory.Exists(AppLocal))
            {
                Directory.CreateDirectory(AppLocal);
            }
            if (!Directory.Exists(TaskLocal))
            {
                Directory.CreateDirectory(TaskLocal);
            }
            if (!Directory.Exists(MqttLocal))
            {
                Directory.CreateDirectory(MqttLocal);
            }
            var DllName = Function.GetPathFileName(DllLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
                    string uuid = FileItem.Name.Replace(".dll", "");
                    ServerMain.LogOut("加载DLL：" + uuid);
                    var AssemblySave = new DllBuildSave
                    {
                        Assembly = new AssemblyLoadContext(uuid, true)
                    };

                    var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                    if (File.Exists(pdb))
                    {
                        using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
                        AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                    }
                    else
                        AssemblySave.Assembly.LoadFromStream(FileStream);

                    AssemblySave.DllType = AssemblySave.Assembly.Assemblies.First().GetTypes()
                        .Where(x => x.Name == "app_" + uuid).First();
                    var Temp = AssemblySave.DllType.GetMethods();
                    foreach (var Item in Temp)
                    {
                        if (Item.Name is "Main" or "GetType" or "ToString"
                            or "Equals" or "GetHashCode")
                            continue;
                        AssemblySave.MethodInfos.Add(Item.Name, Item);
                    }
                    AddDll(uuid, AssemblySave);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            DllName = Function.GetPathFileName(ClassLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
                    string Name = FileItem.Name.Replace(".dll", "");
                    ServerMain.LogOut("加载Class：" + Name);
                    var AssemblySave = new DllBuildSave
                    {
                        Assembly = new AssemblyLoadContext(Name, true)
                    };

                    var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                    if (File.Exists(pdb))
                    {
                        using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
                        AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                    }
                    else
                        AssemblySave.Assembly.LoadFromStream(FileStream);

                    AssemblySave.DllType = AssemblySave.Assembly.Assemblies.First()
                        .GetTypes().Where(x => x.Name == Name).First();
                    AddClass(Name, AssemblySave);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            DllName = Function.GetPathFileName(SocketLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
                    string Name = FileItem.Name.Replace(".dll", "");
                    ServerMain.LogOut("加载Socket：" + Name);
                    var AssemblySave = new DllBuildSave
                    {
                        Assembly = new AssemblyLoadContext(Name, true)
                    };

                    var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                    if (File.Exists(pdb))
                    {
                        using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
                        AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                    }
                    else
                        AssemblySave.Assembly.LoadFromStream(FileStream);

                    AssemblySave.DllType = AssemblySave.Assembly.Assemblies.First()
                        .GetTypes().Where(x => x.Name == Name).First();
                    foreach (var Item in AssemblySave.DllType.GetMethods())
                    {
                        if (Item.Name is CodeDemo.SocketTcp or CodeDemo.SocketUdp)
                            AssemblySave.MethodInfos.Add(Item.Name, Item);
                    }
                    AddSocket(Name, AssemblySave);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            DllName = Function.GetPathFileName(WebSocketLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
                    string Name = FileItem.Name.Replace(".dll", "");
                    ServerMain.LogOut("加载WebSocket：" + Name);
                    var AssemblySave = new DllBuildSave
                    {
                        Assembly = new AssemblyLoadContext(Name, true)
                    };

                    var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                    if (File.Exists(pdb))
                    {
                        using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
                        AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                    }
                    else
                        AssemblySave.Assembly.LoadFromStream(FileStream);

                    AssemblySave.DllType = AssemblySave.Assembly.Assemblies.First()
                        .GetTypes().Where(x => x.Name == Name).First();
                    foreach (var Item in AssemblySave.DllType.GetMethods())
                    {
                        if (Item.Name is CodeDemo.WebSocketMessage or CodeDemo.WebSocketOpen or CodeDemo.WebSocketClose)
                            AssemblySave.MethodInfos.Add(Item.Name, Item);
                    }
                    AddWebSocket(Name, AssemblySave);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            DllName = Function.GetPathFileName(RobotLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
                    string Name = FileItem.Name.Replace(".dll", "");
                    ServerMain.LogOut("加载Robot：" + Name);
                    var AssemblySave = new DllBuildSave
                    {
                        Assembly = new AssemblyLoadContext(Name, true)
                    };

                    var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                    if (File.Exists(pdb))
                    {
                        using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
                        AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                    }
                    else
                        AssemblySave.Assembly.LoadFromStream(FileStream);

                    AssemblySave.DllType = AssemblySave.Assembly.Assemblies.First()
                        .GetTypes().Where(x => x.Name == Name).First();
                    foreach (var Item in AssemblySave.DllType.GetMethods())
                    {
                        if (Item.Name is CodeDemo.RobotMessage or CodeDemo.RobotEvent or CodeDemo.RobotSend)
                            AssemblySave.MethodInfos.Add(Item.Name, Item);
                    }
                    AddRobot(Name, AssemblySave);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            DllName = Function.GetPathFileName(MqttLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
                    string Name = FileItem.Name.Replace(".dll", "");
                    ServerMain.LogOut("加载MQTT：" + Name);
                    var AssemblySave = new DllBuildSave
                    {
                        Assembly = new AssemblyLoadContext(Name, true)
                    };

                    var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                    if (File.Exists(pdb))
                    {
                        using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
                        AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                    }
                    else
                        AssemblySave.Assembly.LoadFromStream(FileStream);

                    AssemblySave.DllType = AssemblySave.Assembly.Assemblies.First()
                        .GetTypes().Where(x => x.Name == Name).First();
                    foreach (var Item in AssemblySave.DllType.GetMethods())
                    {
                        if (Item.Name is CodeDemo.MQTTMessage or CodeDemo.MQTTValidator or CodeDemo.MQTTSubscription)
                            AssemblySave.MethodInfos.Add(Item.Name, Item);
                    }
                    AddMqtt(Name, AssemblySave);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            DllName = Function.GetPathFileName(TaskLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
                    string Name = FileItem.Name.Replace(".dll", "");
                    ServerMain.LogOut("加载Task：" + Name);
                    var AssemblySave = new DllBuildSave
                    {
                        Assembly = new AssemblyLoadContext(Name, true)
                    };

                    var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                    if (File.Exists(pdb))
                    {
                        using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
                        AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                    }
                    else
                        AssemblySave.Assembly.LoadFromStream(FileStream);

                    AssemblySave.DllType = AssemblySave.Assembly.Assemblies.First()
                        .GetTypes().Where(x => x.Name == Name).First();
                    foreach (var Item in AssemblySave.DllType.GetMethods())
                    {
                        if (Item.Name is CodeDemo.TaskRun)
                            AssemblySave.MethodInfos.Add(Item.Name, Item);
                    }
                    AddTask(Name, AssemblySave);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            var Dirs = Function.GetPathName(AppLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    var save = new AppBuildSave();
                    string Name = FileItem.Name;
                    var obj = CSFile.GetApp(Name);
                    if (obj == null)
                        continue;
                    ServerMain.LogOut("加载App：" + Name);
                    using (var FileStream = new FileStream(FileItem.FullName + "\\app.dll", FileMode.Open, FileAccess.Read))
                    {
                        save.Dll = new byte[FileStream.Length];
                        var pdb = FileItem.FullName + "app.pdb";
                        if (File.Exists(pdb))
                            using (var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read))
                            {
                                save.Pdb = new byte[FileStream1.Length];
                                FileStream1.Read(save.Pdb);
                            }
                        else
                            FileStream.Read(save.Dll);
                    }
                    AddApp(Name, save);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
        }
    }
}
