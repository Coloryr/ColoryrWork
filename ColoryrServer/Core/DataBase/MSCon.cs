﻿using ColoryrServer.FileSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace ColoryrServer.DataBase;

public class MSCon
{
    /// <summary>
    /// 连接状态
    /// </summary>
    private static Dictionary<int, bool> State = new();
    /// <summary>
    /// 连接池
    /// </summary>
    private static Dictionary<int, string> ConnectStr = new();
    private static ConcurrentDictionary<int, bool> Connecting = new();
    private static List<SQLConfig> Config;

    /// <summary>
    /// 连接测试
    /// </summary>
    /// <param name="item">连接池项目</param>
    /// <returns>是否测试成功</returns>
    private static bool Test(SqlConnection item)
    {
        try
        {
            item.Open();
            new SqlCommand("select TOP 1 id from test", item).ExecuteNonQuery();
            item.Close();
            return true;
        }
        catch (SqlException ex)
        {
            switch (ex.Number)
            {
                case 1146:
                case 208:
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
            string ConnectString = string.Format(config.Conn, config.IP, config.User, pass);
            State.Add(id, false);
            var Conn = new SqlConnection(ConnectString);
            if (Test(Conn))
            {
                ConnectStr.Add(id, ConnectString);
                State[id] = true;
                ServerMain.LogOut($"Ms数据库{id}已连接");
            }
            else
            {
                ServerMain.LogError($"Ms数据库{id}连接失败");
            }
            Connecting.TryRemove(id, out var v);
            return State[id];
        }
        return false;
    }

    /// <summary>
    /// MSCon初始化
    /// </summary>
    /// <returns>是否连接成功</returns>
    public static void Start()
    {
        ServerMain.LogOut($"正在连接Ms数据库");
        Config = ServerMain.Config.MSsql;
        for (int a = 0; a < Config.Count; a++)
        {
            var config = Config[a];
            if (!config.Enable)
                continue;
            var pass = Encoding.UTF8.GetString(Convert.FromBase64String(config.Password));
            string ConnectString = string.Format(config.Conn, config.IP, config.User, pass);
            State.Add(a, false);
            var Conn = new SqlConnection(ConnectString);
            if (Test(Conn))
            {
                ConnectStr.Add(a, ConnectString);
                State[a] = true;
                ServerMain.LogOut($"Ms数据库{a}已连接");
            }
            else
            {
                ServerMain.LogError($"Ms数据库{a}连接失败");
            }
        }
    }

    /// <summary>
    /// 关闭Ms数据库连接
    /// </summary>
    public static void Stop()
    {
        foreach (var item in State)
        {
            State[item.Key] = false;
        }
        SqlConnection.ClearAllPools();
        ServerMain.LogOut("Ms数据库已断开");
    }

    /// <summary>
    /// 获取数据库链接
    /// </summary>
    /// <param name="ID">数据库ID</param>
    /// <returns>链接</returns>
    public static SqlConnection GetConnection(int id)
    {
        return new SqlConnection(ConnectStr[id]);
    }
}

