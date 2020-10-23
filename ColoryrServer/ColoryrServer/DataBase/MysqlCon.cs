using ColoryrSDK;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace ColoryrServer.DataBase
{
    class MysqlCon
    {
        public static bool State { get; private set; }
        private static object LockObj = new object();
        private static MySqlConnection[] Conns;
        private static int LastIndex = 0;
        public static MySqlConnection GetConn()
        {

            lock (LockObj)
            {
                MySqlConnection Item;
                while (true)
                {
                    Item = Conns[LastIndex];
                    LastIndex++;
                    if (LastIndex >= ServerMain.Config.Mysql.ConnCount)
                        LastIndex = 0;
                    if (Item.State == ConnectionState.Closed)
                    {
                        Item.Open();
                        return Item;
                    }
                    Thread.Sleep(10);
                }
            }
        }
        /// <summary>
        /// Mysql初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public static dynamic Start()
        {
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(ServerMain.Config.Mysql.Password));
            string ConnectString = string.Format("SslMode=none;Server={0};Port={1};User ID={2};Password={3};Charset=utf8;",
                ServerMain.Config.Mysql.IP, ServerMain.Config.Mysql.Port, ServerMain.Config.Mysql.User, pass);
            Conns = new MySqlConnection[ServerMain.Config.Mysql.ConnCount];
            for (int a = 0; a < ServerMain.Config.Mysql.ConnCount; a++)
            {
                var Conn = new MySqlConnection(ConnectString);
                try
                {
                    Conn.Open();
                    var cmd = new MySqlCommand("select * from test", Conn);
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                    Conns[a] = Conn;
                }
                catch (MySqlException ex)
                {
                    switch (ex.Number)
                    {
                        case 1146:
                            Conn.Close();
                            Conns[a] = Conn;
                            break;
                        case 1046:
                            Conn.Close();
                            Conns[a] = Conn;
                            break;
                        default:
                            State = false;
                            ServerMain.LogError(ex);
                            return false;
                    }
                }
            }
            State = true;
            return true;
        }

        public static void Stop()
        {
            if (State)
                foreach (var Item in Conns)
                {
                    Item.Close();
                    Item.Dispose();
                }
            ServerMain.LogOut("Mysql已关闭");
        }

        /// <summary>
        /// 执行mysql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static List<List<dynamic>> MysqlSql(MySqlCommand Sql, string Database)
        {
            try
            {
                Sql.Connection = GetConn();
                Sql.Connection.ChangeDatabase(Database);
                MySqlDataReader reader = Sql.ExecuteReader();
                var List = new List<List<dynamic>>();
                while (reader.Read())
                {
                    var Item = new List<dynamic>();
                    for (int b = 0; b < reader.FieldCount; b++)
                        Item.Add(reader[b]);
                    List.Add(Item);
                }
                reader.Close();
                Sql.Connection.Close();
                return List;
            }
            catch (MySqlException e)
            {
                ServerMain.LogError(e);
                throw new VarDump(e);
            }
        }
    }
}
