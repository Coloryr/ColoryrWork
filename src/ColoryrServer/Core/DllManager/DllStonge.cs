using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrWork.Lib.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager;

public static class DllStonge
{
    private static readonly ConcurrentDictionary<string, DllBuildSave> MapDll = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> MapClass = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> MapSocket = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> MapWebSocket = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> MapRobot = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> MapMqtt = new();
    private static readonly ConcurrentDictionary<string, DllBuildSave> MapTask = new();

    public static readonly string LocalDll = ServerMain.RunLocal + @"Dll/Dll/";
    public static readonly string LocalClass = ServerMain.RunLocal + @"Dll/Class/";
    public static readonly string LocalSocket = ServerMain.RunLocal + @"Dll/Socket/";
    public static readonly string LocalWebSocket = ServerMain.RunLocal + @"Dll/WebSocket/";
    public static readonly string LocalRobot = ServerMain.RunLocal + @"Dll/Robot/";
    public static readonly string LocalMqtt = ServerMain.RunLocal + @"Dll/Mqtt/";
    public static readonly string LocalTask = ServerMain.RunLocal + @"Dll/Task/";

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

    public static DllBuildSave FindClass(AssemblyName name)
    {
        if (MapClass.TryGetValue(name.Name, out var item))
        {
            return item;
        }
        return null;
    }

    public static void AddDll(string uuid, DllBuildSave save)
    {
        if (MapDll.ContainsKey(uuid))
        {
            var old = MapDll[uuid];
            MapDll[uuid] = save;
            Task.Run(() =>
            {
                old.Unload();
                old.SelfType = null;
                old.MethodInfos.Clear();
                DllUseSave.Update(save);
            });
        }
        else
        {
            MapDll.TryAdd(uuid, save);
        }
    }
    public static void RemoveDll(string uuid)
    {
        if (MapDll.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(LocalDll + uuid);
    }
    public static DllBuildSave GetDll(string uuid)
    {
        if (MapDll.TryGetValue(uuid, out var save))
        {
            return save;
        }
        else
            return null;
    }

    public static void AddClass(string uuid, DllBuildSave save)
    {
        if (MapClass.ContainsKey(uuid))
        {
            var old = MapClass[uuid];
            MapClass[uuid] = save;
            Task.Run(() =>
            {
                old.Unload();
                old.SelfType = null;
                old.MethodInfos.Clear();
                DllUseSave.Update(save);
            });
        }
        else
        {
            MapClass.TryAdd(uuid, save);
        }
    }
    public static void RemoveClass(string uuid)
    {
        if (MapClass.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(LocalClass + uuid);
    }
    public static DllBuildSave GetClass(string uuid)
    {
        if (MapClass.TryGetValue(uuid, out var save))
        {
            return save;
        }
        else
            return null;
    }

    public static void AddSocket(string uuid, DllBuildSave save)
    {
        if (MapSocket.ContainsKey(uuid))
        {
            var old = MapDll[uuid];
            MapSocket[uuid] = save;
            Task.Run(() =>
            {
                old.Unload();
                old.SelfType = null;
                old.MethodInfos.Clear();
                DllUseSave.Update(save);
            });
        }
        else
        {
            MapSocket.TryAdd(uuid, save);
        }
    }
    public static void RemoveSocket(string uuid)
    {
        if (MapSocket.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(LocalSocket + uuid);
    }

    public static void AddWebSocket(string uuid, DllBuildSave save)
    {
        if (MapWebSocket.ContainsKey(uuid))
        {
            var old = MapDll[uuid];
            MapWebSocket[uuid] = save;
            Task.Run(() =>
            {
                old.Unload();
                old.SelfType = null;
                old.MethodInfos.Clear();
                DllUseSave.Update(save);
            });
        }
        else
        {
            MapWebSocket.TryAdd(uuid, save);
        }
    }
    public static void RemoveWebSocket(string uuid)
    {
        if (MapWebSocket.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(LocalWebSocket + uuid);
    }
    public static List<DllBuildSave> GetWebSocket()
    {
        return new List<DllBuildSave>(MapWebSocket.Values);
    }

    public static void AddRobot(string uuid, DllBuildSave save)
    {
        if (MapRobot.ContainsKey(uuid))
        {
            var old = MapDll[uuid];
            MapRobot[uuid] = save;
            Task.Run(() =>
            {
                old.Unload();
                old.SelfType = null;
                old.MethodInfos.Clear();
                DllUseSave.Update(save);
            });
        }
        else
        {
            MapRobot.TryAdd(uuid, save);
        }
    }
    public static void RemoveRobot(string uuid)
    {
        if (MapRobot.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(LocalRobot + uuid);
    }
    public static List<DllBuildSave> GetRobot()
    {
        return new List<DllBuildSave>(MapRobot.Values);
    }

    public static void AddMqtt(string uuid, DllBuildSave save)
    {
        if (MapMqtt.ContainsKey(uuid))
        {
            var old = MapDll[uuid];
            MapMqtt[uuid] = save;
            Task.Run(() =>
            {
                old.Unload();
                old.SelfType = null;
                old.MethodInfos.Clear();
                DllUseSave.Update(save);
            });
        }
        else
        {
            MapMqtt.TryAdd(uuid, save);
        }
    }
    public static void RemoveMqtt(string uuid)
    {
        if (MapMqtt.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(LocalMqtt + uuid);
    }
    public static List<DllBuildSave> GetMqtt()
    {
        return new List<DllBuildSave>(MapMqtt.Values);
    }

    public static void AddTask(string uuid, DllBuildSave save)
    {
        if (MapTask.ContainsKey(uuid))
        {
            var old = MapDll[uuid];
            MapTask[uuid] = save;
            Task.Run(() =>
            {
                old.Unload();
                old.SelfType = null;
                old.MethodInfos.Clear();
                DllUseSave.Update(save);
            });
        }
        else
        {
            MapTask.TryAdd(uuid, save);
        }
    }
    public static void RemoveTask(string uuid)
    {
        if (MapTask.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        RemoveAll(LocalRobot + uuid);
    }
    public static DllBuildSave GetTask(string uuid)
    {
        return MapTask.TryGetValue(uuid, out var dll) ? dll : null;
    }
    public static void Start()
    {
        if (!Directory.Exists(LocalDll))
        {
            Directory.CreateDirectory(LocalDll);
        }
        if (!Directory.Exists(LocalClass))
        {
            Directory.CreateDirectory(LocalClass);
        }
        if (!Directory.Exists(LocalSocket))
        {
            Directory.CreateDirectory(LocalSocket);
        }
        if (!Directory.Exists(LocalWebSocket))
        {
            Directory.CreateDirectory(LocalWebSocket);
        }
        if (!Directory.Exists(LocalRobot))
        {
            Directory.CreateDirectory(LocalRobot);
        }
        if (!Directory.Exists(LocalMqtt))
        {
            Directory.CreateDirectory(LocalMqtt);
        }
        if (!Directory.Exists(LocalTask))
        {
            Directory.CreateDirectory(LocalTask);
        }
        if (!Directory.Exists(LocalMqtt))
        {
            Directory.CreateDirectory(LocalMqtt);
        }

        var dirs = Function.GetPathFileName(LocalClass);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadClass.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(LocalDll);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadDll.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(LocalSocket);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadSocket.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(LocalWebSocket);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadWebSocket.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(LocalRobot);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadRobot.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(LocalMqtt);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadMqtt.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Function.GetPathFileName(LocalTask);
        foreach (var item in dirs)
        {
            try
            {
                if (item.FullName.Contains(".pdb"))
                    continue;
                LoadTask.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
    }
}
