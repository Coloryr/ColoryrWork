namespace ColoryrServer.ASP
{
    public class ASPHttpUtils
    {
        public static string HaveCookie(IHeaderDictionary hashtable)
        {
            string Temp = hashtable.Cookie;
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
