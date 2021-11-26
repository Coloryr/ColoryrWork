﻿using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.DataBase
{
    public class RedisCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static Dictionary<int, bool> State = new();

        /// <summary>
        /// 连接池
        /// </summary>
        private static List<RedisConfig> Config;
        private static Dictionary<int, string> ConnectStr = new();

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private static bool Test(ConnectionMultiplexer item)
        {
            try
            {
                if (item.IsConnected == false)
                {
                    return false;
                }
                item.GetDatabase().KeyExists("test");
                return true;
            }
            catch (Exception ex)
            {
                ServerMain.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Redis初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public static void Start()
        {
            ServerMain.LogOut($"正在连接Redis数据库");
            Config = ServerMain.Config.Redis;
            for (int a = 0; a < Config.Count; a++)
            {
                var config = Config[a];
                if (!config.Enable)
                    continue;
                string ConnectString = string.Format(config.Conn, config.IP, config.Port);
                State.Add(a, false);
                for (int b = 0; b < config.ConnCount; b++)
                {
                    var Conn = ConnectionMultiplexer.Connect(ConnectString);
                    if (Test(Conn))
                    {
                        ConnectStr.Add(a, ConnectString);
                        State[a] = true;
                        ServerMain.LogOut($"Redis数据库{a}已连接");
                    }
                    else
                    {
                        ServerMain.LogError($"Redis数据库{a}连接失败");
                    }
                }
            }
        }

        /// <summary>
        /// 关闭Redis数据库连接
        /// </summary>
        public static void Stop()
        {
            foreach (var item in State)
            {
                State[item.Key] = false;
            }
            ServerMain.LogOut("Redis数据库已断开");
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static RedisValue Get(string key, int id)
        {
            try
            {
                var conn = ConnectionMultiplexer.Connect(ConnectStr[id]);
                return conn.GetDatabase().StringGet(key);
            }
            catch (Exception e)
            {
                throw new ErrorDump("Redis错误", e);
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireMinutes">存在秒</param>
        public static bool Set(string key, string value, int expireMinutes, int id)
        {
            try
            {
                var conn = ConnectionMultiplexer.Connect(ConnectStr[id]);
                return conn.GetDatabase().StringSet(key, value);
            }
            catch (Exception e)
            {
                throw new ErrorDump("Redis错误", e);
            }
        }
        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string key, int id)
        {
            try
            {
                var conn = ConnectionMultiplexer.Connect(ConnectStr[id]);
                return conn.GetDatabase().KeyExists(key);
            }
            catch (Exception e)
            {
                throw new ErrorDump("Redis错误", e);
            }
        }
        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否移除</returns>
        public static bool Remove(string key, int id)
        {
            try
            {
                var conn = ConnectionMultiplexer.Connect(ConnectStr[id]);
                return conn.GetDatabase().KeyDelete(key);
            }
            catch (Exception e)
            {
                throw new ErrorDump("Redis错误", e);
            }
        }
        /// <summary>
        /// 数据累加
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>累加后的值</returns>
        public static long Increment(string key, int id, long val = 1)
        {
            try
            {
                var conn = ConnectionMultiplexer.Connect(ConnectStr[id]);
                return conn.GetDatabase().StringIncrement(key, val);
            }
            catch (Exception e)
            {
                throw new ErrorDump("Redis错误", e);
            }
        }
    }
}
