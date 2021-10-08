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
    public class MysqlCon
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        public static Dictionary<int, bool> State = new();

        /// <summary>
        /// 连接池
        /// </summary>
        private static Dictionary<int, ExConn[]> Conns = new();
        private static Dictionary<int, object> LockObj = new();
        private static Dictionary<int, int> LastIndex = new();
        private static List<MysqlConfig> Config;

        /// <summary>
        /// 获取连接Task
        /// </summary>
        private static ExConn GetConn(int id)
        {
            ExConn item;
            while (true)
            {
                lock (LockObj[id])
                {
                    item = Conns[id][LastIndex[id]];
                    LastIndex[id]++;
                    if (LastIndex[id] >= Config[id].ConnCount)
                        LastIndex[id] = 0;
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
                        Task.Run(() => ConnReset(item));
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
            item.State = ConnState.Restart;
            Config = ServerMain.Config.Mysql;
            if (Config.Count <= item.id)
            {
                ServerMain.LogError($"Mysql配置出错，id:{item.id}");
                item.State = ConnState.Stop;
                return;
            }
            var config = Config[item.id];
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
            string ConnectString = string.Format(config.Conn, config.IP, config.Port, config.User, pass);
            var Conn = new MySqlConnection(ConnectString);
            item.Mysql.Dispose();
            item.Mysql = Conn;
            if (Test(item))
            {
                item.State = ConnState.Ok;
                return;
            }
            else
            {
                ServerMain.LogError($"Mysql数据库连接失败，连接池{item.id}第{item.Index}个连接");
            }
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
                new MySqlCommand("select id from test limit 1", item.Mysql).ExecuteNonQuery();
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
        public static void Start()
        {
            ServerMain.LogOut($"正在连接Mysql数据库");
            Config = ServerMain.Config.Mysql;
            for (int a = 0; a < Config.Count; a++)
            {
                var config = Config[a];
                if (!config.Enable)
                    continue;
                var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
                string ConnectString = string.Format(config.Conn, config.IP, config.Port, config.User, pass);
                var conn = new ExConn[config.ConnCount];
                State.Add(a, false);
                LockObj.Add(a, new());
                bool isok = false;
                for (int b = 0; b < config.ConnCount; b++)
                {
                    var Conn = new MySqlConnection(ConnectString);
                    var item = new ExConn
                    {
                        id = a,
                        State = ConnState.Close,
                        Type = ConnType.Mysql,
                        Mysql = Conn,
                        Index = b
                    };
                    if (Test(item))
                    {
                        item.State = ConnState.Ok;
                        conn[b] = item;
                    }
                    else
                    {
                        foreach (var item1 in conn)
                        {
                            if (item1 != null)
                                item1.Mysql?.Dispose();
                        }
                        ServerMain.LogError($"Mysql数据库{a}连接失败");
                        isok = false;
                        break;
                    }
                    isok = true;
                }
                if (!isok)
                {
                    continue;
                }
                State[a] = true;
                Conns.Add(a, conn);
                LastIndex.Add(a, 0);
                ServerMain.LogOut($"Mysql数据库{a}已连接");
            }
        }

        /// <summary>
        /// 关闭Mysql数据库连接
        /// </summary>
        public static void Stop()
        {
            foreach (var item in State)
            {
                State[item.Key] = false;
            }
            foreach (var item in Conns)
            {
                foreach (var item1 in item.Value)
                {
                    item1.State = ConnState.Close;
                    item1.Mysql.Dispose();
                }
            }
            ServerMain.LogOut("Mysql数据库已断开");
        }

        /// <summary>
        /// 执行mysql语句
        /// </summary>
        /// <param name="Database">数据库</param>
        /// <param name="Sql">SQL语句</param>
        /// <returns>结果集</returns>
        public static List<List<dynamic>> MysqlSql(MySqlCommand Sql, string Database, int id)
        {
            try
            {
                ExConn conn = null;
                CancellationTokenSource cancel = new();
                var task = Task.Run(() =>
                {
                    conn = GetConn(id);
                }, cancel.Token);
                if (Task.WhenAny(task, Task.Delay(Config[id].TimeOut)).Result == task)
                {
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
                    cancel.Cancel(false);
                    throw new ErrorDump("Mysql数据库超时");
                }
            }
            catch (MySqlException e)
            {
                throw new ErrorDump("执行sql语句出错", e);
            }
        }
    }
}
