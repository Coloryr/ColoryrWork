using System.Collections.Concurrent;

namespace ColoryrServer.DataBase
{
    internal class RamDataBase
    {
        internal static bool State;
        /// <summary>
        /// 缓存数据
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, dynamic>> RamCache;

        /// <summary>
        /// 初始化缓存
        /// </summary>
        internal static void Start()
        {
            RamCache = new();
            State = true;
        }

        internal static void Stop()
        {
            State = false;
            if (RamCache != null)
            {
                foreach (var item in RamCache.Values)
                {
                    item.Clear();
                }
                RamCache.Clear();
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="Name">缓存名</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        internal static dynamic GetCache(string Name, string key)
        {
            RamCache.TryGetValue(Name, out var temp);
            if (temp == null)
                return null;
            temp.TryGetValue(key, out var temp1);
            return temp1;
        }
        /// <summary>
        /// 检测是否有缓存名的键
        /// </summary>
        /// <param name="Name">缓存名</param>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        internal static bool HaveCacheKey(string Name, string Key)
        {
            if (!HaveCache(Name))
                return false;
            return RamCache[Name].ContainsKey(Key);
        }
        /// <summary>
        /// 设置缓存，若缓存名不存在新建一个
        /// </summary>
        /// <param name="Name">缓存名</param>
        /// <param name="key">键</param>
        /// <param name="Value">值</param>
        internal static void SetCache(string Name, string key, dynamic Value)
        {
            if (!RamCache.ContainsKey(Name))
            {
                var list = new ConcurrentDictionary<string, dynamic>();
                list.TryAdd(key, Value);
                RamCache.TryAdd(Name, list);
            }
            if (!RamCache[Name].ContainsKey(key))
                RamCache[Name].TryAdd(key, Value);
            RamCache[Name][key] = Value;
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <param name="Name">缓存名</param>
        internal static void CloseCache(string Name)
        {
            if (!RamCache.ContainsKey(Name))
                return;
            RamCache.TryRemove(Name, out var temp);
            temp.Clear();
        }
        /// <summary>
        /// 检测是否有缓存名
        /// </summary>
        /// <param name="Name">缓存名</param>
        /// <returns>是否存在</returns>
        internal static bool HaveCache(string Name)
        {
            return RamCache.ContainsKey(Name);
        }

        /// <summary>
        /// 检测缓存名是否存在，不存在创建
        /// </summary>
        /// <param name="Name">缓存名</param>
        /// <returns>缓存名</returns>
        internal static string NewCache(string Name)
        {
            if (!RamCache.ContainsKey(Name))
            {
                var list = new ConcurrentDictionary<string, dynamic>();
                RamCache.TryAdd(Name, list);
            }
            return Name;
        }
    }
}
