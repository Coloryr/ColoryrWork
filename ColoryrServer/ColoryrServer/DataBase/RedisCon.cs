using ColoryrSDK;
using StackExchange.Redis;
using System;

namespace ColoryrServer.DataBase
{
    class RedisCon
    {
        public static bool State { get; private set; }
        private static object LockObj = new object();
        private static ConnectionMultiplexer[] Conns;
        private static int LastIndex = 0;
        private static bool isRun;
        public static ConnectionMultiplexer GetConn()
        {
            if (isRun == false)
                throw new VarDump("Redis服务器错误");
            lock (LockObj)
            {
                ConnectionMultiplexer Item;
                while (true)
                {
                    Item = Conns[LastIndex];
                    if (Item == null)
                        throw new VarDump("Redis服务器错误");
                    LastIndex++;
                    if (LastIndex >= ServerMain.Config.Redis.ConnCount)
                        LastIndex = 0;
                    return Item;
                }
            }
        }
        /// <summary>
        /// Mysql初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public static dynamic Start()
        {
            string ConnStr = "{0}:{1}";
            ConnStr = string.Format(ConnStr, ServerMain.Config.Redis.IP, ServerMain.Config.Redis.Port);
            Conns = new ConnectionMultiplexer[ServerMain.Config.Redis.ConnCount];
            for (int a = 0; a < ServerMain.Config.Redis.ConnCount; a++)
            {
                try
                {
                    var Conn = ConnectionMultiplexer.Connect(ConnStr);
                    if (Conn == null || Conn.IsConnected == false)
                    {
                        ServerMain.LogError("Redis连接失败");
                    }
                    Conns[a] = Conn;
                }
                catch (Exception ex)
                {
                    ServerMain.LogError(ex);
                    State = false;
                    return false;
                }
            }
            State = true;
            isRun = true;
            return true;
        }
        public static void Stop()
        {
            if (State)
                foreach (var item in Conns)
                {
                    if (item != null)
                    {
                        item.Close();
                        item.Dispose();
                    }
                }
            ServerMain.LogOut("Redis已关闭");
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static RedisValue Get(string key)
        {
            var Conn = GetConn();
            return Conn.GetDatabase().StringGet(key);
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireMinutes">存在秒</param>
        public static bool Set(string key, string value, int expireMinutes)
        {
            var Conn = GetConn();
            if (expireMinutes > 0)
            {
                return Conn.GetDatabase().StringSet(key, value, TimeSpan.FromSeconds(expireMinutes));
            }
            else
            {
                return Conn.GetDatabase().StringSet(key, value);
            }
        }
        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            var Conn = GetConn();
            return Conn.GetDatabase().KeyExists(key); //可直接调用
        }
        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Remove(string key)
        {
            var Conn = GetConn();
            return Conn.GetDatabase().KeyDelete(key);
        }
        /// <summary>
        /// 数据累加
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long Increment(string key, long val = 1)
        {
            var Conn = GetConn();
            return Conn.GetDatabase().StringIncrement(key, val);
        }
    }
}
