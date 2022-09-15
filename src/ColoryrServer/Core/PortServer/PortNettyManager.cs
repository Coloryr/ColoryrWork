using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.SDK;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColoryrServer.Core.PortServer;

internal static class PortNettyManager
{
    private static MultithreadEventLoopGroup BossGroup;
    private static MultithreadEventLoopGroup WorkerGroup;
    private readonly static Dictionary<string, INetty> RunNetty = new();
    public static void Start()
    {
        BossGroup = new();
        WorkerGroup = new();
        ServerMain.OnStop += Stop;
    }

    private static void Stop()
    {
        foreach (var item in RunNetty.Values)
        {
            item.Stop();
        }
        RunNetty.Clear();
        Task.WaitAll(BossGroup.ShutdownGracefullyAsync(), WorkerGroup.ShutdownGracefullyAsync());
    }

    public static void AddItem(SocketDllAssembly dll)
    {
        StopItem(dll.Name);
        if (Check(dll.SelfType))
        {
            var obj1 = Activator.CreateInstance(dll.SelfType) as INetty;
            try
            {
                obj1.Start(BossGroup, WorkerGroup);
                RunNetty.Add(dll.Name, obj1);
                ServerMain.LogOut($"Netty[{dll.Name}]启动");
            }
            catch (Exception e)
            {
                string error = e.ToString();
                Task.Run(() => DllRun.ServiceOnError(e));
                DllRunLog.PutError($"[Socket]{dll.Name}", error);
                ServerMain.LogError($"Netty[{dll.Name}]启动错误", e);
            }
        }
    }

    public static void StopItem(string name)
    {
        if (RunNetty.Remove(name, out var item))
        {
            item.Stop();
            ServerMain.LogOut($"Netty[{name}]关闭");
        }
    }

    public static bool Check(Type type)
    {
        bool find = false;
        do
        {
            if (type == typeof(INetty))
            {
                find = true;
                break;
            }
            type = type.BaseType;
        } while (type != null);
        return find;
    }
}
