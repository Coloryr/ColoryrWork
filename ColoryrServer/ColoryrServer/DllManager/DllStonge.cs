﻿using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using Lib.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;

namespace ColoryrServer.DllManager
{
    internal class DllStonge
    {
        private static readonly Dictionary<string, DllBuildSave> DllList = new();
        private static readonly Dictionary<string, DllBuildSave> ClassList = new();
        private static readonly Dictionary<string, DllBuildSave> IoTList = new();
        private static readonly Dictionary<string, DllBuildSave> WebSocketList = new();
        private static readonly Dictionary<string, DllBuildSave> RobotList = new();

        private static readonly Dictionary<string, AppBuildSave> AppList = new();

        public static readonly string DllLocal = ServerMain.RunLocal + @"Dll/Dll/";
        public static readonly string ClassLocal = ServerMain.RunLocal + @"Dll/Class/";
        public static readonly string IoTLocal = ServerMain.RunLocal + @"Dll/IoT/";
        public static readonly string WebSocketLocal = ServerMain.RunLocal + @"Dll/WebSocket/";
        public static readonly string RobotLocal = ServerMain.RunLocal + @"Dll/Robot/";

        public static readonly string AppLocal = ServerMain.RunLocal + @"Dll/App/";

        private static readonly ReaderWriterLockSlim Lock1 = new();
        private static readonly ReaderWriterLockSlim Lock2 = new();
        private static readonly ReaderWriterLockSlim Lock3 = new();
        private static readonly ReaderWriterLockSlim Lock4 = new();
        private static readonly ReaderWriterLockSlim Lock5 = new();
        private static readonly ReaderWriterLockSlim Lock6 = new();
        private static readonly ReaderWriterLockSlim Lock7 = new();

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
            Lock1.EnterWriteLock();
            try
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
                    DllList.Add(uuid, save);
                }
            }
            finally
            {
                Lock1.ExitWriteLock();
            }
        }
        public static void RemoveDll(string uuid)
        {
            Lock1.EnterReadLock();
            try
            {
                if (DllList.ContainsKey(uuid))
                {
                    DllList[uuid].Assembly.Unload();
                    DllList[uuid].Type = null;
                    DllList[uuid].MethodInfos.Clear();
                    DllList.Remove(uuid);
                }
                RemoveAll(DllLocal + uuid);
            }
            finally
            {
                Lock1.ExitReadLock();
            }
        }
        public static DllBuildSave GetDll(string uuid)
        {
            Lock1.EnterReadLock();
            try
            {
                if (DllList.TryGetValue(uuid, out var save))
                {
                    return save;
                }
                else
                    return null;
            }
            finally
            {
                Lock1.ExitReadLock();
            }
        }

        public static void AddClass(string uuid, DllBuildSave save)
        {
            Lock2.EnterWriteLock();
            try
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
                    ClassList.Add(uuid, save);
                }
            }
            finally
            {
                Lock2.ExitWriteLock();
            }
        }
        public static void RemoveClass(string uuid)
        {
            Lock2.EnterReadLock();
            try
            {
                if (ClassList.ContainsKey(uuid))
                {
                    ClassList[uuid].Assembly.Unload();
                    ClassList[uuid].Type = null;
                    ClassList[uuid].MethodInfos.Clear();
                    ClassList.Remove(uuid);
                }
                RemoveAll(ClassLocal + uuid);
            }
            finally
            {
                Lock2.ExitReadLock();
            }
        }
        public static DllBuildSave GetClass(string uuid)
        {
            Lock2.EnterReadLock();
            try
            {
                if (ClassList.TryGetValue(uuid, out var save))
                {
                    return save;
                }
                else
                    return null;
            }
            finally
            {
                Lock2.ExitReadLock();
            }
        }

        public static void AddIoT(string uuid, DllBuildSave save)
        {
            Lock3.EnterWriteLock();
            try
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
                    IoTList.Add(uuid, save);
                }
            }
            finally
            {
                Lock3.ExitWriteLock();
            }
        }
        public static void RemoveIoT(string uuid)
        {
            Lock3.EnterReadLock();
            try
            {
                if (IoTList.ContainsKey(uuid))
                {
                    IoTList[uuid].Assembly.Unload();
                    IoTList[uuid].Type = null;
                    IoTList[uuid].MethodInfos.Clear();
                    IoTList.Remove(uuid);
                }
                RemoveAll(IoTLocal + uuid);
            }
            finally
            {
                Lock3.ExitReadLock();
            }
        }
        public static DllBuildSave GetIoT(string uuid)
        {
            Lock3.EnterReadLock();
            try
            {
                if (IoTList.TryGetValue(uuid, out var save))
                {
                    return save;
                }
                else
                    return null;
            }
            finally
            {
                Lock3.ExitReadLock();
            }
        }

        public static void AddWebSocket(string uuid, DllBuildSave save)
        {
            Lock4.EnterWriteLock();
            try
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
                    WebSocketList.Add(uuid, save);
                }
            }
            finally
            {
                Lock4.ExitWriteLock();
            }
        }
        public static void RemoveWebSocket(string uuid)
        {
            Lock4.EnterReadLock();
            try
            {
                if (WebSocketList.ContainsKey(uuid))
                {
                    WebSocketList[uuid].Assembly.Unload();
                    WebSocketList[uuid].Type = null;
                    WebSocketList[uuid].MethodInfos.Clear();
                    WebSocketList.Remove(uuid);
                }
                RemoveAll(WebSocketLocal + uuid);
            }
            finally
            {
                Lock4.ExitReadLock();
            }
        }
        public static List<DllBuildSave> GetWebSocket()
        {
            Lock4.EnterReadLock();
            try
            {
                return new List<DllBuildSave>(WebSocketList.Values);
            }
            finally
            {
                Lock4.ExitReadLock();
            }
        }

        public static void AddRobot(string uuid, DllBuildSave save)
        {
            Lock5.EnterWriteLock();
            try
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
                    RobotList.Add(uuid, save);
                }
            }
            finally
            {
                Lock5.ExitWriteLock();
            }
        }
        public static void RemoveRobot(string uuid)
        {
            Lock5.EnterReadLock();
            try
            {
                if (RobotList.ContainsKey(uuid))
                {
                    RobotList[uuid].Assembly.Unload();
                    RobotList[uuid].Type = null;
                    RobotList[uuid].MethodInfos.Clear();
                    RobotList.Remove(uuid);
                }
                RemoveAll(RobotLocal + uuid);
            }
            finally
            {
                Lock5.ExitReadLock();
            }
        }
        public static List<DllBuildSave> GetRobot()
        {
            Lock5.EnterReadLock();
            try
            {
                return new List<DllBuildSave>(RobotList.Values);
            }
            finally
            {
                Lock5.ExitReadLock();
            }
        }

        public static void AddApp(string uuid, AppBuildSave save)
        {
            Lock6.EnterWriteLock();
            try
            {
                if (AppList.ContainsKey(uuid))
                {
                    AppList.Remove(uuid);
                }
                AppList.Add(uuid, save);
            }
            finally
            {
                Lock6.ExitWriteLock();
            }
        }
        public static void RemoveApp(string uuid)
        {
            Lock6.EnterWriteLock();
            try
            {
                if (AppList.ContainsKey(uuid))
                {
                    AppList.Remove(uuid);
                }
            }
            finally
            {
                Lock6.ExitWriteLock();
            }
        }
        public static AppBuildSave GetApp(string uuid)
        {
            Lock6.EnterReadLock();
            try
            {
                if (AppList.TryGetValue(uuid, out var save))
                {
                    return save;
                }
                else
                    return null;
            }
            finally
            {
                Lock6.ExitReadLock();
            }
        }

        public void DynamicInit()
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
                        var AssemblySave = new DllBuildSave();
                        AssemblySave.Assembly = new AssemblyLoadContext(uuid, true);

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
                        var AssemblySave = new DllBuildSave();
                        AssemblySave.Assembly = new AssemblyLoadContext(Name, true);

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
                        var AssemblySave = new DllBuildSave();
                        AssemblySave.Assembly = new AssemblyLoadContext(Name, true);

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
                            if (Item.Name == "main")
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
                        var AssemblySave = new DllBuildSave();
                        AssemblySave.Assembly = new AssemblyLoadContext(Name, true);

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
                        var AssemblySave = new DllBuildSave();
                        AssemblySave.Assembly = new AssemblyLoadContext(Name, true);

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
