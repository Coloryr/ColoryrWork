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
using ColoryrSDK;
using System.Security.Cryptography;

namespace ColoryrWork.Lib.ServerDebug;

public static class DebugEncode
{
    public static IByteBuffer WriteDictionary(this IByteBuffer buff, Dictionary<string, string> dic)
    {
        buff.WriteInt(dic.Count);
        foreach (var item in dic)
        {
            buff.WriteString(item.Key).WriteString(item.Value);
        }
        return buff;
    }
    public static IByteBuffer WriteBytes1(this IByteBuffer buff, byte[] data)
    {
        buff.WriteInt(data.Length);
        buff.WriteBytes(data, 0, data.Length);
        return buff;
    }
    public static IByteBuffer WriteString(this IByteBuffer buff, string data)
    {
        var temp = Encoding.UTF8.GetBytes(data);
        buff.WriteBytes1(temp);
        return buff;
    }

    public static IByteBuffer ToPack(this RegisterObj obj)
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteString(obj.url);
        return buffer;
    }

    public static IByteBuffer ToPack(this HttpObj obj)
    {
        var buffer = Unpooled.Buffer();
        IFormatter formatter = new BinaryFormatter();
        using MemoryStream stream = new();
        formatter.Serialize(stream, obj);
        byte[] byt = new byte[stream.Length];
        byt = stream.ToArray();
        stream.Flush();
        buffer.WriteBytes(byt);
        return buffer;
    }

    public static IByteBuffer ToPack(this HttpResObj obj)
    {
        var buffer = Unpooled.Buffer();
        buffer.WriteInt(obj.resopneObj.ReCode)
            .WriteDictionary(obj.resopneObj.Cookie)
            .WriteDictionary(obj.resopneObj.Head)
            .WriteString(obj.resopneObj.ContentType)
            .WriteBytes1(obj.resopneObj.data)
            .WriteLong(obj.id);
        return buffer;
    }
}
