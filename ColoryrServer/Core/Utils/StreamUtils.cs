using ColoryrServer.SDK;
using System.Text;

namespace ColoryrServer.Utils;

public class StreamUtils
{
    public static byte[] JsonOBJ(object obj)
    {
        var data = Tools.ToJson(obj);
        return Encoding.UTF8.GetBytes(data);
    }

    public static byte[] StringOBJ(string data)
    {
        return Encoding.UTF8.GetBytes(data);
    }
}
