using System.Collections.Specialized;

namespace ColoryrServer.NoASP
{
    class HttpUtils
    {
        public static string HaveCookie(NameValueCollection hashtable)
        {
            string Temp = hashtable["Cookie"];
            if (Temp == null)
                return null;
            string[] Cookies = Temp.Split(';');
            foreach (var Item in Cookies)
            {
                var temp = Item.Replace(" ", "");
                if (temp.StartsWith("cs="))
                {
                    return temp.Replace("cs=", "");
                }
            }
            return null;
        }
    }
}
