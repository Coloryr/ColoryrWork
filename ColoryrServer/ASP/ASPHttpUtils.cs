namespace ColoryrServer.ASP
{
    public class ASPHttpUtils
    {
        public static string HaveCookie(string hashtable)
        {
            string[] Cookies = hashtable.Split(';');
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
