using ColoryrServer.SDK;
using System.Text;

namespace ColoryrServer.Utils
{
    class StreamUtils
    {
        public static byte[] JsonOBJ(object obj, EncodeType encodeType = EncodeType.UTF8)
        {
            var data = Tools.ToJson(obj);
            return encodeType switch
            {
                EncodeType.Default => Encoding.Default.GetBytes(data),
                EncodeType.ASCII => Encoding.ASCII.GetBytes(data),
                _ => Encoding.UTF8.GetBytes(data),
            };
        }

        public static byte[] StringOBJ(string data, EncodeType encodeType = EncodeType.UTF8)
        {
            return encodeType switch
            {
                EncodeType.Default => Encoding.Default.GetBytes(data),
                EncodeType.ASCII => Encoding.ASCII.GetBytes(data),
                _ => Encoding.UTF8.GetBytes(data),
            };
        }
    }
}
