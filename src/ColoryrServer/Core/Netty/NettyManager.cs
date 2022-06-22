using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.SDK;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColoryrServer.Core.Netty;

internal static class NettyManager
{
    private readonly static MultithreadEventLoopGroup BossGroup = new();
    private readonly static MultithreadEventLoopGroup WorkerGroup = new();
    private readonly static Dictionary<string, INetty> RunNetty = new();
    public static void Start() 
    {
        //foreach (var item in DllStongeManager.GetSocket())
        //{
        //    AddItem(item);
        //}
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

    public static void AddItem(DllAssembly dll) 
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
                DllRunError.PutError($"[Socket]{dll.Name}", error);
                ServerMain.LogOut($"Netty[{dll.Name}]启动错误:{e}");
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
