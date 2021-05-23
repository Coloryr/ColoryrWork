using System;
using System.Security.Cryptography;
using System.Text;

namespace Lib.Build
{
    public class Utils
    {
        public static string GetSHA1(string data)
        {
            return BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", "");
        }
    }
}
