﻿using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.DataBase
{
    public class OracleCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        private static Dictionary<int, bool> State = new();
        /// <summary>
        /// 连接池
        /// </summary>
        private static List<SQLConfig> Config;
        private static ConcurrentDictionary<int, bool> Connecting = new();
        private static Dictionary<int, string> ConnectStr = new();

        /// <summary>
        /// 连接测试
        /// </summary>
        /// <param name="item">连接池项目</param>
        /// <returns>是否测试成功</returns>
        private static bool Test(OracleConnection item)
        {
            try
            {
                item.Open();
                new OracleCommand("select id from test where rownum<=1", item).ExecuteNonQuery();
                item.Close();
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 942:
                        item.Close();
                        return true;
                    default:
                        ServerMain.LogError(ex);
                        return false;
                }
            }
        }

        /// <summary>
        /// 尝试重连数据库
        /// </summary>
        /// <param name="id">数据库ID</param>
        /// <returns>是否能连接</returns>
        internal static bool Contains(int id)
        {
            if (State.ContainsKey(id) && State[id])
                return true;
            else if (Config.Count - 1 >= id)
            {
                if (Connecting.ContainsKey(id))
                    return false;
                if (!Connecting.TryAdd(id, true))
                    return false;
                var config = Config[id];
                if (!config.Enable)
                    return false;
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
                string ConnectString = string.Format(config.Conn, config.IP, config.Port, config.User, pass);
                State.Add(id, false);
                var Conn = new OracleConnection(ConnectString);
                if (Test(Conn))
                {
                    ConnectStr.Add(id, ConnectString);
                    State[id] = true;
                    ServerMain.LogOut($"Mysql数据库{id}已连接");
                }
                else
                {
                    ServerMain.LogError($"Mysql数据库{id}连接失败");
                }
                Connecting.TryRemove(id, out var v);
                return State[id];
            }
            return false;
        }

        /// <summary>
        /// Oracle初始化
        /// </summary>
        /// <returns>是否连接成功</returns>
        public static void Start()
        {
            ServerMain.LogOut($"正在连接Oracle数据库");
            Config = ServerMain.Config.Oracle;
            for (int a = 0; a < Config.Count; a++)
            {
                var config = Config[a];
                if (!config.Enable)
                    continue;
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
                string ConnectString = string.Format(config.Conn, config.IP, config.Port, config.User, pass);
                State.Add(a, false);
                var Conn = new OracleConnection(ConnectString);
                if (Test(Conn))
                {
                    ConnectStr.Add(a, ConnectString);
                    State[a] = true;
                    ServerMain.LogOut($"Mysql数据库{a}已连接");
                }
                else
                {
                    ServerMain.LogError($"Mysql数据库{a}连接失败");
                }
            }
        }

        /// <summary>
        /// 关闭Oracle数据库连接
        /// </summary>
        public static void Stop()
        {
            foreach (var item in State)
            {
                State[item.Key] = false;
            }
            OracleConnection.ClearAllPools();
            ServerMain.LogOut("Oracle数据库已断开");
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static SqlRes OracleSqlRes(OracleCommand Sql, string Database, int id)
        {
            try
            {
                Sql.Connection = new OracleConnection(ConnectStr[id]);
                Sql.Connection.Open();
                Sql.Connection.ChangeDatabase(Database);
                OracleDataReader reader = Sql.ExecuteReader();
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
            catch (OracleException e)
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
        public static List<List<dynamic>> OracleSql(OracleCommand Sql, string Database, int id)
        {
            try
            {
                Sql.Connection = new OracleConnection(ConnectStr[id]);
                Sql.Connection.Open();
                Sql.Connection.ChangeDatabase(Database);
                OracleDataReader reader = Sql.ExecuteReader();
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
            catch (OracleException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
        }
    }
}
