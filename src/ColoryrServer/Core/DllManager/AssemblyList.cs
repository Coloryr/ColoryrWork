using ColoryrServer.Core.DllManager.Service;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.Core.Http;
using ColoryrServer.Core.PortServer;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager;

internal static class AssemblyList
{
    private static readonly ConcurrentDictionary<string, DllAssembly> MapDll = new();
    private static readonly ConcurrentDictionary<string, DllAssembly> MapClass = new();
    private static readonly ConcurrentDictionary<string, SocketDllAssembly> MapSocket = new();
    private static readonly ConcurrentDictionary<string, DllAssembly> MapWebSocket = new();
    private static readonly ConcurrentDictionary<string, RobotDllAssembly> MapRobot = new();
    private static readonly ConcurrentDictionary<string, DllAssembly> MapMqtt = new();
    private static readonly ConcurrentDictionary<string, ServiceDllAssembly> MapService = new();

    private static void Stop()
    {
        foreach (var item in MapDll.Values)
        {
            item.Unload();
        }
        foreach (var item in MapClass.Values)
        {
            item.Unload();
        }
        foreach (var item in MapSocket.Values)
        {
            item.Unload();
        }
        foreach (var item in MapWebSocket.Values)
        {
            item.Unload();
        }
        foreach (var item in MapRobot.Values)
        {
            item.Unload();
        }
        foreach (var item in MapMqtt.Values)
        {
            item.Unload();
        }
        foreach (var item in MapService.Values)
        {
            item.Unload();
        }
        MapDll.Clear();
        MapClass.Clear();
        MapSocket.Clear();
        MapWebSocket.Clear();
        MapRobot.Clear();
        MapMqtt.Clear();
        MapService.Clear();
    }

    public static void Start()
    {
        ServerMain.OnStop += Stop;
    }

    public static DllAssembly FindClass(AssemblyName name)
    {
        if (MapClass.TryGetValue(name.Name, out var item))
        {
            return item;
        }
        return null;
    }

    public static void AddDll(string uuid, DllAssembly save)
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

        HttpInvokeRoute.AddDll(uuid, save);
    }
    public static void RemoveDll(string uuid)
    {
        if (MapDll.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        FileDllManager.RemoveDll(uuid);
        HttpInvokeRoute.Remove(uuid);
    }
    public static DllAssembly GetDll(string uuid)
    {
        if (MapDll.TryGetValue(uuid, out var save))
        {
            return save;
        }
        else
            return null;
    }

    public static void AddClass(string uuid, DllAssembly save)
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
        FileDllManager.RemoveClass(uuid);
    }
    public static DllAssembly GetClass(string uuid)
    {
        if (MapClass.TryGetValue(uuid, out var save))
        {
            return save;
        }
        else
            return null;
    }

    public static void AddSocket(string uuid, SocketDllAssembly save)
    {
        PortNettyManager.StopItem(uuid);
        if (MapSocket.ContainsKey(uuid))
        {
            var old = MapSocket[uuid];
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

        if (save.Netty)
            PortNettyManager.AddItem(save);
    }
    public static void RemoveSocket(string uuid)
    {
        if (MapSocket.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        FileDllManager.RemoveSocket(uuid);
    }
    public static List<SocketDllAssembly> GetSocket()
    {
        return new List<SocketDllAssembly>(MapSocket.Values);
    }

    public static void AddWebSocket(string uuid, DllAssembly save)
    {
        if (MapWebSocket.ContainsKey(uuid))
        {
            var old = MapWebSocket[uuid];
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
        FileDllManager.RemoveWebSocket(uuid);
    }
    public static List<DllAssembly> GetWebSocket()
    {
        return new List<DllAssembly>(MapWebSocket.Values);
    }

    public static void AddRobot(string uuid, RobotDllAssembly save)
    {
        if (MapRobot.ContainsKey(uuid))
        {
            var old = MapRobot[uuid];
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
        FileDllManager.RemoveRobot(uuid);
    }
    public static List<RobotDllAssembly> GetRobot()
    {
        return new List<RobotDllAssembly>(MapRobot.Values);
    }

    public static void AddMqtt(string uuid, DllAssembly save)
    {
        if (MapMqtt.ContainsKey(uuid))
        {
            var old = MapMqtt[uuid];
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
        FileDllManager.RemoveMqtt(uuid);
    }
    public static List<DllAssembly> GetMqtt()
    {
        return new List<DllAssembly>(MapMqtt.Values);
    }

    public static void AddService(string uuid, ServiceDllAssembly save)
    {
        if (MapService.ContainsKey(uuid))
        {
            var old = MapService[uuid];
            MapService[uuid] = save;
            Task.Run(() =>
            {
                old.Unload();
                old.SelfType = null;
                old.MethodInfos.Clear();
                DllUseSave.Update(save);
                ServiceManager.Load(save);
            });
        }
        else
        {
            MapService.TryAdd(uuid, save);
        }
    }
    public static void RemoveService(string uuid)
    {
        if (MapService.TryRemove(uuid, out var item))
        {
            item.Unload();
            item.SelfType = null;
            item.MethodInfos.Clear();
        }
        FileDllManager.RemoveService(uuid);
    }
    public static ServiceDllAssembly GetService(string uuid)
    {
        return MapService.TryGetValue(uuid, out var dll) ? dll : null;
    }
    public static List<ServiceDllAssembly> GetService()
    {
        return new List<ServiceDllAssembly>(MapService.Values);
    }
}
