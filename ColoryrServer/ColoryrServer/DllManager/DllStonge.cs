﻿using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using Lib.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace ColoryrServer.DllManager
{
    internal class DllStonge
    {
        private static readonly ConcurrentDictionary<string, DllBuildSave> DllList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> ClassList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> IoTList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> WebSocketList = new();
        private static readonly ConcurrentDictionary<string, DllBuildSave> RobotList = new();

        private static readonly ConcurrentDictionary<string, AppBuildSave> AppList = new();

        public static readonly string DllLocal = ServerMain.RunLocal + @"Dll\Dll\";
        public static readonly string ClassLocal = ServerMain.RunLocal + @"Dll\Class\";
        public static readonly string IoTLocal = ServerMain.RunLocal + @"Dll\IoT\";
        public static readonly string WebSocketLocal = ServerMain.RunLocal + @"Dll\WebSocket\";
        public static readonly string RobotLocal = ServerMain.RunLocal + @"Dll\Robot\";

        public static readonly string AppLocal = ServerMain.RunLocal + @"Dll\App\";

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
            if (DllList.ContainsKey(uuid))
            {
                DllList[uuid].Assembly.Unload();
                DllList[uuid].Type = null;
                DllList[uuid].MethodInfos.Clear();
                DllList[uuid] = save;
            }
            else
            {
                DllList.TryAdd(uuid, save);
            }
        }
        public static void RemoveDll(string uuid)
        {
            if (DllList.ContainsKey(uuid))
            {
                DllList[uuid].Assembly.Unload();
                DllList[uuid].Type = null;
                DllList[uuid].MethodInfos.Clear();
                DllList.TryRemove(uuid, out var item);
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
                ClassList[uuid].Type = null;
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
                ClassList[uuid].Type = null;
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

        public static void AddIoT(string uuid, DllBuildSave save)
        {
            if (IoTList.ContainsKey(uuid))
            {
                IoTList[uuid].Assembly.Unload();
                IoTList[uuid].Type = null;
                IoTList[uuid].MethodInfos.Clear();
                IoTList[uuid] = save;
            }
            else
            {
                IoTList.TryAdd(uuid, save);
            }
        }
        public static void RemoveIoT(string uuid)
        {
            if (IoTList.ContainsKey(uuid))
            {
                IoTList[uuid].Assembly.Unload();
                IoTList[uuid].Type = null;
                IoTList[uuid].MethodInfos.Clear();
                IoTList.TryRemove(uuid, out var item);
            }
            RemoveAll(IoTLocal + uuid);
        }

        public static void AddWebSocket(string uuid, DllBuildSave save)
        {
            if (WebSocketList.ContainsKey(uuid))
            {
                WebSocketList[uuid].Assembly.Unload();
                WebSocketList[uuid].Type = null;
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
                WebSocketList[uuid].Type = null;
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
                RobotList[uuid].Type = null;
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
                RobotList[uuid].Type = null;
                RobotList[uuid].MethodInfos.Clear();
                RobotList.TryRemove(uuid, out var item);
            }
            RemoveAll(RobotLocal + uuid);
        }
        public static List<DllBuildSave> GetRobot()
        {
            return new List<DllBuildSave>(RobotList.Values);
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
            if (!Directory.Exists(IoTLocal))
            {
                Directory.CreateDirectory(IoTLocal);
            }
            if (!Directory.Exists(WebSocketLocal))
            {
                Directory.CreateDirectory(WebSocketLocal);
            }
            if (!Directory.Exists(RobotLocal))
            {
                Directory.CreateDirectory(RobotLocal);
            }
            if (!Directory.Exists(AppLocal))
            {
                Directory.CreateDirectory(AppLocal);
            }
            var DllName = Function.GetPathFileName(DllLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using (var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read))
                    {
                        string uuid = FileItem.Name.Replace(".dll", "");
                        ServerMain.LogOut("加载DLL：" + uuid);
                        var AssemblySave = new DllBuildSave
                        {
                            Assembly = new AssemblyLoadContext(uuid, true)
                        };

                        var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                        if (File.Exists(pdb))
                            using (var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read))
                            {
                                AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                            }
                        else
                            AssemblySave.Assembly.LoadFromStream(FileStream);

                        AssemblySave.Type = AssemblySave.Assembly.Assemblies.First().GetTypes()
                            .Where(x => x.Name == "app_" + uuid).First();
                        var Temp = AssemblySave.Type.GetMethods();
                        foreach (var Item in Temp)
                        {
                            if (Item.Name == "Main" || Item.Name == "GetType" || Item.Name == "ToString"
                                || Item.Name == "Equals" || Item.Name == "GetHashCode")
                                continue;
                            AssemblySave.MethodInfos.Add(Item.Name, Item);
                        }
                        AddDll(uuid, AssemblySave);
                    }
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
                    using (var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read))
                    {
                        string Name = FileItem.Name.Replace(".dll", "");
                        ServerMain.LogOut("加载Class：" + Name);
                        var AssemblySave = new DllBuildSave
                        {
                            Assembly = new AssemblyLoadContext(Name, true)
                        };

                        var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                        if (File.Exists(pdb))
                            using (var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read))
                            {
                                AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                            }
                        else
                            AssemblySave.Assembly.LoadFromStream(FileStream);

                        AssemblySave.Type = AssemblySave.Assembly.Assemblies.First()
                            .GetTypes().Where(x => x.Name == Name).First();
                        AddClass(Name, AssemblySave);
                    }
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
            DllName = Function.GetPathFileName(IoTLocal);
            foreach (var FileItem in DllName)
            {
                try
                {
                    if (FileItem.FullName.Contains(".pdb"))
                        continue;
                    using (var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read))
                    {
                        string Name = FileItem.Name.Replace(".dll", "");
                        ServerMain.LogOut("加载IoT：" + Name);
                        var AssemblySave = new DllBuildSave
                        {
                            Assembly = new AssemblyLoadContext(Name, true)
                        };

                        var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                        if (File.Exists(pdb))
                            using (var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read))
                            {
                                AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                            }
                        else
                            AssemblySave.Assembly.LoadFromStream(FileStream);

                        AssemblySave.Type = AssemblySave.Assembly.Assemblies.First()
                            .GetTypes().Where(x => x.Name == Name).First();
                        foreach (var Item in AssemblySave.Type.GetMethods())
                        {
                            if (Item.Name == "tcpmessage" || Item.Name == "udpmessage")
                                AssemblySave.MethodInfos.Add(Item.Name, Item);
                        }
                        AddIoT(Name, AssemblySave);
                    }
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
                    using (var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read))
                    {
                        string Name = FileItem.Name.Replace(".dll", "");
                        ServerMain.LogOut("加载WebSocket：" + Name);
                        var AssemblySave = new DllBuildSave
                        {
                            Assembly = new AssemblyLoadContext(Name, true)
                        };

                        var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                        if (File.Exists(pdb))
                            using (var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read))
                            {
                                AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                            }
                        else
                            AssemblySave.Assembly.LoadFromStream(FileStream);

                        AssemblySave.Type = AssemblySave.Assembly.Assemblies.First()
                            .GetTypes().Where(x => x.Name == Name).First();
                        foreach (var Item in AssemblySave.Type.GetMethods())
                        {
                            if (Item.Name == "main" || Item.Name == "open" || Item.Name == "close")
                                AssemblySave.MethodInfos.Add(Item.Name, Item);
                        }
                        AddWebSocket(Name, AssemblySave);
                    }
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
                    using (var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read))
                    {
                        string Name = FileItem.Name.Replace(".dll", "");
                        ServerMain.LogOut("加载Robot：" + Name);
                        var AssemblySave = new DllBuildSave
                        {
                            Assembly = new AssemblyLoadContext(Name, true)
                        };

                        var pdb = FileItem.FullName.Replace(".dll", ".pdb");
                        if (File.Exists(pdb))
                            using (var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read))
                            {
                                AssemblySave.Assembly.LoadFromStream(FileStream, FileStream1);
                            }
                        else
                            AssemblySave.Assembly.LoadFromStream(FileStream);

                        AssemblySave.Type = AssemblySave.Assembly.Assemblies.First()
                            .GetTypes().Where(x => x.Name == Name).First();
                        foreach (var Item in AssemblySave.Type.GetMethods())
                        {
                            if (Item.Name == "main" || Item.Name == "after" || Item.Name == "robot")
                                AssemblySave.MethodInfos.Add(Item.Name, Item);
                        }
                        AddRobot(Name, AssemblySave);
                    }
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
