using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.DataBase
{
    internal class MSCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static bool State { get; private set; }

        /// <summary>
        /// 连接池
        /// </summary>
        private static ExConn[] Conns;

        private static readonly object LockObj = new();
        private static int LastIndex = 0;
        private static MSsqlConfig Config;

        /// <summary>
        /// 获取连接Task
        /// </summary>
        private static ExConn GetConn()
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
                        item.Ms.Open();
                        item.State = ConnState.Open;
                        return item;
                    }
                    catch (SqlException e)
                    {
                        ServerMain.LogError(e);
                        ConnReset(item);
                    }
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
            Task.Run(() =>
            {
                item.State = ConnState.Restart;
                Config = ServerMain.Config.MSsql;
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(Config.Password));
                string ConnectString = string.Format(Config.Conn, Config.IP, Config.User, pass);
                var Conn = new SqlConnection(ConnectString);
                item.Ms.Dispose();
                item.Ms = Conn;
                if (Test(item))
                {
                    return;
                }
                else
                {
                    ServerMain.LogError($"Ms数据库重连失败，连接池第{item.Index}个连接");
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
                item.Ms.Open();
                new SqlCommand("select * from test", item.Ms).ExecuteNonQuery();
                item.Ms.Close();
                item.State = ConnState.Ok;
                return true;
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 1146:
                    case 208:
                        item.Ms.Close();
                        item.State = ConnState.Ok;
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
        public static bool Start()
        {
            Config = ServerMain.Config.MSsql;
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(Config.Password));
            string ConnectString = string.Format(Config.Conn, Config.IP, Config.User, pass);
            Conns = new ExConn[Config.ConnCount];
            for (int a = 0; a < Config.ConnCount; a++)
            {
                var Conn = new SqlConnection(ConnectString);
                var item = new ExConn
                {
                    State = ConnState.Error,
                    Type = ConnType.Ms,
                    Ms = Conn,
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
        /// 关闭Ms数据库连接
        /// </summary>
        public static void Stop()
        {
            if (State)
            {
                State = false;
                foreach (var a in Conns)
                {
                    a.State = ConnState.Close;
                    a.Ms.Dispose();
                }
                ServerMain.LogOut("Ms数据库已断开");
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static List<List<dynamic>> MSsqlSql(SqlCommand Sql, string Database)
        {
            try
            {
                ExConn conn = null;
                var task = Task.Run(()=>
                {
                    conn = GetConn();
                });
                if (Task.WhenAny(task, Task.Delay(Config.TimeOut)).Result == task)
                {
                    Sql.Connection = conn.Ms;
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
                    conn.State = ConnState.Ok;
                    return readlist;
                }
                else
                {
                    throw new VarDump("MS数据库超时");
                }
            }
            catch (SqlException e)
            {
                throw new VarDump(e);
            }
        }
    }
}

