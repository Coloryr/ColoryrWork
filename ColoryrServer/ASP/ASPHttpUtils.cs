namespace ColoryrServer.ASP
{
    public class ASPHttpUtils
    {
        public static Dictionary<string, List<string>> HaveCookie(string hashtable)
        {
            if (hashtable == null)
                return new();
            var list = new Dictionary<string, List<string>>();
            string[] cookies = hashtable.Split(';');
            foreach (var item in cookies)
            {
                var temp = item.Split("=");
                if (temp.Length == 1)
                {
                    list.Add(temp[0].Trim(), new());
                }
                else
                {
                    string key = temp[0].Trim();
                    if (list.TryGetValue(key, out var list1))
                    {
                        list1.Add(temp[1].Trim());
                    }
                    else
                    {
                        list.Add(key, new List<string>() { temp[1].Trim() });
                    }
                }
            }
            return list;
        }
    }
}
