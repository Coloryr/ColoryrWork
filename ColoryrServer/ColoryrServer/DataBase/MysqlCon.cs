using ColoryrSDK;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.DataBase
{
    class MysqlCon
    {
        public static bool State { get; private set; }
        private static object LockObj = new object();
        private static ExConn[] Conns;
        private static int LastIndex = 0;
        public static ExConn GetConn()
        {
            ExConn item;
            while (true)
            {
                lock (LockObj)
                {
                    item = Conns[LastIndex];
                    LastIndex++;
                }
                if (LastIndex >= ServerMain.Config.Mysql.ConnCount)
                    LastIndex = 0;
                if (item.State == SelfState.Ok)
                {
                    try
                    {
                        item.Mysql.Open();
                        item.State = SelfState.Open;
                        return item;
                    }
                    catch (MySqlException e)
                    {
                        ServerMain.LogError(e);
                        ConnReset(item);
                    }
                }
                Thread.Sleep(10);
            }
        }

        public static void ConnReset(ExConn item)
        {
            Task.Run(() =>
            {
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(ServerMain.Config.Mysql.Password));
                string ConnectString = string.Format("SslMode=none;Server={0};Port={1};User ID={2};Password={3};Charset=utf8;",
                    ServerMain.Config.Mysql.IP, ServerMain.Config.Mysql.Port, ServerMain.Config.Mysql.User, pass);
                var Conn = new MySqlConnection(ConnectString);
                item.Mysql.Dispose();
                item.Mysql = Conn;
                if (Test(item))
                {
                    return;
                }
                else
                {
                    ServerMain.LogError($"Mysql数据库连接失败，连接池第{item.Index}个连接");
                }
            });
        }

        private static bool Test(ExConn item)
        {
            try
            {
                item.Mysql.Open();
                new MySqlCommand("select * from test", item.Mysql).ExecuteNonQuery();
                item.Mysql.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1146:
                    case 1046:
                        item.Mysql.Close();
                        item.State = SelfState.Ok;
                        return true;
                    default:
                        State = false;
                        ServerMain.LogError(ex);
                        return false;
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
            Conns = new ExConn[ServerMain.Config.Mysql.ConnCount];
            for (int a = 0; a < ServerMain.Config.Mysql.ConnCount; a++)
            {
                var Conn = new MySqlConnection(ConnectString);
                var item = new ExConn
                {
                    State = SelfState.Ok,
                    Type = ConnType.Mysql,
                    Mysql = Conn,
                    Index = a
                };
                if (Test(item))
                {
                    Conns[a] = item;
                }
                else
                {
                    State = false;
                    return false;
                }
            }
            State = true;
            return true;
        }

        public static void Stop()
        {
            if (State)
            {
                State = false;
                foreach (var item in Conns)
                {
                    item.State = SelfState.Close;
                    item.Mysql.Dispose();
                }
            }
            ServerMain.LogOut("Mysql已断开");
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
                var conn = GetConn();
                Sql.Connection = conn.Mysql;
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
                conn.State = SelfState.Ok;
                return readlist;
            }
            catch (MySqlException e)
            {
                throw new VarDump(e);
            }
        }
    }
}
