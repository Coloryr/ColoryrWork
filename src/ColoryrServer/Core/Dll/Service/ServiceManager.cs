using ColoryrServer.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ColoryrServer.Core.Dll.Service;

internal static class ServiceManager
{
    private static readonly ConcurrentDictionary<string, IService> Services = new();

    public static void Remove(string uuid)
    {
        if (Services.TryRemove(uuid, out var item))
        {
            item.Close();
        }
    }

    public static void Load(ServiceDllAssembly item)
    {
        if (item.ServiceType == ServiceType.Normal)
        {
            ServerMain.LogOut($"Service[{item.Name}]正在初始化");
            var service = new ServiceWithThread(item.Name, null);
            if (Services.TryAdd(item.Name, service))
            {
                if (item.AutoStart)
                {
                    service.OnStart();
                }
            }
            else
            {
                service.Close();
            }
        }
        else if (item.ServiceType == ServiceType.OnlyOpen)
        {
            ServerMain.LogOut($"Service[{item.Name}]正在初始化");
            var service = new ServiceWithServer(item.Name);
            if (Services.TryAdd(item.Name, service))
            {
                if (item.AutoStart)
                {
                    service.OnStart();
                }
            }
            else
            {
                service.Close();
            }
        }
    }

    private static void Stop()
    {
        foreach (var item in Services.Values)
        {
            item.Close();
        }
    }

    public static void Start()
    {
        ServerMain.OnStop += Stop;

        foreach (var item in AssemblyList.GetService())
        {
            Load(item);
        }
    }

    public static bool Have(string name)
    {
        return Services.ContainsKey(name);
    }
    public static void Start(string name)
    {
        if (Services.ContainsKey(name))
        {
            var item = Services[name];
            item.OnStart();
        }
    }
    public static void Stop(string name)
    {
        if (Services.ContainsKey(name))
        {
            Services[name].OnStop();
        }
    }
    public static void Pause(string name)
    {
        if (Services.ContainsKey(name))
        {
            Services[name].Pause();
        }
    }
    public static ServiceState GetState(string name)
    {
        if (Services.ContainsKey(name))
        {
            return Services[name].State;
        }
        return ServiceState.Error;
    }

    public static void SetArg(string name, object[] arg)
    {
        if (Services.ContainsKey(name))
        {
            Services[name].SetArg(arg);
        }
    }

    public static void Reload(string name)
    {
        if (Services.ContainsKey(name))
        {
            var item = Services[name];
            item.Close();
        }
    }

    internal static Exception GetError(string name)
    {
        if (Services.ContainsKey(name))
        {
            var item = Services[name];
            return item.LastError;
        }

        return null;
    }
}
