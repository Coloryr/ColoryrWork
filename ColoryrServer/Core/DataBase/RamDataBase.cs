﻿using ColoryrServer.FileSystem;
using System.Collections.Concurrent;
using System.Threading;

namespace ColoryrServer.DataBase
{
    public class RamDataBase
    {
        internal static bool State;
        /// <summary>
        /// 缓存数据
        /// </summary>
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, dynamic>> RamCache;

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
                RamCache.TryAdd(item, data);
            }
            SaveThread = new Thread(() =>
            {
                while (State)
                {
                    Thread.Sleep(100);
                    if (QueueSave.TryTake(out var temp))
                    {
                        FileRam.Save(temp, RamCache[temp]);
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
                    FileRam.Save(item.Key, item.Value);
                }
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
        /// 检测是否有缓存名
        /// </summary>
        /// <param name="Name">缓存名</param>
        /// <returns>是否存在</returns>
        internal static bool HaveCache(string Name)
        {
            return RamCache.ContainsKey(Name);
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
            QueueSave.Add(Name);
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
            QueueSave.Add(Name);
            return Name;
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
            QueueRemove.Add(Name);
        }
    }
}