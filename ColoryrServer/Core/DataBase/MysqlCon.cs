using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.DataBase
{
    public class MysqlCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static Dictionary<int, bool> State = new();

        /// <summary>
        /// 连接池
        /// </summary>
        private static List<MysqlConfig> Config;
        private static Dictionary<int, string> ConnectStr = new();

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private static bool Test(MySqlConnection item)
        {
            try
            {
                item.Open();
                new MySqlCommand("select id from test limit 1", item).ExecuteNonQuery();
                item.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1146:
                    case 1046:
                        item.Close();
                        return true;
                    default:
                        ServerMain.LogError(ex);
                        return false;
                }
            }
        }

        /// <summary>
        /// Mysql初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public static void Start()
        {
            ServerMain.LogOut($"正在连接Mysql数据库");
            Config = ServerMain.Config.Mysql;
            for (int a = 0; a < Config.Count; a++)
            {
                var config = Config[a];
                if (!config.Enable)
                    continue;
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
                string ConnectString = string.Format(config.Conn, config.IP, config.Port, config.User, pass);
                State.Add(a, false);
                var Conn = new MySqlConnection(ConnectString);
                var isok = Test(Conn);
                if (!isok)
                {
                    ServerMain.LogError($"Mysql数据库{a}连接失败");
                    break;
                }
                else
                {
                    ConnectStr.Add(a, ConnectString);
                    State[a] = true;
                    ServerMain.LogOut($"Mysql数据库{a}已连接");
                }
            }
        }

        /// <summary>
        /// 关闭Mysql数据库连接
        /// </summary>
        public static void Stop()
        {
            foreach (var item in State)
            {
                State[item.Key] = false;
            }
            MySqlConnection.ClearAllPools();
            ServerMain.LogOut("Mysql数据库已断开");
        }

        /// <summary>
        /// 执行mysql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static SqlRes MysqlSqlRes(MySqlCommand Sql, string Database, int id)
        {
            try
            {
                Sql.Connection = new MySqlConnection(ConnectStr[id]);
                Sql.Connection.ChangeDatabase(Database);
                MySqlDataReader reader = Sql.ExecuteReader();
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
            catch (MySqlException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
        }

        /// <summary>
        /// 执行mysql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static List<List<dynamic>> MysqlSql(MySqlCommand Sql, string Database, int id)
        {
            try
            {
                Sql.Connection = new MySqlConnection(ConnectStr[id]);
                Sql.Connection.ChangeDatabase(Database);
                MySqlDataReader reader = Sql.ExecuteReader();
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
            catch (MySqlException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
        }
    }
}
