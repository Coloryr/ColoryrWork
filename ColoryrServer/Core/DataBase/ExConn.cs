﻿using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using StackExchange.Redis;
using System.Data.SqlClient;

namespace ColoryrServer.DataBase
{
    /// <summary>
    /// 状态
    /// </summary>
    public enum ConnState
    {
        Ok, Error, Restart, Open, Close, Stop
    }
    /// <summary>
    /// 存储类型
    /// </summary>
    public enum ConnType
    {
        Mysql, Ms, Oracle, Redis
    }
    /// <summary>
    /// 数据库连接存储
    /// </summary>
    public record ExConn
    {
        public int id;
        public int Index;
        public ConnState State;
        public ConnType Type;
        public MySqlConnection Mysql;
        public SqlConnection Ms;
        public OracleConnection Oracle;
        public ConnectionMultiplexer Redis;
    }
}