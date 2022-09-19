using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ColoryrWork.Lib.ServerDebug;

namespace ColoryrServer.ServerDebug;

internal static class PackRead
{
    public static void Pack(IByteBuffer buffer)
    {
        byte type = buffer.ReadByte();
        switch (type)
        {
            case 1:
                {
                    bool res = buffer.ReadBoolean();
                    if (!res)
                    {
                        throw new Exception("路由添加失败");
                    }
                }
                break;
            case 2:
                {
                    var obj = buffer.ReadHttpPack();
                    DebugIn.Invoke(obj);
                }
                break;
        }
    }
}
