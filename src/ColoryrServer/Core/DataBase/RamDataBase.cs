using ColoryrServer.Core.FileSystem;
using System.Collections.Concurrent;
using System.Threading;

namespace ColoryrServer.Core.Database;

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

    public RamDataObj(ConcurrentDictionary<string, dynamic>? data = null)
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

internal static class RamDatabase
{
    internal static bool State;
    /// <summary>
    /// 缓存数据
    /// </summary>
    private static readonly ConcurrentDictionary<string, RamDataObj> RamCache = [];

    private static readonly ConcurrentBag<string> QueueSave = [];
    private static readonly ConcurrentBag<string> QueueRemove = [];
    private static Thread SaveThread;

    /// <summary>
    /// 停止内存数据库
    /// </summary>
    private static void Stop()
    {
        State = false;
        foreach (var item in RamCache)
        {
            if (item.Value.IsSave)
                FileRam.Save(item.Key, item.Value.Data);
        }
        RamCache.Clear();
        QueueSave.Clear();
        QueueRemove.Clear();
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="name">缓存名</param>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    internal static dynamic? GetCache(string name, string key)
    {
        RamCache.TryGetValue(name, out var temp);
        if (temp == null)
            return null;
        temp.Data.TryGetValue(key, out var temp1);
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
        return RamCache[name].Data.ContainsKey(Key);
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
            item.Data.TryAdd(key, value);
            RamCache.TryAdd(name, item);
        }
        if (!RamCache[name].Data.ContainsKey(key))
            RamCache[name].Data.TryAdd(key, value);
        RamCache[name].Data[key] = value;
        if (RamCache[name].IsSave)
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
        if (!RamCache.TryGetValue(name, out RamDataObj? value))
        {
            RamCache.TryAdd(name, item);
        }
        else
        {
            value.IsSave = save;
        }
        if (save)
        {
            QueueSave.Add(name);
        }
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
        if (RamCache.TryRemove(name, out var temp))
        {
            temp.Data.Clear();
            if (temp.IsSave == true)
            {
                QueueRemove.Add(name);
            }
        }
    }

    /// <summary>
    /// 初始化缓存
    /// </summary>
    internal static void Start()
    {
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
                    FileRam.Save(temp, RamCache[temp].Data);
                }
                if (QueueRemove.TryTake(out var temp1))
                {
                    FileRam.Remove(temp1);
                }
            }
        })
        {
            Name = "RamDataThread"
        };
        SaveThread.Start();
        ServerMain.OnStop += Stop;
    }
}
