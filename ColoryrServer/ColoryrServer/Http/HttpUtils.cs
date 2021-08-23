using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Http
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
