namespace ColoryrServer.ASP;

public static class ASPHttpUtils
{
    public static Dictionary<string, List<string>> HaveCookie(string? hashtable)
    {
        if (hashtable == null)
            return [];
        var list = new Dictionary<string, List<string>>();
        string[] cookies = hashtable.Split(';');
        foreach (var item in cookies)
        {
            var temp = item.Split("=");
            if (temp.Length == 1)
            {
                list.Add(temp[0].Trim(), []);
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
                    list.Add(key, [temp[1].Trim()]);
                }
            }
        }
        return list;
    }
}
