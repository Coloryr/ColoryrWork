using ColoryrSDK;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace ColoryrServer.DataBase
{
    class OracleCon
    {
        public static bool State { get; private set; }
        private static OracleConnection[] Conns;
        private static object LockObj = new object();
        private static int LastIndex = 0;
        public static OracleConnection GetConn()
        {
            lock (LockObj)
            {
                OracleConnection Item;
                while (true)
                {
                    Item = Conns[LastIndex];
                    LastIndex++;
                    if (LastIndex >= ServerMain.Config.MSsql.ConnCount)
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
        public static bool Start()
        {
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(ServerMain.Config.MSsql.Password));
            string ConnectString = string.Format("Server={0};UID={1};PWD={2};",
                ServerMain.Config.MSsql.IP, ServerMain.Config.MSsql.User, pass);
            Conns = new OracleConnection[ServerMain.Config.Mysql.ConnCount];
            for (int a = 0; a < ServerMain.Config.Mysql.ConnCount; a++)
            {
                var Conn = new OracleConnection(ConnectString);
                try
                {
                    Conn.Open();
                    var cmd = new OracleCommand("select * from test", Conn);
                    cmd.ExecuteNonQuery();
                    Conn.Close();
                    Conns[a] = Conn;
                }
                catch (OracleException ex)
                {
                    switch (ex.Number)
                    {
                        case 1146:
                            Conn.Close();
                            Conns[a] = Conn;
                            break;
                        case 208:
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
                foreach (var a in Conns)
                {
                    a.Close();
                    a.Dispose();
                }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static List<List<dynamic>> OracleSql(OracleCommand Sql, string Database)
        {
            try
            {
                Sql.Connection = GetConn();
                Sql.Connection.ChangeDatabase(Database);
                OracleDataReader reader = Sql.ExecuteReader();
                var ReList = new List<List<dynamic>>();
                while (reader.Read())
                {
                    var Item = new List<dynamic>();
                    for (int b = 0; b < reader.FieldCount; b++)
                        Item.Add(reader[b]);
                    ReList.Add(Item);
                }
                reader.Close();
                Sql.Connection.Close();
                return ReList;
            }
            catch (OracleException e)
            {
                ServerMain.LogError(e);
                throw new VarDump(e);
            }
        }
    }
}
