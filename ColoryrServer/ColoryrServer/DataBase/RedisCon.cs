using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.DataBase
{
    internal class RedisCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static Dictionary<int, bool> State = new();

        /// <summary>
        /// 连接池
        /// </summary>
        private static Dictionary<int, ExConn[]> Conns = new();
        private static Dictionary<int, object> LockObj = new();
        private static Dictionary<int, int> LastIndex = new();
        private static List<RedisConfig> Config;

        /// <summary>
        /// 获取连接Task
        /// </summary>
        private static ExConn GetConn(int id)
        {
            ExConn item;
            while (true)
            {
                lock (LockObj[id])
                {
                    item = Conns[id][LastIndex[id]];
                    LastIndex[id]++;
                    if (LastIndex[id] >= Config[id].ConnCount)
                        LastIndex[id] = 0;
                }
                if (item.State == ConnState.Ok)
                {
                    return item;
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 开启重连Task
        /// </summary>
        /// <param name="item">连接池项目</param>
        public static void ConnReset(ExConn item)
        {
            item.State = ConnState.Restart;
            Config = ServerMain.Config.Redis;
            if (Config.Count <= item.id)
            {
                ServerMain.LogError($"Redis配置出错，id:{item.id}");
                item.State = ConnState.Stop;
                return;
            }
            var config = Config[item.id];
            string ConnectString = string.Format(config.Conn, config.IP, config.Port);
            var Conn = ConnectionMultiplexer.Connect(ConnectString);
            item.Redis.Dispose();
            item.Redis = Conn;
            if (Test(item))
            {
                item.State = ConnState.Ok;
                return;
            }
            else
            {
                ServerMain.LogError($"Redis数据库重连失败，连接池第{item.Index}个连接");
            }
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
                var conn = new ExConn[config.ConnCount];
                State.Add(a, false);
                LockObj.Add(a, new());
                bool isok = false;
                for (int b = 0; b < config.ConnCount; b++)
                {
                    try
                    {
                        var Conn = ConnectionMultiplexer.Connect(ConnectString);
                        var item = new ExConn
                        {
                            id = a,
                            State = ConnState.Close,
                            Type = ConnType.Redis,
                            Redis = Conn,
                            Index = b
                        };
                        if (Test(item))
                        {
                            item.State = ConnState.Ok;
                            conn[b] = item;
                        }
                        else
                        {
                            foreach (var item1 in conn)
                            {
                                if (item1 != null)
                                    item1.Redis.Dispose();
                            }
                            ServerMain.LogError($"Redis数据库{a}连接失败");
                            isok = false;
                            break;
                        }
                    }
                    catch(Exception e)
                    {
                        foreach (var item1 in conn)
                        {
                            if (item1 != null)
                                item1.Redis.Dispose();
                        }
                        ServerMain.LogError(e);
                        ServerMain.LogError($"Redis数据库{a}连接失败");
                        isok = false;
                        break;
                    }
                }
                if (!isok)
                {
                    continue;
                }
                State[a] = true;
                Conns.Add(a, conn);
                ServerMain.LogOut($"Redis数据库{a}已连接");
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
            foreach (var item in Conns)
            {
                foreach (var item1 in item.Value)
                {
                    item1.State = ConnState.Close;
                    item1.Redis.Dispose();
                }
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
                ExConn conn = null;
                CancellationTokenSource cancel = new();
                var task = Task.Run(() =>
                {
                    conn = GetConn(id);
                }, cancel.Token);
                if (Task.WhenAny(task, Task.Delay(Config[id].TimeOut)).Result == task)
                {
                    var data = conn.Redis.GetDatabase().StringGet(key);
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    cancel.Cancel(false);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump("Redis错误", e);
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
                ExConn conn = null;
                CancellationTokenSource cancel = new();
                var task = Task.Run(() =>
                {
                    conn = GetConn(id);
                }, cancel.Token);
                if (Task.WhenAny(task, Task.Delay(Config[id].TimeOut)).Result == task)
                {
                    bool data = false;
                    if (expireMinutes > 0)
                    {
                        data = conn.Redis.GetDatabase().StringSet(key, value, TimeSpan.FromSeconds(expireMinutes));
                    }
                    else
                    {
                        data = conn.Redis.GetDatabase().StringSet(key, value);
                    }
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    cancel.Cancel(false);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump("Redis错误", e);
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
                ExConn conn = null;
                CancellationTokenSource cancel = new();
                var task = Task.Run(() =>
                {
                    conn = GetConn(id);
                }, cancel.Token);
                if (Task.WhenAny(task, Task.Delay(Config[id].TimeOut)).Result == task)
                {
                    var data = conn.Redis.GetDatabase().KeyExists(key);
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    cancel.Cancel(false);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump("Redis错误", e);
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
                ExConn conn = null;
                CancellationTokenSource cancel = new();
                var task = Task.Run(() =>
                {
                    conn = GetConn(id);
                }, cancel.Token);
                if (Task.WhenAny(task, Task.Delay(Config[id].TimeOut)).Result == task)
                {
                    var data = conn.Redis.GetDatabase().KeyDelete(key);
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    cancel.Cancel(false);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump("Redis错误", e);
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
                ExConn conn = null;
                CancellationTokenSource cancel = new();
                var task = Task.Run(() =>
                {
                    conn = GetConn(id);
                }, cancel.Token);
                if (Task.WhenAny(task, Task.Delay(Config[id].TimeOut)).Result == task)
                {
                    var data = conn.Redis.GetDatabase().StringIncrement(key, val);
                    conn.State = ConnState.Ok;
                    return data;
                }
                else
                {
                    cancel.Cancel(false);
                    throw new VarDump("Redis数据库超时");
                }
            }
            catch (Exception e)
            {
                throw new VarDump("Redis错误", e);
            }
        }
    }
}
