using ColoryrServer.DataBase;
using Dapper;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ColoryrServer.SDK
{
    public class Mysql
    {
        private string Database;
        private int ID;
        /// <summary>
        /// Mysql数据库
        /// </summary>
        /// <param name="Database">数据库名</param>
        /// <param name="ID">数据库ID</param>
        public Mysql(string Database, int ID = 0)
        {
            this.ID = ID;
            this.Database = Database;
            if (!MysqlCon.Contains(ID))
                throw new ErrorDump($"没有Mysql数据库{ID}");
            if (string.IsNullOrWhiteSpace(Database))
                throw new ErrorDump("没有选择数据库");
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
        /// 旧版执行语句
        /// </summary>
        public List<List<dynamic>> MysqlSql(string sql, Dictionary<string, string> arg)
        {
            try
            {
                var conn = new MySqlCommand(sql, Get());
                if (arg != null)
                    foreach (var item in arg)
                    {
                        conn.Parameters.Add(new MySqlParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                    }
                conn.Connection.Open();
                conn.Connection.ChangeDatabase(Database);
                MySqlDataReader reader = conn.ExecuteReader();
                var readlist = new List<List<dynamic>>();
                while (reader.Read())
                {
                    var item = new List<dynamic>();

                    for (int b = 0; b < reader.FieldCount; b++)
                        item.Add(reader[b]);
                    readlist.Add(item);
                }
                reader.Close();
                conn.Connection.Close();
                return readlist;
            }
            catch (MySqlException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
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

    public class MSsql
    {
        private string Database;
        private int ID;
        /// <summary>
        /// MSsql数据库
        /// </summary>
        /// <param name="Database">数据库名</param>
        /// <param name="ID">数据库ID</param>
        public MSsql(string Database, int ID = 0)
        {
            this.Database = Database;
            this.ID = ID;
            if (!MSCon.Contains(ID))
                throw new ErrorDump($"没有MS数据库{ID}");
            if (string.IsNullOrWhiteSpace(Database))
                throw new ErrorDump("没有选择数据库");

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
        /// 旧版执行语句
        /// </summary>
        public List<List<dynamic>> MSsqlSql(string sql, Dictionary<string, string> arg)
        {
            try
            {
                var conn = new SqlCommand(sql, Get());
                if (arg != null)
                    foreach (var item in arg)
                    {
                        conn.Parameters.Add(new SqlParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                    }
                conn.Connection.Open();
                conn.Connection.ChangeDatabase(Database);
                SqlDataReader reader = conn.ExecuteReader();
                var readlist = new List<List<dynamic>>();
                while (reader.Read())
                {
                    var item = new List<dynamic>();

                    for (int b = 0; b < reader.FieldCount; b++)
                        item.Add(reader[b]);
                    readlist.Add(item);
                }
                reader.Close();
                conn.Connection.Close();
                return readlist;
            }
            catch (SqlException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
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

    public class Oracle
    {
        private string Database;
        private int ID;
        /// <summary>
        /// MSsql数据库
        /// </summary>
        /// <param name="Database">数据库名</param>
        /// <param name="ID">数据库ID</param>
        public Oracle(string Database = "", int ID = 0)
        {
            this.ID = ID;
            if (!OracleCon.Contains(ID))
                throw new ErrorDump($"没有Oracle数据库{ID}");
            if (string.IsNullOrWhiteSpace(Database))
                throw new ErrorDump("没有选择数据库");
            this.Database = Database;
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
        /// 旧版执行语句
        /// </summary>
        public List<List<dynamic>> OracleSql(string sql, Dictionary<string, string> arg)
        {
            try
            {
                var conn = new OracleCommand(sql, Get());
                if (arg != null)
                    foreach (var item in arg)
                    {
                        conn.Parameters.Add(new OracleParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                    }
                conn.Connection.Open();
                conn.Connection.ChangeDatabase(Database);
                OracleDataReader reader = conn.ExecuteReader();
                var readlist = new List<List<dynamic>>();
                while (reader.Read())
                {
                    var item = new List<dynamic>();

                    for (int b = 0; b < reader.FieldCount; b++)
                        item.Add(reader[b]);
                    readlist.Add(item);
                }
                reader.Close();
                conn.Connection.Close();
                return readlist;
            }
            catch (SqlException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
        }

        /// <summary>
        /// 获取一个数据库链接
        /// </summary>
        /// <returns>链接</returns>
        public OracleConnection Get()
        {
            return OracleCon.GetConnection(ID);
        }
    }

    public class Redis
    {
        private int ID;
        /// <summary>
        /// Redis数据库
        /// </summary>
        /// <param name="ID">数据库ID</param>
        public Redis(int ID = 0)
        {
            this.ID = ID;
            if (!RedisCon.State.ContainsKey(ID))
                throw new ErrorDump($"没有Redis数据库{ID}");
            if (RedisCon.State[ID] == false)
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

    public class RamData
    {
        public string Name { get; private set; }
        /// <summary>
        /// 启用缓存
        /// </summary>
        /// <param name="Name">缓存名</param>
        public RamData(string Name)
        {
            this.Name = RamDataBase.NewCache(Name);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回值</returns>
        public dynamic Get(string key)
            => RamDataBase.GetCache(Name, key);
        /// <summary>
        /// 设置键值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(string key, dynamic value)
            => RamDataBase.SetCache(Name, key, value);
        /// <summary>
        /// 检查是否有键
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>是否存在</returns>
        public bool HaveKey(string key)
            => RamDataBase.HaveCacheKey(Name, key);
        /// <summary>
        /// 清理缓存
        /// </summary>
        public void Close()
            => RamDataBase.CloseCache(Name);
    }
}
