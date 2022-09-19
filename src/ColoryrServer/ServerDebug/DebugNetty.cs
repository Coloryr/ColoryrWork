using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.ServerDebug;

internal static class DebugNetty
{
    private static MultithreadEventLoopGroup Group = new();
    private static Bootstrap Bootstrap = new();
    private static IChannel ClientChannel;
    private static EchoClientHandler handler;
    public async static void Start(string ip, int port, string key, string iv)
    {
        Bootstrap
            .Group(Group)
            .Channel<TcpSocketChannel>()
            .Option(ChannelOption.TcpNodelay, true)
            .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                pipeline.AddLast(new LoggingHandler());
                pipeline.AddLast(new LengthFieldPrepender(4));
                pipeline.AddLast(new LengthFieldBasedFrameDecoder(1024 * 500, 0, 4, 0, 4));

                pipeline.AddLast(handler = new EchoClientHandler(key, iv));
            }));

        ClientChannel = await Bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
    }

    public async static void Stop() 
    {
        await ClientChannel.CloseAsync();
        await Group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
    }

    public static void Send(IByteBuffer buffer)
    {
        handler.Send(buffer);
    }

    public class EchoClientHandler : ChannelHandlerAdapter
    {
        private Aes Aes;
        private ICryptoTransform encode;
        private ICryptoTransform decode;

        public EchoClientHandler(string ikey, string iiv)
        {
            Aes = Aes.Create();
            Aes.BlockSize = 128;
            Aes.KeySize = 256;
            Aes.FeedbackSize = 128;
            Aes.Padding = PaddingMode.PKCS7;

            Aes.Mode = CipherMode.CBC;
            byte[] key = Encoding.UTF8.GetBytes(ikey);
            byte[] iv = Encoding.UTF8.GetBytes(iiv);
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
        }

        private byte[] Decode(byte[] input)
        {
            return decode.TransformFinalBlock(input, 0, input.Length);
        }

        private byte[] Encode(byte[] input)
        {
            return encode.TransformFinalBlock(input, 0, input.Length);
        }

        public async void Send(IByteBuffer buffer)
        {
            byte[] data = new byte[buffer.Capacity];
            var data1 = Encode(data);
            var buffer1 = Unpooled.Buffer();
            buffer.WriteBytes(data1);
            await ClientChannel.WriteAndFlushAsync(buffer1);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var byteBuffer = message as IByteBuffer;
            if (byteBuffer != null)
            {
                var data = byteBuffer.Array;
                data = Decode(data);
                var buffer1 = Unpooled.WrappedBuffer(data);
                PackRead.Pack(buffer1);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}
