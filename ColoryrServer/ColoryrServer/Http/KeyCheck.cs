namespace ColoryrServer.Http
{
    //class VaildOBJ
    //{
    //    public bool IsVaild { get; set; }
    //    public string AppKey { get; set; }
    //    public string DataKey { get; set; }
    //    public string Setting { get; set; }
    //    public KeyType Type { get; set; }
    //}
    //class KeyCheck
    //{
    //    public static Dictionary<string, string> Token { get; set; } = new Dictionary<string, string>();
    //    private static object LockObj = new object();

    //    public static VaildOBJ GenKey(string appkey, string DataKey = null, string setting = null)
    //    {
    //        var Temp = Guid.NewGuid().ToString();
    //        if (DataKey == null)
    //        {
    //            Token.Add(Temp, Temp);
    //            return new VaildOBJ
    //            {
    //                AppKey = appkey,
    //                Setting = Temp,
    //                Type = KeyType.Self
    //            };
    //        }
    //        if (Token.ContainsKey(DataKey))
    //        {
    //            Token.Remove(DataKey);
    //        }
    //        Token.Add(setting, Temp);
    //        return new VaildOBJ
    //        {
    //            Setting = Temp,
    //            Type = KeyType.Self
    //        };
    //    }
    //    public static VaildOBJ IsVaild(NameValueCollection Hashtable, string UUID)
    //    {
    //        CSFileObj Data;
    //        if (!CSFile.DllFileList.TryGetValue(UUID, out Data))
    //            return new VaildOBJ();

    //        switch (Data.KeyType)
    //        {
    //            case KeyType.Self:
    //                return Vaild(Hashtable, Data.Key);
    //            case KeyType.Yiban:
    //                return Vaild(Hashtable, UUID, Data.Key);
    //        }
    //        return new VaildOBJ();
    //    }
    //    private static VaildOBJ Vaild(NameValueCollection Hashtable, string Key)
    //    {
    //        string appkey = Hashtable["in-appkey"];
    //        string datakey = Hashtable["in-datakey"];
    //        string setting = Hashtable["in-setting"];

    //        if (appkey != null && datakey != null && setting != null)
    //        {
    //            lock (LockObj)
    //            {
    //                if (appkey == Key && Token.TryGetValue(datakey, out var temp))
    //                {
    //                    if (temp == setting)
    //                    {
    //                        var re = GenKey(Key, datakey, setting);
    //                        re.IsVaild = true;
    //                        return re;
    //                    }
    //                }
    //            }
    //        }
    //        return GenKey(Key);
    //    }
    //}
}
