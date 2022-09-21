using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoryrServer.SDK;
using MySql.Data.MySqlClient;
using System.Data;
using ColoryrServer.Core.Http;
using System.Collections.Concurrent;
using ColoryrWork.Lib.Debug.Object;
using ColoryrServer.ServerDebug;

namespace ColoryrServer.SDK;

public static class StaticDatabase
{
    public static ConcurrentDictionary<long, Semaphore> locks = new();
    public static ConcurrentDictionary<long, DatabaseResObj> res = new();

    private static readonly Random random = new();
    private static readonly object obj1 = new();

    public static long Add()
    {
        long item;
        lock (obj1)
        {
            do
            {
                item = random.NextInt64();
            }
            while (locks.ContainsKey(item));
            locks.TryAdd(item, new Semaphore(0, 2));
            res.TryRemove(item, out _);
        }
        return item;
    }

    public static void Res(long id, DatabaseResObj obj)
    {
        if (!res.ContainsKey(id))
        {
            res.TryAdd(id, obj);
            locks[id].Release();
        }
    }

    public static void Remove(long id)
    {
        lock (obj1)
        {
            locks.TryRemove(id, out _);
            res.TryRemove(id, out _);
        }
    }
}

public class DebugMysql
{
    private string Database;
    private int ID;
    /// <summary>
    /// Mysql数据库
    /// </summary>
    /// <param name="database">数据库名</param>
    /// <param name="id">数据库ID</param>
    public DebugMysql(string database = "", int id = 0)
    {
        ID = id;
        Database = database;
    }

    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<dynamic> Query(string sql, object arg)
    {
        var obj =new DatabaseObj()
        {
            arg = arg,
            database = Database,
            id = ID,
            qid = StaticDatabase.Add(),
            read = 0,
            sql = sql,
            type = 0
        };
        PackWrite.SendDatabase(obj);

        StaticDatabase.locks[obj.qid].WaitOne();
        var res = StaticDatabase.res[obj.qid];
        StaticDatabase.Remove(obj.qid);
        if (res.ok)
        {
            return res.res;
        }
        else
        {
            throw new Exception(res.message);
        }
    }
    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<T> Query<T>(string sql, object arg)
    {
        var obj = new DatabaseObj()
        {
            arg = arg,
            database = Database,
            id = ID,
            qid = StaticDatabase.Add(),
            read = 0,
            sql = sql,
            type = 0
        };
        PackWrite.SendDatabase(obj);

        StaticDatabase.locks[obj.qid].WaitOne();
        var res = StaticDatabase.res[obj.qid];
        StaticDatabase.Remove(obj.qid);
        if (res.ok)
        {
            return null;
        }
        else
        {
            throw new Exception(res.message);
        }

        
    }

    /// <summary>
    /// 执行语句
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public int Execute(string sql, object arg)
    {
        return 0;
    }


}
