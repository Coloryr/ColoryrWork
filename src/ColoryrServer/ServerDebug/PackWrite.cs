using ColoryrWork.Lib.Debug.Object;
using ColoryrWork.Lib.ServerDebug;
using DotNetty.Buffers;

namespace ColoryrServer.ServerDebug;

internal static class PackWrite
{
    public static void SendRegister(RegisterObj obj)
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteByte(1)
            .WriteBytes(obj.ToPack());
        DebugNetty.Send(buffer);
    }

    public static void SendHttpRes(HttpResObj obj)
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteByte(2)
            .WriteBytes(obj.ToPack());
        DebugNetty.Send(buffer);
    }

    public static void SendDatabase(DatabaseObj obj)
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteByte(3)
            .WriteBytes(obj.ToPack());
        DebugNetty.Send(buffer);
    }
}
