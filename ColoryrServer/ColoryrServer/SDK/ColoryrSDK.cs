using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Robot;
using ColoryrServer.Utils;
using ColoryrServer.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ColoryrServer.SDK
{
    public class HttpMultipartFile
    {
        public Stream Data { get; set; }
        public string FileName { get; set; }
    }
    public class ServerContentType
    {
        public const string OSTREAM = "application/octet-stream";
        public const string POSTXFORM = "application/x-www-form-urlencoded";
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
    }
    public enum MyContentType
    {
        Json, XFormData, MFormData, Other
    }
    public enum EncodeType
    {
        UTF8, Default, ASCII
    }
    public class EnCode
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
            try
            {
                using var _publicKeyRsaProvider = Openssl.CreateRsaProviderFromPublicKey(publicKey);
                return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(data), fOAEP));
            }
            catch (Exception e)
            {
                throw new ErrorDump("密钥错误", e);
            }
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
        /// <returns>加密后的数据</returns>
        public static byte[] AES128(byte[] data, string key, string iv)
        {
            if (key.Length != 16)
            {
                if (key.Length > 16)
                {
                    key = key[..15];
                }
                else
                {
                    key += new string(new char[16 - key.Length]);
                }
            }
            if (iv.Length != 8)
            {
                if (iv.Length > 8)
                {
                    iv = iv[..7];
                }
                else
                {
                    iv += new string(new char[8 - iv.Length]);
                }
            }
            return AES128(data, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
        }
        public static byte[] AES128(byte[] data, byte[] key, byte[] iv)
        {
            try
            {
                using var rDel = Aes.Create();
                rDel.Key = key;
                rDel.IV = iv;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.PKCS7;

                using var cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);

                return resultArray;
            }
            catch (Exception e)
            {
                throw new ErrorDump("加密失败", e);
            }
        }
        /// <summary>
        /// AES-256-CBC加密
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="key">密匙</param>
        /// <param name="iv">盐</param>
        /// <returns>加密后的数据</returns>
        public static byte[] AES256(byte[] data, string key, string iv)
        {
            if (key.Length != 32)
            {
                if (key.Length > 32)
                {
                    key = key[..31];
                }
                else
                {
                    key += new string(new char[32 - key.Length]);
                }
            }
            if (iv.Length != 16)
            {
                if (iv.Length > 16)
                {
                    iv = iv[..15];
                }
                else
                {
                    iv += new string(new char[16 - iv.Length]);
                }
            }
            return AES256(data, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
        }
        public static byte[] AES256(byte[] data, byte[] key, byte[] iv)
        {
            try
            {
                using var rDel = Aes.Create();
                rDel.BlockSize = 128;
                rDel.KeySize = 256;
                rDel.FeedbackSize = 128;
                rDel.Padding = PaddingMode.PKCS7;
                rDel.Mode = CipherMode.CBC;
                rDel.Key = key;
                rDel.IV = iv;

                using var cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(data, 0, data.Length);

                return resultArray;
            }
            catch (Exception e)
            {
                throw new ErrorDump("加密失败", e);
            }
        }
    }
    public class DeCode
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
        /// <returns>解密后的数据</returns>
        public static byte[] AES128(byte[] data, string key, string iv)
        {
            if (key.Length != 16)
            {
                if (key.Length > 16)
                {
                    key = key[..15];
                }
                else
                {
                    key += new string(new char[16 - key.Length]);
                }
            }
            if (iv.Length != 8)
            {
                if (iv.Length > 8)
                {
                    iv = iv[..7];
                }
                else
                {
                    iv += new string(new char[8 - iv.Length]);
                }
            }
            return AES128(data, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
        }
        public static byte[] AES128(byte[] data, byte[] key, byte[] iv)
        {
            using var rijalg = Aes.Create();
            rijalg.Padding = PaddingMode.PKCS7;
            rijalg.Mode = CipherMode.CBC;
            rijalg.Key = key;
            rijalg.IV = iv;

            using var decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

            try
            {
                byte[] resultArray = decryptor.TransformFinalBlock(data, 0, data.Length);

                return resultArray;
            }
            catch (Exception e)
            {
                throw new ErrorDump("解密失败", e);
            }
        }
        /// <summary>
        /// AES256解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">盐</param>
        /// <returns>解密后的数据</returns>
        public static byte[] AES256(byte[] data, string key, string iv)
        {
            if (key.Length != 32)
            {
                if (key.Length > 32)
                {
                    key = key[..31];
                }
                else
                {
                    key += new string(new char[32 - key.Length]);
                }
            }
            if (iv.Length != 16)
            {
                if (iv.Length > 16)
                {
                    iv = iv[..15];
                }
                else
                {
                    iv += new string(new char[16 - iv.Length]);
                }
            }
            return AES256(data, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
        }
        public static byte[] AES256(byte[] data, byte[] key, byte[] iv)
        {
            using var rijalg = Aes.Create();
            rijalg.BlockSize = 128;
            rijalg.KeySize = 256;
            rijalg.FeedbackSize = 128;
            rijalg.Padding = PaddingMode.PKCS7;
            rijalg.Mode = CipherMode.CBC;
            rijalg.Key = key;
            rijalg.IV = iv;

            using var decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

            try
            {
                byte[] resultArray = decryptor.TransformFinalBlock(data, 0, data.Length);

                return resultArray;
            }
            catch (Exception e)
            {
                throw new ErrorDump("解密失败", e);
            }
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
            try
            {
                using var _privateKeyRsaProvider = Openssl.CreateRsaProviderFromPrivateKey(privateKey);
                return Convert.ToBase64String(_privateKeyRsaProvider.Decrypt(Encoding.UTF8.GetBytes(data), fOAEP));
            }
            catch
            {
                throw new ErrorDump("密钥错误");
            }
        }
    }
    public class Tools
    {
        /// <summary>
        /// HEX字符串到HEX
        /// </summary>
        /// <param name="hexString">HEX字符串</param>
        /// <returns>HEX数组</returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
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
                return Activator.CreateInstance(data.DllType, obj);
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
        /// 转码
        /// </summary>
        /// <param name="InputType">输入格式</param>
        /// <param name="OutputType">输出格式</param>
        /// <param name="InputData">输入数据</param>
        /// <returns>转码后的数据</returns>
        public static byte[] Transcoding(TranscodeType InputType, TranscodeType OutputType, byte[] InputData)
        {
            try
            {
                return Transcode.Start(InputType, OutputType, InputData);
            }
            catch (Exception e)
            {
                throw new ErrorDump("转码失败", e);
            }
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
    }
    public class FileLoad
    {
        /// <summary>
        /// 从文件加载字符串
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件里面的字符串</returns>
        public static string LoadString(string filename)
            => FileTemp.LoadString(filename);
        /// <summary>
        /// 读一个文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件二进制</returns>
        public static byte[] LoadBytes(string filename)
            => FileTemp.LoadBytes(filename);
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
    public class ErrorDump : Exception
    {
        public string data { get; set; }
        public ErrorDump(string data)
        {
            this.data = data;
        }
        public ErrorDump(string data, Exception ex)
        {
            this.data = data + "\n" + ex.ToString();
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
                    var name = item.GetType();
                    data.Add(name.Name, item);
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
    internal class RepeatDictionaryComparer : IEqualityComparer<string>
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
}