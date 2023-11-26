using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.Cluster;

internal class PipeClientHandler : ChannelHandlerAdapter
{
    private Bootstrap _bootstrap;
    private int times = 0;
    private int delay = 1;
    private string _ip;
    private ushort _port;
    private bool stop;
    public override bool IsSharable => true;

    public PipeClientHandler(Bootstrap bootstrap, string ip, ushort port)
    {
        _bootstrap = bootstrap;
        _ip = ip;
        _port = port;
    }

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        if (message is IByteBuffer byteBuffer)
        {

        }
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        ServerMain.LogError("集群模式错误", exception);
        context.CloseAsync();
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
        times = 0;
        delay = 1;
    }

    public override void ChannelUnregistered(IChannelHandlerContext ctx)
    {
        times++;
        if (times == 10)
        {
            delay = 30;
        }
        else if (times == 20)
        {
            delay = 60;
        }
        else if(times == 30)
        ServerMain.LogWarn($"集群链接中断，{delay}秒后自动重连");
        Task.Run(() =>
        {
            Thread.Sleep(1000);
            ServerMain.LogWarn($"集群自动重连次数:{times}");
            Start();
        });
    }

    public void Stop()
    { 
        
    }

    public void Start()
    {
        _bootstrap.ConnectAsync(_ip, _port);
    }
}