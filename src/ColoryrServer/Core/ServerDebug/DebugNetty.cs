using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.ServerDebug;

internal static class DebugNetty
{
    private static MultithreadEventLoopGroup bossGroup;
    private static MultithreadEventLoopGroup workerGroup;
    private static IChannel bootstrapChannel;
    private static Aes Aes;
    private static ICryptoTransform encode;
    private static ICryptoTransform decode;
    public async static void Start()
    {
        ServerMain.LogOut("服务器调试启动中");
        ServerMain.OnStop += Stop;
        try
        {
            Aes = Aes.Create();
            Aes.BlockSize = 128;
            Aes.KeySize = 256;
            Aes.FeedbackSize = 128;
            Aes.Padding = PaddingMode.PKCS7;

            Aes.Mode = CipherMode.CBC;
            byte[] key = Encoding.UTF8.GetBytes(ServerMain.Config.DebugPort.Key);
            byte[] iv = Encoding.UTF8.GetBytes(ServerMain.Config.DebugPort.IV);
            var size = Aes.KeySize / 8;
            if (key.Length != size)
            {
                Array.Resize(ref key, size);
            }
            size = Aes.BlockSize / 8;
            if (iv.Length != size)
            {
                Array.Resize(ref iv, size);
            }

            Aes.Key = key;
            Aes.IV = iv;

            encode = Aes.CreateEncryptor();
            decode = Aes.CreateDecryptor();

            bossGroup = new MultithreadEventLoopGroup(1);
            workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Handler(new LoggingHandler("ServerDebug"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new LoggingHandler("ServerDebug"));
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(1024 * 500, 0, 4, 0, 4));
                    pipeline.AddLast(new DebugServerHandler());
                }));

            bootstrapChannel = await bootstrap.BindAsync(ServerMain.Config.DebugPort.Port);

            ServerMain.LogOut($"服务器调试启动在:{ServerMain.Config.DebugPort.Port}");
        }
        catch (Exception e)
        {
            ServerMain.LogError("服务器调试启动错误", e);
        }
    }

    private async static void Stop()
    {
        ServerMain.LogOut("服务器调试关闭中");
        await bootstrapChannel?.CloseAsync();
        await bootstrapChannel?.DisconnectAsync();
        Task.WaitAll(
            bossGroup.ShutdownGracefullyAsync(
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)),
            workerGroup.ShutdownGracefullyAsync(
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)));
        ServerMain.LogOut("服务器调试已关闭");
    }

    internal static byte[] Decode(byte[] input)
    {
        return decode.TransformFinalBlock(input, 0, input.Length);
    }

    private static byte[] Encode(byte[] input)
    {
        return encode.TransformFinalBlock(input, 0, input.Length);
    }

    public static async void Send(IChannelHandlerContext context, IByteBuffer buffer)
    {
        byte[] data = new byte[buffer.ReadableBytes];
        buffer.ReadBytes(data);
        data = Encode(data);
        await context.WriteAndFlushAsync(Unpooled.WrappedBuffer(data));
    }
}

public class DebugServerHandler : ChannelHandlerAdapter
{

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        var buffer = message as IByteBuffer;
        if (buffer != null)
        {
            var data = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(data);
            data = DebugNetty.Decode(data);
            PackRead.Pack(context, Unpooled.WrappedBuffer(data));
        }
    }

    public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        Console.WriteLine("Exception: " + exception);
        context.CloseAsync();
    }
}
