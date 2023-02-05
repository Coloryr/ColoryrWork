using ColoryrServer.Core.Database;
using ColoryrServer.Core.DBConnection;
using Dapper;
using MySql.Data.MySqlClient;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ColoryrServer.SDK;

public interface IDatabase
{
    public IEnumerable<dynamic> Query(string sql, object arg);
    public IEnumerable<T> Query<T>(string sql, object arg);
    public int Execute(string sql, object arg);
}

public partial class Mysql : IDatabase
{
    private string Database;
    private int ID;
    /// <summary>
    /// Mysql数据库
    /// </summary>
    /// <param name="database">数据库名</param>
    /// <param name="id">数据库ID</param>
    public Mysql(string database = "", int id = 0)
    {
        ID = id;
        Database = database;
        if (!MysqlCon.Contains(id))
            throw new ErrorDump($"没有Mysql数据库{id}");
    }

    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<dynamic> Query(string sql, object arg)
    {
        var conn = MysqlCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Query(sql, arg);
    }
    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<T> Query<T>(string sql, object arg)
    {
        var conn = MysqlCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Query<T>(sql, arg);
    }

    /// <summary>
    /// 执行语句
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public int Execute(string sql, object arg)
    {
        var conn = MysqlCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Execute(sql, arg);
    }

    /// <summary>
    /// 获取一个数据库链接
    /// </summary>
    /// <returns>链接</returns>
    public MySqlConnection Get()
    {
        return MysqlCon.GetConnection(ID);
    }
}

public partial class MSsql : IDatabase
{
    private string Database;
    private int ID;
    /// <summary>
    /// MSsql数据库
    /// </summary>
    /// <param name="database">数据库名</param>
    /// <param name="id">数据库ID</param>
    public MSsql(string database = "", int id = 0)
    {
        Database = database;
        ID = id;
        if (!MSCon.Contains(id))
            throw new ErrorDump($"没有MS数据库{id}");
    }

    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<dynamic> Query(string sql, object arg)
    {
        var conn = MSCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Query(sql, arg);
    }
    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<T> Query<T>(string sql, object arg)
    {
        var conn = MSCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Query<T>(sql, arg);
    }

    /// <summary>
    /// 执行语句
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public int Execute(string sql, object arg)
    {
        var conn = MSCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Execute(sql, arg);
    }

    /// <summary>
    /// 获取一个数据库链接
    /// </summary>
    /// <returns>链接</returns>
    public SqlConnection Get()
    {
        return MSCon.GetConnection(ID);
    }
}

public partial class SqliteSql : IDatabase
{
    private string Database;
    private int ID;
    /// <summary>
    /// Sqlite数据库
    /// </summary>
    /// <param name="database">数据库名</param>
    /// <param name="id">数据库ID</param>
    public SqliteSql(string database = "", int id = 0)
    {
        ID = id;
        Database = database;
        if (!SqliteCon.Contains(id))
            throw new ErrorDump($"没有Sqlite数据库{id}");
    }

    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<dynamic> Query(string sql, object arg)
    {
        var conn = SqliteCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Query(sql, arg);
    }
    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<T> Query<T>(string sql, object arg)
    {
        var conn = SqliteCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Query<T>(sql, arg);
    }

    /// <summary>
    /// 执行语句
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public int Execute(string sql, object arg)
    {
        var conn = SqliteCon.GetConnection(ID);
        conn.ChangeDatabase(Database);
        return conn.Execute(sql, arg);
    }

    /// <summary>
    /// 获取一个数据库链接
    /// </summary>
    /// <returns>链接</returns>
    public MySqlConnection Get()
    {
        return MysqlCon.GetConnection(ID);
    }
}
public partial class OracleSql : IDatabase
{
    private string Database;
    private int ID;
    /// <summary>
    /// MSsql数据库
    /// </summary>
    /// <param name="database">数据库名</param>
    /// <param name="id">数据库ID</param>
    public OracleSql(string database = "", int id = 0)
    {
        ID = id;
        if (!Core.DBConnection.OracleCon.Contains(id))
            throw new ErrorDump($"没有Oracle数据库{id}");
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
        var conn = Get();
        conn.ChangeDatabase(Database);
        return conn.Query(sql, arg);
    }
    /// <summary>
    /// 执行查询
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public IEnumerable<T> Query<T>(string sql, object arg)
    {
        var conn = Get();
        conn.ChangeDatabase(Database);
        return conn.Query<T>(sql, arg);
    }

    /// <summary>
    /// 执行语句
    /// </summary>
    /// <param name="sql">sql语句</param>
    /// <param name="arg">参数</param>
    /// <returns>返回的数据</returns>
    public int Execute(string sql, object arg)
    {
        var conn = Get();
        conn.ChangeDatabase(Database);
        return conn.Execute(sql, arg);
    }

    /// <summary>
    /// 获取一个数据库链接
    /// </summary>
    /// <returns>链接</returns>
    public Oracle.ManagedDataAccess.Client.OracleConnection Get()
    {
        return Core.DBConnection.OracleCon.GetConnection(ID);
    }
}

public partial class Redis
{
    private int ID;
    /// <summary>
    /// Redis数据库
    /// </summary>
    /// <param name="id">数据库ID</param>
    public Redis(int id = 0)
    {
        ID = id;
        if (!RedisCon.State.ContainsKey(id))
            throw new ErrorDump($"没有Redis数据库{id}");
        if (RedisCon.State[id] == false)
            throw new ErrorDump("Redis没有链接");
    }
    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>值</returns>
    public RedisValue Get(string key)
        => RedisCon.Get(key, ID);

    /// <summary>
    /// 设置键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="Time">存在秒</param>
    public bool Set(string key, string value, int Time = 0)
        => RedisCon.Set(key, value, Time, ID);
    /// <summary>
    /// 是否存在键
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否存在</returns>
    public bool Exists(string key)
        => RedisCon.Exists(key, ID);
    /// <summary>
    /// 删除键值对
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否成功</returns>
    public bool Remove(string key)
        => RedisCon.Remove(key, ID);
    /// <summary>
    /// 累加
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>累加后的数据</returns>
    public long Increment(string key, long val = 1)
        => RedisCon.Increment(key, ID, val);
}

public partial class RamData
{
    public string Name { get; private set; }
    /// <summary>
    /// 启用缓存
    /// </summary>
    /// <param name="name">缓存名</param>
    public RamData(string name, bool save = false)
    {
        Name = RamDatabase.NewCache(name, save);
    }
    /// <summary>
    /// 获取值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>返回值</returns>
    public dynamic Get(string key)
        => RamDatabase.GetCache(Name, key);
    /// <summary>
    /// 设置键值
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public void Set(string key, dynamic value)
        => RamDatabase.SetCache(Name, key, value);
    /// <summary>
    /// 检查是否有键
    /// </summary>
    /// <param name="key">键名</param>
    /// <returns>是否存在</returns>
    public bool HaveKey(string key)
        => RamDatabase.HaveCacheKey(Name, key);
    /// <summary>
    /// 清理缓存
    /// </summary>
    public void Close()
        => RamDatabase.CloseCache(Name);
}
