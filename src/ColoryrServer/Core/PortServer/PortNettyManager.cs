using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.FileSystem.Database;
using ColoryrServer.SDK;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColoryrServer.Core.PortServer;

internal static class PortNettyManager
{
    private static MultithreadEventLoopGroup BossGroup = new();
    private static MultithreadEventLoopGroup WorkerGroup = new();
    private readonly static Dictionary<string, INetty> RunNetty = new();

    public static void Start()
    {
        ServerMain.OnStop += Stop;
    }

    private static void Stop()
    {
        foreach (var item in RunNetty.Values)
        {
            item.Stop();
        }
        RunNetty.Clear();
        Task.WaitAll(
            BossGroup.ShutdownGracefullyAsync(
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
            WorkerGroup.ShutdownGracefullyAsync(
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
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
                LogDatabsae.PutError($"[Socket]{dll.Name}", error);
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
