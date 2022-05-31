using System.Collections.Concurrent;

namespace ColoryrServer.Core.DataBase;

public static class ExtensionMethods
{
    public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
    {
        dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
    }
}

public class RamDataObj
{
    public bool IsSave { get; set; }
    public ConcurrentDictionary<string, dynamic> list { get; init; }

    public RamDataObj()
    {
        list = new();
    }

    public RamDataObj(ConcurrentDictionary<string, dynamic> lists)
    {
        list = lists;
    }

    public void Delete()
    {
        list.Clear();
    }
}