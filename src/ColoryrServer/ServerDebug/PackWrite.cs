using ColoryrWork.Lib.Debug.Object;
using ColoryrWork.Lib.ServerDebug;
using DotNetty.Buffers;
using EcmaScript.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
