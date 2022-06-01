using ColoryrServer.Core.FileSystem;
using System.Collections.Concurrent;
using System.Threading;

namespace ColoryrServer.Core.DataBase;

public class RamDataBase
{
    internal static bool State;
    /// <summary>
    /// 缓存数据
    /// </summary>
    private static ConcurrentDictionary<string, RamDataObj> RamCache;

    private static ConcurrentBag<string> QueueSave;
    private static ConcurrentBag<string> QueueRemove;
    private static Thread SaveThread;

    /// <summary>
    /// 初始化缓存
    /// </summary>
    public static void Start()
    {
        RamCache = new();
        QueueSave = new();
        QueueRemove = new();
        State = true;
        var list = FileRam.GetAll();
        foreach (var item in list)
        {
            var data = FileRam.Load(item);
            if (data == null)
                continue;
            RamCache.TryAdd(item, new RamDataObj(data));
        }
        SaveThread = new Thread(() =>
        {
            while (State)
            {
                Thread.Sleep(100);
                if (QueueSave.TryTake(out var temp))
                {
                    FileRam.Save(temp, RamCache[temp].list);
                }
                if (QueueRemove.TryTake(out var temp1))
                {
                    FileRam.Remove(temp1);
                }
            }
        });
        SaveThread.Start();
    }

    public static void Stop()
    {
        State = false;
        if (RamCache != null)
        {
            foreach (var item in RamCache)
            {
                if (item.Value.IsSave)
                    FileRam.Save(item.Key, item.Value.list);
            }
        }
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="name">缓存名</param>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    internal static dynamic GetCache(string name, string key)
    {
        RamCache.TryGetValue(name, out var temp);
        if (temp == null)
            return null;
        temp.list.TryGetValue(key, out var temp1);
        return temp1;
    }
    /// <summary>
    /// 检测是否有缓存名
    /// </summary>
    /// <param name="name">缓存名</param>
    /// <returns>是否存在</returns>
    internal static bool HaveCache(string name)
    {
        return RamCache.ContainsKey(name);
    }
    /// <summary>
    /// 检测是否有缓存名的键
    /// </summary>
    /// <param name="name">缓存名</param>
    /// <param name="key">键</param>
    /// <returns>是否存在</returns>
    internal static bool HaveCacheKey(string name, string Key)
    {
        if (!HaveCache(name))
            return false;
        return RamCache[name].list.ContainsKey(Key);
    }
    /// <summary>
    /// 设置缓存，若缓存名不存在新建一个
    /// </summary>
    /// <param name="name">缓存名</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    internal static void SetCache(string name, string key, dynamic value)
    {
        if (!RamCache.ContainsKey(name))
        {
            var item = new RamDataObj();
            item.list.TryAdd(key, value);
            RamCache.TryAdd(name, item);
        }
        if (!RamCache[name].list.ContainsKey(key))
            RamCache[name].list.TryAdd(key, value);
        RamCache[name].list[key] = value;
        QueueSave.Add(name);
    }
    /// <summary>
    /// 检测缓存名是否存在，不存在创建
    /// </summary>
    /// <param name="name">缓存名</param>
    /// <returns>缓存名</returns>
    internal static string NewCache(string name, bool save = false)
    {
        var item = new RamDataObj()
        {
            IsSave = save
        };
        if (!RamCache.ContainsKey(name))
        {
            RamCache.TryAdd(name, item);
        }
        else
        {
            RamCache[name].IsSave = save;
        }
        QueueSave.Add(name);
        return name;
    }
    /// <summary>
    /// 清空缓存
    /// </summary>
    /// <param name="Name">缓存名</param>
    internal static void CloseCache(string name)
    {
        if (!RamCache.ContainsKey(name))
            return;
        RamCache.TryRemove(name, out var temp);
        temp.list.Clear();
        QueueRemove.Add(name);
    }
}
