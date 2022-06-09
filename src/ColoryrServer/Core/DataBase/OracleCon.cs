using ColoryrServer.Core.FileSystem;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.Core.DataBase;

internal static class OracleCon
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
    private static bool Test(string conn)
    {
        try
        {
            new OracleConnection(conn).Execute("select id from test where rownum<=1");
            return true;
        }
        catch (OracleException ex)
        {
            switch (ex.Number)
            {
                case 942:
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
            if (Test(ConnectString))
            {
                ConnectStr.Add(id, ConnectString);
                State[id] = true;
                ServerMain.LogOut($"Oracle数据库{id}已连接");
            }
            else
            {
                ServerMain.LogError($"Oracle数据库{id}连接失败");
            }
            Connecting.TryRemove(id, out var v);
            return State[id];
        }
        return false;
    }

    /// <summary>
    /// 关闭Oracle数据库连接
    /// </summary>
    private static void Stop()
    {
        foreach (var item in State)
        {
            State[item.Key] = false;
        }
        OracleConnection.ClearAllPools();
        ServerMain.LogOut("Oracle数据库已断开");
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
            if (Test(ConnectString))
            {
                ConnectStr.Add(a, ConnectString);
                State[a] = true;
                ServerMain.LogOut($"Oracle数据库{a}已连接");
            }
            else
            {
                ServerMain.LogError($"Oracle数据库{a}连接失败");
            }
        }
        ServerMain.OnStop += Stop;
    }

    /// <summary>
    /// 获取数据库链接
    /// </summary>
    /// <param name="ID">数据库ID</param>
    /// <returns>链接</returns>
    public static OracleConnection GetConnection(int id)
    {
        return new OracleConnection(ConnectStr[id]);
    }
}
