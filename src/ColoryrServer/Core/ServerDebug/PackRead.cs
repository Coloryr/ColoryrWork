using ColoryrServer.Core.Http;
using ColoryrWork.Lib.ServerDebug;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.ServerDebug;

internal static class PackRead
{
    public static void Pack(IChannelHandlerContext context, IByteBuffer buffer)
    {
        byte type = buffer.ReadByte();
        switch (type)
        {
            case 1:
                {
                    var obj1 = buffer.ReadRegisterPack();
                    var res = HttpInvokeRoute.CheckBase(obj1.url);
                    if (!res)
                    {
                        HttpInvokeRoute.AddDebug(obj1.url, context);
                    }
                    context.SendRegister(res);
                }
                break;
            case 2:
                {
                    var obj = buffer.ReadHttpResPack();

                }
                break;
        }
    }
}
