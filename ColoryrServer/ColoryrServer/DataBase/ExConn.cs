using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using StackExchange.Redis;
using System.Data.SqlClient;

namespace ColoryrServer.DataBase
{
    public enum SelfState
    {
        Ok, Error, Restart, Open, Close
    }
    public enum ConnType
    {
        Mysql, Ms, Oracle, Redis
    }
    class ExConn
    {
        public int Index;
        public SelfState State;
        public ConnType Type;
        public MySqlConnection Mysql;
        public SqlConnection Ms;
        public OracleConnection Oracle;
        public ConnectionMultiplexer Redis;
    }
}
