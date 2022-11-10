using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace ColoryrServer.Core.Database;

public static class LogDatabsae
{
    private static readonly string DB = ServerMain.RunLocal + "DllLog.db";

    private static string DBConnStr;

    public static void Start()
    {
        DBConnStr = new SqliteConnectionStringBuilder("Data Source=" + DB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        using var sql = new SqliteConnection(DBConnStr);

        sql.Execute(@"create table if not exists error (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `dll` text,
  `text` text,
  `time` text
);");
        sql.Execute(@"create table if not exists webbuild (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `name` text,
  `text` text,
  `time` text
);");
    }

    public static void PutError(string dll, string text)
    {
        Task.Run(() =>
        {
            using var sql = new SqliteConnection(DBConnStr);
            sql.Execute("INSERT INTO error (dll,text,time) VALUES(@dll,@text,@time)", new { dll, text, time = DateTime.Now.ToString() });
        });
    }

    public static void PutBuildLog(string name, string text)
    {
        Task.Run(() =>
        {
            using var sql = new SqliteConnection(DBConnStr);
            sql.Execute("INSERT INTO webbuild (name,text,time) VALUES(@name,@text,@time)", new { name, text, time = DateTime.Now.ToString() });
        });
    }
}
