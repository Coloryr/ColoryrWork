using ColoryrWork.Lib.Debug.Object;
using DotNetty.Buffers;
using MsgPack.Serialization;
using System.Collections.Generic;
using System.Text;

namespace ColoryrWork.Lib.ServerDebug;

public static class DebugDecode
{
    public static string ReadString(this IByteBuffer buff)
    {
        return buff.ReadString(buff.ReadInt(), Encoding.UTF8);
    }

    public static Dictionary<string, string> ReadDictionary(this IByteBuffer buff)
    {
        Dictionary<string, string> dic = new();
        for (int i = 0; i < buff.ReadInt(); i++)
        {
            dic.Add(buff.ReadString(), buff.ReadString());
        }
        return dic;
    }

    public static byte[] ReadBytes1(this IByteBuffer buff)
    {
        int size = buff.ReadInt();
        byte[] temp = new byte[size];
        buff.ReadBytes(temp);
        return temp;
    }

    public static RegisterObj ReadRegisterPack(this IByteBuffer buffer)
    {
        return new()
        {
            url = buffer.ReadString()
        };
    }

    public static HttpResObj ReadHttpResPack(this IByteBuffer buffer)
    {
        return new()
        {
            resopneObj = new()
            {
                ReCode = buffer.ReadInt(),
                Cookie = buffer.ReadDictionary(),
                Head = buffer.ReadDictionary(),
                ContentType = buffer.ReadString(),
                Data = buffer.ReadBytes1()
            },
            id = buffer.ReadLong()
        };
    }

    public static HttpObj ReadHttpPack(this IByteBuffer buffer)
    {
        byte[] byt = new byte[buffer.ReadableBytes];
        buffer.ReadBytes(byt);
        var serializer = MessagePackSerializer.Get<HttpObj>();
        var obj = serializer.UnpackSingleObject(byt);
        return obj;
    }

    public static DatabaseObj ReadDatabasePack(this IByteBuffer buffer) 
    {
        byte[] byt = new byte[buffer.ReadableBytes];
        buffer.ReadBytes(byt);
        var serializer = MessagePackSerializer.Get<DatabaseObj>();
        var obj = serializer.UnpackSingleObject(byt);
        return obj;
    }

    public static DatabaseResObj ReadDatabaseResPack(this IByteBuffer buffer)
    {
        byte[] byt = new byte[buffer.ReadableBytes];
        buffer.ReadBytes(byt);
        var serializer = MessagePackSerializer.Get<DatabaseResObj>();
        var obj = serializer.UnpackSingleObject(byt);
        return obj;
    }
}
