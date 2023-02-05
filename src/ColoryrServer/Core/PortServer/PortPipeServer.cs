using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.PortServer;

internal static class PortPipeServer
{
    private readonly static MultithreadEventLoopGroup BossGroup = new(1);
    private readonly static MultithreadEventLoopGroup WorkerGroup = new();
    private static Bootstrap Bootstrap;
    private static ServerBootstrap ServerBootstrap;
    private static IChannel BoundChannel;

    public static void Start()
    {
        ServerMain.OnStop += Stop;

        if (!ServerMain.Config.Pipe.Enable)
            return;

        if (ServerMain.Config.Pipe.Server)
        {
            StartServer();
        }
        else
        {
            StartClient();
        }
    }

    private static async void StartClient()
    {
        try
        {
            ServerMain.LogWarn("Pipe启动客户端");
            Bootstrap = new Bootstrap();
            Bootstrap
                .Group(WorkerGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(4));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 4, 0, 4));

                    pipeline.AddLast("echo", new PipeClientHandler());
                }));
            await ClientConnect();
        }
        catch (Exception e)
        {

        }
    }

    private static async void StartServer()
    {
        ServerBootstrap = new ServerBootstrap();
        ServerBootstrap.Group(BossGroup, WorkerGroup)
            .Channel<TcpServerSocketChannel>()
            .Option(ChannelOption.SoBacklog, 100)
            .Handler(new LoggingHandler("SRV-LSTN"))
            .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 4, 0, 4));

                pipeline.AddLast("echo", new PipeServerHandler());
            }));

        BoundChannel = await ServerBootstrap.BindAsync(ServerMain.Config.Pipe.Port);
    }

    public static Task ClientConnect()
    {
        return Bootstrap.ConnectAsync(ServerMain.Config.Pipe.IP, ServerMain.Config.Pipe.Port);
    }

    private static async void Stop()
    {
        if (ServerMain.Config.Pipe.Server)
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

public class PipeClientHandler : ChannelHandlerAdapter
{
    private int times = 0;
    public override bool IsSharable => true;

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        var byteBuffer = message as IByteBuffer;
        if (byteBuffer != null)
        {

        }
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        ServerMain.LogError("Pipe错误", exception);
        context.CloseAsync();
    }

    public override void ChannelUnregistered(IChannelHandlerContext ctx)
    {
        ServerMain.LogWarn("Pipe链接中断，1秒后自动重连");
        times++;
        Task.Run(async () =>
        {
            Thread.Sleep(1000);
            ServerMain.LogWarn($"Pipe自动重连次数:{times}");
            await PortPipeServer.ClientConnect();
        });
    }
}

public class PipeServerHandler : ChannelHandlerAdapter
{
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        var buffer = message as IByteBuffer;
        if (buffer != null)
        {

        }
        context.WriteAsync(message);
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        ServerMain.LogError("Pipe错误", exception);
        context.CloseAsync();
    }
}
