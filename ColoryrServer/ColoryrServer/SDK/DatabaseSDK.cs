using ColoryrServer.DataBase;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
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
            if (!MysqlCon.State.ContainsKey(ID))
                throw new VarDump($"没有Mysql数据库{ID}");
            if (MysqlCon.State[ID] == false)
                throw new VarDump("Mysql未就绪");
            if (string.IsNullOrWhiteSpace(Database))
                throw new VarDump("没有选择数据库");
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>返回的数据</returns>
        public List<List<dynamic>> MysqlSql(string sql, Dictionary<string, string> arg)
        {
            var com = GenCommand(sql, arg);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            return MysqlCon.MysqlSql(com, Database, ID);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">Mysql参数</param>
        /// <returns>执行结果</returns>
        public List<List<dynamic>> MysqlSql(string sql, MySqlParameter[] arg)
        {
            var com = new MySqlCommand(sql);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            com.Parameters.AddRange(arg);
            return MysqlCon.MysqlSql(com, Database, ID);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="arg">Mysql命令语句</param>
        /// <returns>执行结果</returns>
        public List<List<dynamic>> MysqlSql(MySqlCommand arg)
            => MysqlCon.MysqlSql(arg, Database, ID);
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>Mysql命令语句</returns>
        public MySqlCommand MysqlCommand(string sql, Dictionary<string, string> arg)
        {
            var com = GenCommand(sql, arg);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            MysqlCon.MysqlSql(com, Database, ID);
            return com;
        }
        private MySqlCommand GenCommand(string sql, Dictionary<string, string> arg)
        {
            var com = new MySqlCommand(sql);
            if (arg != null)
                foreach (var item in arg)
                {
                    com.Parameters.Add(new MySqlParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                }
            return com;
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
            if (!MSCon.State.ContainsKey(ID))
                throw new VarDump($"没有Mysql数据库{ID}");
            if (MSCon.State[ID] == false)
                throw new VarDump("MS数据库没有链接");
            if (string.IsNullOrWhiteSpace(Database))
                throw new VarDump("没有选择数据库");

        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>返回的数据</returns>
        public List<List<dynamic>> MSsqlSql(string sql, Dictionary<string, string> arg)
        {
            var a = GenCommand(sql, arg);
            if (a == null)
                throw new VarDump("SQL语句参数非法");
            return MSCon.MSsqlSql(a, Database, ID);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">Mysql参数</param>
        /// <returns>执行结果</returns>
        public List<List<dynamic>> MSsqlSql(string sql, MySqlParameter[] arg)
        {
            var com = new SqlCommand(sql);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            com.Parameters.AddRange(arg);
            return MSCon.MSsqlSql(com, Database, ID);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>Mysql命令语句</returns>
        public SqlCommand MysqlCommand(string sql, Dictionary<string, string> arg)
        {
            var com = GenCommand(sql, arg);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            MSCon.MSsqlSql(com, Database, ID);
            return com;
        }
        private static SqlCommand GenCommand(string sql, Dictionary<string, string> arg)
        {
            var com = new SqlCommand(sql);
            if (arg != null)
                foreach (var item in arg)
                {
                    com.Parameters.Add(new SqlParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                }
            return com;
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
            if (!OracleCon.State.ContainsKey(ID))
                throw new VarDump($"没有Oracle数据库{ID}");
            if (OracleCon.State[ID] == false)
                throw new VarDump("Oracle没有链接");
            if (string.IsNullOrWhiteSpace(Database))
                throw new VarDump("没有选择数据库");
            this.Database = Database;
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>返回的数据</returns>
        public List<List<dynamic>> OracleSql(string sql, Dictionary<string, string> arg)
        {
            var a = GenCommand(sql, arg);
            if (a == null)
                throw new VarDump("SQL语句参数非法");
            return OracleCon.OracleSql(a, Database, ID);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">Mysql参数</param>
        /// <returns>执行结果</returns>
        public List<List<dynamic>> OracleSql(string sql, OracleParameter[] arg)
        {
            var com = new OracleCommand(sql);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            com.Parameters.AddRange(arg);
            return OracleCon.OracleSql(com, Database, ID);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>Mysql命令语句</returns>
        public OracleCommand OracleCommand(string sql, Dictionary<string, string> arg)
        {
            var com = GenCommand(sql, arg);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            OracleCon.OracleSql(com, Database, ID);
            return com;
        }
        private static OracleCommand GenCommand(string sql, Dictionary<string, string> arg)
        {
            var com = new OracleCommand(sql);
            if (arg != null)
                foreach (var item in arg)
                {
                    com.Parameters.Add(new OracleParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                }
            return com;
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
                throw new VarDump($"没有Redis数据库{ID}");
            if (RedisCon.State[ID] == false)
                throw new VarDump("Redis没有链接");
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public object Get(dynamic key)
            => RedisCon.Get(key, ID);

        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="Time">存在秒</param>
        public bool Set(dynamic key, dynamic value, int Time = 0)
            => RedisCon.Set(key, value, Time, ID);
        /// <summary>
        /// 是否存在键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public bool Exists(dynamic key)
            => RedisCon.Exists(key, ID);
        /// <summary>
        /// 删除键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功</returns>
        public bool Remove(dynamic key)
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
