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
    internal class MysqlCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static bool State { get; private set; }

        /// <summary>
        /// 连接池
        /// </summary>
        private static ExConn[] Conns;
        private static readonly object LockObj = new object();
        private static int LastIndex = 0;
        private static MysqlConfig Config;

        /// <summary>
        /// 获取连接Task
        /// </summary>
        public static Task<ExConn> GetConn = new(() =>
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
                    try
                    {
                        item.Mysql.Open();
                        item.State = ConnState.Open;
                        return item;
                    }
                    catch (MySqlException e)
                    {
                        ServerMain.LogError(e);
                        ConnReset(item);
                    }
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
                Config = ServerMain.Config.Mysql;
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(Config.Password));
                string ConnectString = string.Format(Config.Conn, Config.IP, Config.Port, Config.User, pass);
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

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private static bool Test(ExConn item)
        {
            try
            {
                item.Mysql.Open();
                new MySqlCommand("select * from test", item.Mysql).ExecuteNonQuery();
                item.Mysql.Close();
                item.State = ConnState.Ok;
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1146:
                    case 1046:
                        item.Mysql.Close();
                        item.State = ConnState.Ok;
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
        public static dynamic Start()
        {
            Config = ServerMain.Config.Mysql;
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(Config.Password));
            string ConnectString = string.Format(Config.Conn, Config.IP, Config.Port, Config.User, pass);
            Conns = new ExConn[Config.ConnCount];
            for (int a = 0; a < Config.ConnCount; a++)
            {
                var Conn = new MySqlConnection(ConnectString);
                var item = new ExConn
                {
                    State = ConnState.Error,
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

        /// <summary>
        /// 关闭Mysql数据库连接
        /// </summary>
        public static void Stop()
        {
            if (State)
            {
                State = false;
                foreach (var item in Conns)
                {
                    item.State = ConnState.Close;
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
                var task = GetConn;
                if (Task.WhenAny(task, Task.Delay(Config.TimeOut)).Result == task)
                {
                    var conn = task.Result;
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
                    conn.State = ConnState.Ok;
                    return readlist;
                }
                else
                {
                    ConnReset(task.Result);
                    throw new VarDump("Mysql数据库超时");
                }
            }
            catch (MySqlException e)
            {
                throw new VarDump(e);
            }
        }
    }
}
