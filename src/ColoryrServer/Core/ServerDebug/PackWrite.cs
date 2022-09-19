using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.AspNetCore.Mvc.Formatters;
using ColoryrWork.Lib.Debug.Object;
using ColoryrWork.Lib.ServerDebug;

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
        buffer.WriteByte(2);
        buffer.WriteBytes(obj.ToPack());
        DebugNetty.Send(context, buffer);
    }
}
