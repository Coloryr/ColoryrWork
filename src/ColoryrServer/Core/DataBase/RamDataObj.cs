using System.Collections.Concurrent;

namespace ColoryrServer.Core.DataBase;

/// <summary>
/// 快速方法
/// </summary>
public static class ExtensionMethods
{
    public static void AddOrUpdate<K, V>(this ConcurrentDictionary<K, V> dictionary, K key, V value)
    {
        dictionary.AddOrUpdate(key, value, (oldkey, oldvalue) => value);
    }
}

/// <summary>
/// 内存数据库内容缓存
/// </summary>
public class RamDataObj
{
    /// <summary>
    /// 是否固化
    /// </summary>
    public bool IsSave { get; set; }
    /// <summary>
    /// 数据
    /// </summary>
    public ConcurrentDictionary<string, dynamic> Data { get; init; }

    public RamDataObj(ConcurrentDictionary<string, dynamic> data = null)
    {
        if (data == null)
            Data = new();
        else
            Data = data;
    }

    /// <summary>
    /// 删除缓存
    /// </summary>
    public void Delete()
    {
        Data.Clear();
    }
}