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

        private static ExConn[] Conns;
        private readonly static object LockObj = new();
        private static int LastIndex = 0;
        public static ExConn GetConn()
        {
            ExConn Item;
            while (true)
            {
                lock (LockObj)
                {
                    Item = Conns[LastIndex];
                    LastIndex++;
                    if (LastIndex >= ServerMain.Config.MSsql.ConnCount)
                        LastIndex = 0;
                }
                if (Item.State ==  SelfState.Ok)
                {
                    try
                    {
                        Item.Oracle.Open();
                        Item.State = SelfState.Open;
                        return Item;
                    }
                    catch ()
                    { 
                        
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
