using ColoryrServer.Core;
using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.Robot;
using ColoryrServer.Core.Utils;
using ColoryrServer.Core.WebSocket;
using ColoryrServer.Utils;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ColoryrServer.SDK;

public partial class HttpMultipartFile
{
    public Stream Data { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public string ContentDisposition { get; set; }
}
public partial class ServerContentType
{
    public const string OSTREAM = "application/octet-stream";
    public const string POSTXFORM = "application/x-www-form-urlencoded";
    public const string JPG = "application/x-jpg";
    public const string POSTFORMDATA = "multipart/form-data";
    public const string JSON = "application/json";
    public const string XML = "application/xml";
    public const string HTML = "text/html";
    public const string JPEG = "image/jpeg";
    public const string PNG = "application/x-png";
    public const string EXCEL = "application/vnd.ms-excel";
    public const string WORD = "application/msword";
    public const string PPT = "application/x-ppt";
    public const string MP3 = "audio/mp3";
    public const string MP4 = "video/mpeg4";
    public const string GIF = "image/gif";
    public const string ICO = "application/x-ico";

    public static string GetType(string type)
    {
        switch (type.ToLower())
        {
            case ".jpg":
                return JPG;
            case ".json":
                return JSON;
            case ".xml":
                return XML;
            case ".png":
                return PNG;
            case ".mp3":
                return MP3;
            case ".mp4":
                return MP4;
            default:
                return HTML;
        }
    }
}
public enum MyContentType
{
    Json, XFormData, MFormData, Other
}
public enum EncodeType
{
    UTF8, Default, ASCII
}
public partial class EnCode
{
    /// <summary>
    /// Base64编码
    /// </summary>
    /// <param name="data">源数据</param>
    /// <returns>编码后数据</returns>
    public static string BASE64(string data)
    {
        byte[] temp = Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(temp);
    }
    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>加密后的字符串</returns>
    public static string MD5(string data)
        => BitConverter.ToString(MD5_R(data)).ToLower().Replace("-", "");
    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="data">加密</param>
    /// <returns>加密后的byte</returns>
    public static byte[] MD5_R(string data)
    {
        using MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] buffer = Encoding.Default.GetBytes(data);
        return md5.ComputeHash(buffer);
    }
    /// <summary>
    /// OPENSSL加密
    /// </summary>
    /// <param name="publicKey">公钥</param>
    /// <param name="data">数据</param>
    /// <param name="fOAEP"></param>
    /// <returns>加密后的数据</returns>
    public static string OpenSSL(string publicKey, string data, bool fOAEP = false)
    {
        using var _publicKeyRsaProvider = Openssl.CreateRsaProviderFromPublicKey(publicKey);
        return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(data), fOAEP));
    }
    /// <summary>
    /// SHA1加密
    /// </summary>
    /// <param name="data">原数据</param>
    /// <returns>加密后的数据</returns>
    public static string SHA1(string data)
    {
        using var sha1 = System.Security.Cryptography.SHA1.Create();
        byte[] str01 = Encoding.Default.GetBytes(data);
        byte[] str02 = sha1.ComputeHash(str01);
        return BitConverter.ToString(str02).Replace("-", "");
    }
    /// <summary>
    /// AES-128-CBC加密
    /// </summary>
    /// <param name="data">原始数据</param>
    /// <param name="key">密匙</param>
    /// <param name="iv">盐</param>
    /// <param name="mode">填充模式</param>
    /// <returns>加密后的数据</returns>
    public static byte[] AES128(byte[] data, string key, string iv)
    {
        return AES128(data, key, iv, PaddingMode.PKCS7);
    }
    public static byte[] AES128(byte[] data, string key, string iv, PaddingMode mode)
    {
        return AES128(data, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv), mode);
    }
    public static byte[] AES128(byte[] data, byte[] key, byte[] iv)
    {
        return AES128(data, key, iv, PaddingMode.PKCS7);
    }
    public static byte[] AES128(byte[] data, byte[] key, byte[] iv, PaddingMode mode)
    {
        using var rDel = Aes.Create();
        var size = rDel.KeySize / 8;
        if (key.Length != size)
        {
            Array.Resize(ref key, size);
        }
        size = rDel.BlockSize / 8;
        if (iv.Length != size)
        {
            Array.Resize(ref iv, size);
        }

        rDel.Key = key;
        rDel.IV = iv;
        rDel.Mode = CipherMode.CBC;
        rDel.Padding = mode;

        using var cTransform = rDel.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);

        return resultArray;
    }


    /// <summary>
    /// AES-256-CBC加密
    /// </summary>
    /// <param name="data">原始数据</param>
    /// <param name="key">密匙</param>
    /// <param name="iv">盐</param>
    /// <param name="mode">填充模式</param>
    /// <returns>加密后的数据</returns>
    public static byte[] AES256(byte[] data, string key, string iv)
    {
        return AES256(data, key, iv, PaddingMode.PKCS7);
    }
    public static byte[] AES256(byte[] data, string key, string iv, PaddingMode mode)
    {
        return AES256(data, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv), mode);
    }
    public static byte[] AES256(byte[] data, byte[] key, byte[] iv)
    {
        return AES256(data, key, iv, PaddingMode.PKCS7);
    }

    public static byte[] AES256(byte[] data, byte[] key, byte[] iv, PaddingMode mode)
    {
        using var rDel = Aes.Create();
        var size = rDel.KeySize / 8;
        if (key.Length != size)
        {
            Array.Resize(ref key, size);
        }
        size = rDel.BlockSize / 8;
        if (iv.Length != size)
        {
            Array.Resize(ref iv, size);
        }

        rDel.BlockSize = 128;
        rDel.KeySize = 256;
        rDel.FeedbackSize = 128;
        rDel.Padding = mode;
        rDel.Mode = CipherMode.CBC;
        rDel.Key = key;
        rDel.IV = iv;

        using var cTransform = rDel.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);

        return resultArray;
    }
}
public partial class DeCode
{
    /// <summary>
    /// BASE64解码
    /// </summary>
    /// <param name="data">要解密的数据</param>
    /// <returns>加密后的数据</returns>
    public static string BASE64(string data)
        => Encoding.UTF8.GetString(Convert.FromBase64String(data));
    /// <summary>
    /// AES128解码
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="key">密钥</param>
    /// <param name="iv">盐</param>
    /// <param name="mode">填充模式</param>
    /// <returns>解密后的数据</returns>
    public static byte[] AES128(byte[] data, string key, string iv)
    {
        return AES128(data, key, iv, PaddingMode.PKCS7);
    }
    public static byte[] AES128(byte[] data, string key, string iv, PaddingMode mode)
    {
        return AES128(data, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv), mode);
    }
    public static byte[] AES128(byte[] data, byte[] key, byte[] iv)
    {
        return AES128(data, key, iv, PaddingMode.PKCS7);
    }
    public static byte[] AES128(byte[] data, byte[] key, byte[] iv, PaddingMode mode)
    {
        using var rijalg = Aes.Create();
        var size = rijalg.KeySize / 8;
        if (key.Length != size)
        {
            Array.Resize(ref key, size);
        }
        size = rijalg.BlockSize / 8;
        if (iv.Length != size)
        {
            Array.Resize(ref iv, size);
        }

        rijalg.Padding = mode;
        rijalg.Mode = CipherMode.CBC;
        rijalg.Key = key;
        rijalg.IV = iv;

        using var decryptor = rijalg.CreateDecryptor();
        byte[] resultArray = decryptor.TransformFinalBlock(data, 0, data.Length);

        return resultArray;
    }

    /// <summary>
    /// AES256解密
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="key">密钥</param>
    /// <param name="iv">盐</param>
    /// <param name="mode">填充模式</param>
    /// <returns>解密后的数据</returns>
    public static byte[] AES256(byte[] data, string key, string iv)
    {
        return AES256(data, key, iv, PaddingMode.PKCS7);
    }
    public static byte[] AES256(byte[] data, string key, string iv, PaddingMode mode)
    {
        return AES256(data, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv), mode);
    }
    public static byte[] AES256(byte[] data, byte[] key, byte[] iv)
    {
        return AES256(data, key, iv, PaddingMode.PKCS7);
    }
    public static byte[] AES256(byte[] data, byte[] key, byte[] iv, PaddingMode mode)
    {
        using var rijalg = Aes.Create();
        var size = rijalg.KeySize / 8;
        if (key.Length != size)
        {
            Array.Resize(ref key, size);
        }
        size = rijalg.BlockSize / 8;
        if (iv.Length != size)
        {
            Array.Resize(ref iv, size);
        }

        rijalg.BlockSize = 128;
        rijalg.KeySize = 256;
        rijalg.FeedbackSize = 128;
        rijalg.Padding = mode;
        rijalg.Mode = CipherMode.CBC;
        rijalg.Key = key;
        rijalg.IV = iv;

        using var decryptor = rijalg.CreateDecryptor();

        byte[] resultArray = decryptor.TransformFinalBlock(data, 0, data.Length);

        return resultArray;
    }


    /// <summary>
    /// OPENSSL解码
    /// </summary>
    /// <param name="privateKey">私钥</param>
    /// <param name="data">数据</param>
    /// <param name="fOAEP"></param>
    /// <returns>解密后的数据</returns>
    public static string OpenSSL(string privateKey, string data, bool fOAEP = false)
    {
        using var _privateKeyRsaProvider = Openssl.CreateRsaProviderFromPrivateKey(privateKey);
        return Convert.ToBase64String(_privateKeyRsaProvider.Decrypt(Encoding.UTF8.GetBytes(data), fOAEP));
    }
}
public partial class Tools
{
    /// <summary>
    /// Bytes转HEX字符串
    /// </summary>
    /// <param name="src">Bytes</param>
    /// <returns>字符串</returns>
    public static string BytesToHexString(byte[] src)
    {
        StringBuilder stringBuilder = new();
        if (src == null || src.Length <= 0)
        {
            return "";
        }
        for (int i = 0; i < src.Length; i++)
        {
            int v = src[i] & 0xFF;
            string hv = string.Format("{0:X2}", v);
            stringBuilder.Append(hv);
        }
        return stringBuilder.ToString();
    }
    /// <summary>
    /// HEX字符串转Bytes
    /// </summary>
    /// <param name="hexString">HEX字符串</param>
    /// <returns>HEX数组</returns>
    public static byte[] HexStringToByte(string hex)
    {
        int len = hex.Length / 2;
        byte[] result = new byte[len];
        char[] achar = hex.ToCharArray();
        for (int i = 0; i < len; i++)
        {
            int pos = i * 2;
            result[i] = (byte)(ToByte(achar[pos]) << 4 | ToByte(achar[pos + 1]));
        }
        return result;
    }
    public const string bytelist = "0123456789ABCDEF";
    /// <summary>
    /// Byte转HEX字符
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static byte ToByte(char c)
    {
        byte b = (byte)bytelist.IndexOf(c);
        return b;
    }
    /// <summary>
    /// 截取字符串
    /// </summary>
    /// <param name="a">源字符串</param>
    /// <param name="b">从开始</param>
    /// <param name="c">到结束</param>
    /// <returns>分割后的</returns>
    public static string GetString(string a, string b, string c = null)
    {
        if (b == null)
        {
            return a;
        }
        int x = a.IndexOf(b) + b.Length;
        int y;
        if (c != null)
        {
            y = a.IndexOf(c, x);
            if (a[y - 1] == '"')
            {
                y = a.IndexOf(c, y + 1);
            }
            if (y - x <= 0)
                return a;
            else
                return a[x..y];
        }
        else
            return a[x..];
    }
    /// <summary>
    /// GBK到UTF-8
    /// </summary>
    /// <param name="msg">GBK字符串</param>
    /// <returns>UTF-8字符串</returns>
    public static string GBKtoUTF8(string msg)
    {
        try
        {
            byte[] srcBytes = Encoding.Default.GetBytes(msg);
            byte[] bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, srcBytes);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return msg;
        }
    }
    /// <summary>
    /// UTF-8到GBK
    /// </summary>
    /// <param name="msg">UTF-8字符串</param>
    /// <returns>GBK字符串</returns>
    public static string UTF8toGBK(string msg)
    {
        try
        {
            byte[] srcBytes = Encoding.UTF8.GetBytes(msg);
            byte[] bytes = Encoding.Convert(Encoding.UTF8, Encoding.Default, srcBytes);
            return Encoding.Default.GetString(bytes);
        }
        catch
        {
            return msg;
        }
    }
    /// <summary>
    /// 转成JSON字符串
    /// </summary>
    /// <param name="obj">对象</param>
    /// <returns>json字符串</returns>
    public static string ToJson(object obj)
    {
        try
        {
            return JsonConvert.SerializeObject(obj);
        }
        catch (Exception e)
        {
            throw new ErrorDump("转成Json失败", e);
        }
    }
    /// <summary>
    /// JSON转成对象
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="json">字符串</param>
    /// <returns>对象</returns>
    public static T ToObject<T>(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception e)
        {
            throw new ErrorDump("转成对象失败", e);
        }
    }
    /// <summary>
    /// 字符串转成JSON对象
    /// </summary>
    /// <param name="json">json字符串</param>
    /// <returns>JSON对象</returns>
    public static JObject ToJObject(string json)
    {
        try
        {
            return JObject.Parse(json);
        }
        catch (Exception e)
        {
            throw new ErrorDump("转成JSON对象失败", e);
        }
    }
    /// <summary>
    /// 获取类
    /// </summary>
    /// <param name="classname">类名</param>
    /// <returns>类</returns>
    public static dynamic GetClass(string classname, params object[] obj)
    {
        var data = DllStonge.GetClass(classname);
        if (data == null)
            throw new ErrorDump("没有这个类:" + classname);
        try
        {
            return Activator.CreateInstance(data.SelfType, obj);
        }
        catch (Exception e)
        {
            throw new ErrorDump("创建类:" + classname + "失败", e);
        }
    }
    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns>时间戳</returns>
    public static long GetTimeSpan()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
    /// <summary>
    /// 判断WebSocket客户端是否在线
    /// </summary>
    /// <param name="id">客户端ID</param>
    /// <returns>是否在线</returns>
    public static bool WebSocketIsOnline(string id)
        => ServerWebSocket.IsOnline(id);
    /// <summary>
    /// 获取在线的机器人
    /// </summary>
    /// <returns>机器人QQ号列表</returns>
    public static List<long> GetBots()
        => new(RobotUtils.GetQQs());
    /// <summary>
    /// 压缩HTML
    /// </summary>
    /// <param name="html">原始数据</param>
    /// <returns>压缩后的数据</returns>
    public static string CompressHTML(string html)
        => CodeCompress.HTML(html);
    /// <summary>
    /// 压缩JS
    /// </summary>
    /// <param name="js">原始数据</param>
    /// <returns>压缩后的数据</returns>
    public static string CompressJS(string js)
        => CodeCompress.JS(js);
    /// <summary>
    /// 压缩CSS
    /// </summary>
    /// <param name="css">原始数据</param>
    /// <returns>压缩后的数据</returns>
    public static string CompressCSS(string css)
        => CodeCompress.CSS(css);
    /// <summary>
    /// 获取机器人
    /// </summary>
    /// <returns>机器人</returns>
    public static RobotSDK GetRobot()
        => RobotUtils.robot;
    /// <summary>
    /// 获取一个WebSocket客户端
    /// </summary>
    /// <param name="uuid">客户端UUID</param>
    /// <returns>WebSocket客户端</returns>
    public static IWebSocketConnection GetWebSocket(Guid uuid)
        => ServerWebSocket.Get(uuid);
    /// <summary>
    /// 获取一个WebSocket客户端
    /// </summary>
    /// <param name="port">端口</param>
    /// <returns>WebSocket客户端</returns>
    public static IWebSocketConnection GetWebSocket(int port)
        => ServerWebSocket.Get(port);
}
public partial class FileLoad
{
    /// <summary>
    /// 从文件加载字符串
    /// </summary>
    /// <param name="filename">文件名</param>
    /// <returns>文件里面的字符串</returns>
    public static string LoadString(string filename, bool isTemp = true)
        => FileTemp.LoadString(filename, isTemp);
    /// <summary>
    /// 读一个文件
    /// </summary>
    /// <param name="filename">文件名</param>
    /// <returns>文件二进制</returns>
    public static byte[] LoadBytes(string filename, bool isTemp = true)
        => FileTemp.LoadBytes(filename, isTemp);
    /// <summary>
    /// 开始文件流
    /// </summary>
    /// <param name="http">请求数据</param>
    /// <param name="local">文件夹</param>
    /// <param name="name">文件名</param>
    /// <returns>流</returns>
    public static HttpResponseStream StartStream(HttpRequest http, string local, string name)
        => FileHttpStream.StartStream(http, local, name);
}
public partial class ErrorDump : Exception
{
    public string data { get; init; }
    public ErrorDump(string data)
    {
        this.data = data;
    }
    public ErrorDump(string data, Exception ex)
    {
        this.data = data + "\n" + GenString(ex);
    }

    private string GenString(Exception ex)
    {
        string a = "";
        Exception b = ex;
        while (b != null)
        { 
            a += b?.Message + "\n" + b?.StackTrace + "\n";
            b = ex.InnerException;
        }
        return a;
    }
}
public class VarDump : Exception
{
    private object[] obj;
    /// <summary>
    /// 变量输出
    /// </summary>
    /// <param name="obj">变量</param>
    public VarDump(params object[] obj)
    {
        this.obj = obj;
    }

    public string Get()
    {
        try
        {
            if (obj.Length == 0)
            {
                return "no obj";
            }
            else if (obj.Length == 1 && obj[0] is string)
            {
                return obj[0] as string;
            }
            var data = new Dictionary<string, object>(new RepeatDictionaryComparer());
            foreach (var item in obj)
            {
                if (item == null)
                {
                    data.Add("null", "null");
                }
                else
                {
                    var name = item.GetType();
                    data.Add(name.Name, item);
                }
            }
            return JsonConvert.SerializeObject(data, Formatting.Indented).Replace("\\\"", "\"");
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
            return "变量输出失败" + e.ToString();
        }
    }
}
class RepeatDictionaryComparer : IEqualityComparer<string>
{
    public bool Equals(string x, string y)
    {
        return x != y;
    }
    public int GetHashCode(string obj)
    {
        return obj.GetHashCode();
    }
}
