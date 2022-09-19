using ColoryrWork.Lib.Debug.Object;
using DotNetty.Buffers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
                data = buffer.ReadBytes1()
            },
            id = buffer.ReadLong()
        };
    }

    public static HttpObj ReadHttpPack(this IByteBuffer buffer)
    {
        IFormatter formatter = new BinaryFormatter();
        byte[] byt = new byte[buffer.Capacity];
        buffer.ReadBytes(byt);
        Stream stream = new MemoryStream(byt, 0, byt.Length);
        return formatter.Deserialize(stream) as HttpObj;
    }
}
