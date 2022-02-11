using System;
using System.Security.Cryptography;
using System.Text;

namespace ColoryrWork.Lib.Build
{
    public class BuildUtils
    {
        public static string GetSHA1(string data)
        {
            return BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "");
        }
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

        private const string bytelist = "0123456789ABCDEF";

        private static byte ToByte(char c)
        {
            byte b = (byte)bytelist.IndexOf(c);
            return b;
        }
    }
}
