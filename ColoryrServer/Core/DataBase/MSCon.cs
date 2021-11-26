using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ColoryrServer.DataBase
{
    public class MSCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static Dictionary<int, bool> State = new();

        /// <summary>
        /// 连接池
        /// </summary>
        private static Dictionary<int, string> ConnectStr = new();
        private static List<SQLConfig> Config;

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private static bool Test(SqlConnection item)
        {
            try
            {
                item.Open();
                new SqlCommand("select TOP 1 id from test", item).ExecuteNonQuery();
                item.Close();
                return true;
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 1146:
                    case 208:
                        item.Close();
                        return true;
                    default:
                        ServerMain.LogError(ex);
                        return false;
                }
            }
        }

        /// <summary>
        /// MSCon初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public static void Start()
        {
            ServerMain.LogOut($"正在连接Ms数据库");
            Config = ServerMain.Config.MSsql;
            for (int a = 0; a < Config.Count; a++)
            {
                var config = Config[a];
                if (!config.Enable)
                    continue;
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
                string ConnectString = string.Format(config.Conn, config.IP, config.User, pass);
                State.Add(a, false);
                var Conn = new SqlConnection(ConnectString);
                if (Test(Conn))
                {
                    ConnectStr.Add(a, ConnectString);
                    State[a] = true;
                    ServerMain.LogOut($"Ms数据库{a}已连接");
                }
                else
                {
                    ServerMain.LogError($"Ms数据库{a}连接失败");
                }
            }
        }

        /// <summary>
        /// 关闭Ms数据库连接
        /// </summary>
        public static void Stop()
        {
            foreach (var item in State)
            {
                State[item.Key] = false;
            }
            SqlConnection.ClearAllPools();
            ServerMain.LogOut("Ms数据库已断开");
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static SqlRes MSsqlSqlRes(SqlCommand Sql, string Database, int id)
        {
            try
            {
                Sql.Connection = new SqlConnection(ConnectStr[id]);
                Sql.Connection.ChangeDatabase(Database);
                SqlDataReader reader = Sql.ExecuteReader();
                var readlist = new List<List<dynamic>>();
                var readlist1 = new List<Dictionary<string, dynamic>>();
                while (reader.Read())
                {
                    var item = new List<dynamic>();
                    var item1 = new Dictionary<string, dynamic>();
                    var data = reader.GetSchemaTable();
                    for (int b = 0; b < reader.FieldCount; b++)
                    {
                        item1.Add(data.Rows[b][0] as string, reader[b]);
                        item.Add(reader[b]);
                    }
                    readlist1.Add(item1);
                    readlist.Add(item);
                }
                reader.Close();
                Sql.Connection.Close();
                return new SqlRes()
                {
                    data = readlist,
                    data1 = readlist1
                };
            }
            catch (SqlException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static List<List<dynamic>> MSsqlSql(SqlCommand Sql, string Database, int id)
        {
            try
            {
                Sql.Connection = new SqlConnection(ConnectStr[id]);
                Sql.Connection.ChangeDatabase(Database);
                SqlDataReader reader = Sql.ExecuteReader();
                var readlist = new List<List<dynamic>>();
                while (reader.Read())
                {
                    var item = new List<dynamic>();
                    for (int b = 0; b < reader.FieldCount; b++)
                        item.Add(reader[b]);
                    readlist.Add(item);
                }
                reader.Close();
                Sql.Connection.Close();
                return readlist;
            }
            catch (SqlException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
        }
    }
}

