using ColoryrServer.SDK;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColoryrServer.Core.FileSystem.Database;

internal static class LoginDatabase
{
    public record LoginObj
    {
        public string user { get; set; }
        public string uuid { get; set; }
        public DateTime time { get; set; }
    }

    public record UserObj
    {
        public string user { get; set; }
        public string password { get; set; }
        public DateTime time { get; set; }
    }
    private static readonly string DB = ServerMain.RunLocal + "Login.db";
    private static string connStr;
    public static void Start()
    {
        connStr = new SqliteConnectionStringBuilder("Data Source=" + DB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        using var sql = new SqliteConnection(connStr);
        sql.Execute(@"create table if not exists login (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `user` text,
  `uuid` text,
  `time` datetime
);");

        sql.Execute(@"create table if not exists user (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `user` text,
  `password` text,
  `time` datetime
);");
        var list = sql.Query("SELECT id FROM user");
        if (!list.Any())
        {
            sql.Execute("INSERT INTO user (user,password) VALUES(@user,@password)",
                new { user = "admin", password = EnCode.SHA1("Admin").ToLower() });
        }
    }
    /// <summary>
    /// 检查登录
    /// </summary>
    /// <param name="user">用户名</param>
    /// <param name="uuid">密钥</param>
    /// <returns>结果</returns>
    public static bool CheckLogin(string user, string uuid)
    {
        using var sql = new SqliteConnection(connStr);
        var list = sql.Query<LoginObj>("SELECT user,uuid,time FROM login WHERE user=@user", new { user });
        if (!list.Any())
            return false;
        LoginObj item = list.First();
        if (item.uuid != uuid)
            return false;
        if (DateTime.Now - item.time > TimeSpan.FromDays(7))
            return false;

        return true;
    }

    public static bool AddUser(string user, string password)
    {
        using var sql = new SqliteConnection(connStr);
        var list = sql.Query<UserObj>("SELECT id FROM user WHERE user=@user",
            new { user });
        if (list.Any())
            return false;

        sql.Execute("INSERT INTO user (user,password) VALUES(@user,@password)",
            new { user, password });
        return true;
    }

    public static bool Remove(string user)
    {
        using var sql = new SqliteConnection(connStr);
        var list = sql.Query<UserObj>("SELECT id FROM user WHERE user=@user",
            new { user });
        if (!list.Any())
            return false;
        sql.Execute("DELETE FROM user WHERE user = @user",
            new { user });
        sql.Execute("DELETE FROM login WHERE user = @user",
            new { user });
        return true;
    }

    public static bool CheckPassword(string user, string password)
    {
        using var sql = new SqliteConnection(connStr);
        var list = sql.Query<UserObj>("SELECT password FROM user WHERE user=@user", new { user });
        if (!list.Any())
            return false;
        var item = list.First();
        return item.password == password;
    }

    public static List<UserObj> GetAllUser()
    {
        using var sql = new SqliteConnection(connStr);
        var list = sql.Query<UserObj>("SELECT user,time FROM user");
        return list.ToList();
    }

    public static void UpdateToken(string user, string uuid)
    {
        using var sql = new SqliteConnection(connStr);
        var list = sql.Query("SELECT id FROM login WHERE user=@user", new { user });
        if (list.Any())
            sql.Execute("UPDATE login SET uuid=@uuid,time=@time WHERE user=@user", new { uuid, user, time = DateTime.Now });
        else
            sql.Execute("INSERT INTO login (uuid,user,time) VALUES(@uuid,@user,@time)", new { uuid, user, time = DateTime.Now });

        sql.Execute("UPDATE user SET time=@time WHERE user=@user", new { user, time = DateTime.Now });
    }
}
