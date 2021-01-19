using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.DataBase
{
    internal class RedisCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static bool State { get; private set; }

        /// <summary>
        /// 连接池
        /// </summary>
        private static ExConn[] Conns;

        private static object LockObj = new object();
        private static int LastIndex = 0;
        private static RedisConfig Config;

        /// <summary>
        /// 获取连接Task
        /// </summary>
        private static Task<ExConn> GetConn = new(() =>
        {
            ExConn item;
            while (true)
            {
                lock (LockObj)
                {
                    item = Conns[LastIndex];
                    LastIndex++;
                    if (LastIndex >= Config.ConnCount)
                        LastIndex = 0;
                }
                if (item.State == ConnState.Ok)
                {
                    item.State = ConnState.Open;
                    return item;
                }
                Thread.Sleep(1);
            }
        });

        /// <summary>
        /// 开启重连Task
        /// </summary>
        /// <param name="item">连接池项目</param>
        public static void ConnReset(ExConn item)
        {
            Task.Run(() =>
            {
                item.State = ConnState.Restart;
                Config = ServerMain.Config.Redis;
                string ConnectString = string.Format(Config.Conn, Config.IP, Config.Port);
                var Conn = ConnectionMultiplexer.Connect(ConnectString);
                item.Redis.Dispose();
                item.Redis = Conn;
                if (Test(item))
                {
                    return;
                }
                else
                {
                    ServerMain.LogError($"Redis数据库重连失败，连接池第{item.Index}个连接");
                }
            });
        }

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private static bool Test(ExConn item)
        {
            try
            {
                if (item.Redis == null || item.Redis.IsConnected == false)
                {
                    return false;
                }
                item.Redis.GetDatabase().KeyExists("test");
                item.Redis.Close();
                item.State = ConnState.Ok;
                return true;
            }
            catch (Exception ex)
            {
                ServerMain.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Mysql初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public static dynamic Start()
        {
            Config = ServerMain.Config.Redis;
            string ConnStr = string.Format(Config.Conn, Config.IP, Config.Port);
            Conns = new ExConn[Config.ConnCount];
            for (int a = 0; a < Config.ConnCount; a++)
            {
                try
                {
                    var Conn = new ExConn
                    {
                        Redis = ConnectionMultiplexer.Connect(ConnStr),
                        Index = a,
                        State = ConnState.Error,
                        Type = ConnType.Redis
                    };
                    if (Test(Conn))
                    {
                        Conns[a] = Conn;
                    }
                    else
                    {
                        State = false;
                        return false;
                    }
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                    State = false;
                    return false;
                }
            }
            State = true;
            return true;
        }

        /// <summary>
        /// 关闭Redis数据库连接
        /// </summary>
        public static void Stop()
        {
            if (State)
            {
                State = false;
                foreach (var a in Conns)
                {
                    a.State = ConnState.Close;
                    a.Redis.Dispose();
                }
                ServerMain.LogOut("Redis数据库已断开");
            }
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static RedisValue Get(string key)
        {
            try
            {
                var task = GetConn;
                if (Task.WhenAny(task, Task.Delay(Config.TimeOut)).Result == task)
                {
                    var conn = task.Result;
                    var data = conn.Redis.GetDatabase().StringGet(key);
                    conn.Redis.Close();
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    ConnReset(task.Result);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireMinutes">存在秒</param>
        public static bool Set(string key, string value, int expireMinutes)
        {
            try
            {
                var task = GetConn;
                if (Task.WhenAny(task, Task.Delay(Config.TimeOut)).Result == task)
                {
                    var conn = task.Result;
                    bool data = false;
                    if (expireMinutes > 0)
                    {
                        data = conn.Redis.GetDatabase().StringSet(key, value, TimeSpan.FromSeconds(expireMinutes));
                    }
                    else
                    {
                        data = conn.Redis.GetDatabase().StringSet(key, value);
                    }
                    conn.Redis.Close();
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    ConnReset(task.Result);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }
        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string key)
        {
            try
            {
                var task = GetConn;
                if (Task.WhenAny(task, Task.Delay(Config.TimeOut)).Result == task)
                {
                    var conn = task.Result;
                    var data = conn.Redis.GetDatabase().KeyExists(key);
                    conn.Redis.Close();
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    ConnReset(task.Result);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }
        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否移除</returns>
        public static bool Remove(string key)
        {
            try
            {
                var task = GetConn;
                if (Task.WhenAny(task, Task.Delay(Config.TimeOut)).Result == task)
                {
                    var conn = task.Result;
                    var data = conn.Redis.GetDatabase().KeyDelete(key);
                    conn.Redis.Close();
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    ConnReset(task.Result);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }
        /// <summary>
        /// 数据累加
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>累加后的值</returns>
        public static long Increment(string key, long val = 1)
        {
            try
            {
                var task = GetConn;
                if (Task.WhenAny(task, Task.Delay(Config.TimeOut)).Result == task)
                {
                    var conn = task.Result;
                    var data = conn.Redis.GetDatabase().StringIncrement(key, val);
                    conn.Redis.Close();
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    ConnReset(task.Result);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }
    }
}
