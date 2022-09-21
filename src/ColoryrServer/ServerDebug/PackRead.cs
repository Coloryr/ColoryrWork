using ColoryrWork.Lib.ServerDebug;
using DotNetty.Buffers;

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
                    Task.Run(() =>
                    {
                        var res = DebugIn.Invoke(obj);
                        if (res != null)
                        {
                            PackWrite.SendHttpRes(res);
                        }
                    });
                }
                break;
            case 3:
                {
                    var obj = buffer.ReadDatabaseResPack();
                    
                }
                break;
        }
    }
}
