# ColoryrServer

Socket代码编写

[返回](code.md)

Socket代码有两种类型，传统Socket数据包处理和自定义Netty处理

传统Socket数据包处理默认代码
```C#
using ColoryrServer.SDK;

[DLLIN]
public class test
{
    public bool OnTcpMessage(TcpSocketRequest head)
    {
        return false; //true表示事件已处理完毕
    }
    public bool OnUdpMessage(UdpSocketRequest head)
    {
        return false;
    }
}
```

传统Socket数据包处理**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性 
- `OnTcpMessage`表示收到了TCP数据包
- `OnUdpMessage`表示收到了Udp数据包

返回如果为true，则这个事件不会传到下个Dll中去

可以通过查看[SocketSDK](../../src/ColoryrServer/Core/SDK/SocketSDK.cs)获取更多信息

Netty处理默认代码如下
```C#
using ColoryrServer.SDK;
using System;
using DotNetty.Transport.Channels;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels.Sockets;
using System.Security.Cryptography.X509Certificates;

[DLLIN]
public class test1 : INetty
{
    private IChannel bootstrapChannel;
    public override void Start(MultithreadEventLoopGroup bossGroup, MultithreadEventLoopGroup workerGroup)
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
```

Netty处理**必须**带有[ColoryrServer.SDK.DLLIN](../../src/ColoryrServer/Core/SDK/NotesSDK.cs#L21)的属性和继承[ColoryrServer.SDK.INetty](../../src/ColoryrServer/Core/SDK/ColoryrSDK.cs#L21)抽象类  

Netty处理在编译后和服务器启动后会自动启动Netty服务器，然后处理数据包  
编译时自动关闭Netty服务器，自动更换业务代码再启动Netty服务器
