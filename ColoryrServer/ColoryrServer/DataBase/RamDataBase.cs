using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ColoryrServer.DataBase
{
    class RamDataBase
    {
        /// <summary>
        /// 缓存数据
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, dynamic>> RamCache = new();

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
        internal static string GetNewCache(string Name = null)
        {
            if (!string.IsNullOrWhiteSpace(Name) && RamCache.ContainsKey(Name))
                return Name;
            else
            {
                var stringCookie = Guid.NewGuid().ToString();
                while (RamCache.ContainsKey(stringCookie))
                {
                    stringCookie = Guid.NewGuid().ToString();
                }
                return stringCookie;
            }
        }
        internal static void SetCache(string Name, string key, dynamic value)
        {
            if (!RamCache.ContainsKey(Name))
            {
                var list = new ConcurrentDictionary<string, dynamic>();
                list.TryAdd(key, value);
                RamCache.TryAdd(Name, list);
            }
            if (!RamCache[Name].ContainsKey(key))
                RamCache[Name].TryAdd(key, value);
            RamCache[Name][key] = value;
        }
        internal static void CloseCache(string Name)
        {
            if (!RamCache.ContainsKey(Name))
                return;
            RamCache.TryRemove(Name, out var temp);
            temp.Clear();
        }

        internal static bool HaveCache(string Name)
        {
            return RamCache.ContainsKey(Name);
        }
    }
}
