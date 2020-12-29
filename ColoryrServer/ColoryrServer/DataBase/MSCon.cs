using ColoryrSDK;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.DataBase
{
    class MSCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static bool State { get; private set; }

        private static ExConn[] Conns;
        private static readonly object LockObj = new();
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
                    if (LastIndex >= ServerMain.Config.MSsql.ConnCount)
                        LastIndex = 0;
                }
                if (item.State ==  SelfState.Ok)
                {
                    try
                    {
                        item.Ms.Open();
                        item.State = SelfState.Open;
                        return item;
                    }
                    catch (SqlException e)
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
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(ServerMain.Config.MSsql.Password));
                string ConnectString = string.Format("Server={0};UID={1};PWD={2};",
                ServerMain.Config.MSsql.IP, ServerMain.Config.MSsql.User, pass);
                var Conn = new SqlConnection(ConnectString);
                item.Ms.Dispose();
                item.Ms = Conn;
                if (Test(item))
                {
                    return;
                }
                else
                {
                    ServerMain.LogError($"Ms数据库连接失败，连接池第{item.Index}个连接");
                }
            });
        }

        private static bool Test(ExConn item)
        {
            try
            {
                item.Ms.Open();
                new SqlCommand("select * from test", item.Ms).ExecuteNonQuery();
                item.Ms.Close();
                item.State = SelfState.Ok;
                return true;
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 1146:
                    case 208:
                        item.Ms.Close();
                        item.State = SelfState.Ok;
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
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(ServerMain.Config.MSsql.Password));
            string ConnectString = string.Format("Server={0};UID={1};PWD={2};",
                ServerMain.Config.MSsql.IP, ServerMain.Config.MSsql.User, pass);
            Conns = new ExConn[ServerMain.Config.Mysql.ConnCount];
            for (int a = 0; a < ServerMain.Config.Mysql.ConnCount; a++)
            {
                var Conn = new SqlConnection(ConnectString);
                var item = new ExConn
                {
                    State = SelfState.Ok,
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

        public static void Stop()
        {
            if (State)
            {
                State = false;
                foreach (var a in Conns)
                {
                    a.State = SelfState.Close;
                    a.Ms.Dispose();
                }
            }
            ServerMain.LogOut("Ms数据库已断开");
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
                var conn = GetConn();
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
                conn.State = SelfState.Ok;
                return readlist;
            }
            catch (SqlException e)
            {
                throw new VarDump(e);
            }
        }
    }
}

