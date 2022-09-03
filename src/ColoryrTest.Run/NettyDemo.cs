namespace ColoryrTest.Run;

using ColoryrServer.SDK;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Security.Cryptography.X509Certificates;

[DllIN]
public class NettyDemoCS : INetty
{
    private IChannel bootstrapChannel;
    public override void Start(MultithreadEventLoopGroup bossGroup, MultithreadEventLoopGroup workerGroup)
    {

    }
    public void Start(MultithreadEventLoopGroup bossGroup, MultithreadEventLoopGroup workerGroup, int port)
    {
        X509Certificate2 tlsCertificate = null;
        //tlsCertificate = new X509Certificate2(Path.Combine(local, "dotnetty.com.pfx"), "password");
        var bootstrap = new ServerBootstrap();
        bootstrap
            .Group(bossGroup, workerGroup)
            .Channel<TcpServerSocketChannel>()
            .Option(ChannelOption.SoBacklog, 100)
            .Handler(new LoggingHandler("LSTN"))
            .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                if (tlsCertificate != null)
                {
                    pipeline.AddLast(TlsHandler.Server(tlsCertificate));
                }
                pipeline.AddLast(new LoggingHandler("CONN"));
                pipeline.AddLast(new DiscardServerHandler());
            }));

        bootstrapChannel = bootstrap.BindAsync(port).Result;
    }

    public override void Stop()
    {
        bootstrapChannel.CloseAsync().Wait();
    }
}

class DiscardServerHandler : SimpleChannelInboundHandler<object>
{
    protected override void ChannelRead0(IChannelHandlerContext context, object message)
    {
    }

    public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
    {
        Console.WriteLine("{0}", e.ToString());
        ctx.CloseAsync();
    }
}