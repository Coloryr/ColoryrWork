using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Codecs;

namespace ColoryrServer.Core.Cluster;

internal static class ClusterPipe
{
    private readonly static MultithreadEventLoopGroup BossGroup = new(1);
    private readonly static MultithreadEventLoopGroup WorkerGroup = new();
    private readonly static List<PipeClientHandler> ClientPipe = new();
    private readonly static List<PipeServerHandler> ServertPipe = new();
    private readonly static ServerBootstrap ServerBootstrap;
    private static IChannel BoundChannel;

    public static void Start()
    {
        if (!ServerMain.Config.Pipe.Enable)
        {
            return;
        }

        ServerMain.OnStop += Stop;

        if (ServerMain.Config.Pipe.Mode == 0)
        {
            StartServer();
        }
        else if (ServerMain.Config.Pipe.Mode == 1)
        {
            foreach (var item in ServerMain.Config.Pipe.Node)
            {
                StartClient(item.Key, item.Value);
            }
        }
    }

    private static void StartClient(string ip, ushort port)
    {
        try
        {
            ServerMain.LogWarn($"集群启动客户端 {ip}:{port}");
            var client = new Bootstrap();
            var pipe = new PipeClientHandler(client, ip, port);
            client
                .Group(WorkerGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));

                    pipeline.AddLast("pipe", pipe);
                }));
            pipe.Start();
        }
        catch (Exception e)
        {
            ServerMain.LogError("集群客户端启动失败", e);
        }
    }

    private static async void StartServer()
    {
        try
        {
            ServerMain.LogWarn("集群启动服务器");
            //ServerBootstrap = new ServerBootstrap();
            //ServerBootstrap.Group(BossGroup, WorkerGroup)
            //    .Channel<TcpServerSocketChannel>()
            //    .Option(ChannelOption.SoBacklog, 100)
            //    .Handler(new LoggingHandler("SRV-LSTN"))
            //    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            //    {
            //        IChannelPipeline pipeline = channel.Pipeline;
            //        pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
            //        pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));

            //        pipeline.AddLast("pipe", new PipeServerHandler());
            //    }));

            //BoundChannel = await ServerBootstrap.BindAsync(ServerMain.Config.Pipe.Port);
        }
        catch (Exception e)
        {
            ServerMain.LogError("集群服务器启动失败", e);
        }
    }

    private static async void Stop()
    {
        if (BoundChannel is { })
        {
            await BoundChannel.CloseAsync();
        }
        Task.WaitAll(
            BossGroup.ShutdownGracefullyAsync(
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
            WorkerGroup.ShutdownGracefullyAsync(
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
    }
}
