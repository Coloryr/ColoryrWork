using ColoryrWork.Lib.Debug.Object;
using ColoryrWork.Lib.ServerDebug;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace ColoryrServer.Core.ServerDebug;

internal static class PackWrite
{
    public static void SendRegister(this IChannelHandlerContext context, bool res)
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteByte(1)
            .WriteBoolean(res);
        DebugNetty.Send(context, buffer);
    }

    public static void SendHttp(this IChannelHandlerContext context, HttpObj obj)
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteByte(2)
            .WriteBytes(obj.ToPack());
        DebugNetty.Send(context, buffer);
    }

    public static void SendDatabaseRes(this IChannelHandlerContext context, DatabaseResObj obj) 
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteByte(3)
            .WriteBytes(obj.ToPack());
        DebugNetty.Send(context, buffer);
    }
}
