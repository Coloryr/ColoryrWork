using ColoryrServer.SDK;
using DotNetty.Transport.Channels;
using System.Diagnostics;

namespace ColoryrTest.Run;

internal class Program
{
    private static bool Check(Type type)
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
    static void Main(string[] args)
    {
        var bossGroup = new MultithreadEventLoopGroup();
        var workerGroup = new MultithreadEventLoopGroup();
        var p1 = new NettyDemoCS();
        var p2 = new NettyDemoCS();
        Stopwatch watch = new();
        watch.Start();
        Type type = p1.GetType();
        bool res = Check(type);
        watch.Stop();
        Console.WriteLine(res);
        Console.WriteLine(watch.Elapsed);
        //p1.Start(bossGroup, workerGroup, 12345);
        //p2.Start(bossGroup, workerGroup, 24567);
        //Console.Read();
        //p1.Stop();
        //p2.Stop();
        //Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
    }
}