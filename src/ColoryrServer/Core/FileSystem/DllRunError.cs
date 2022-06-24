using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace ColoryrServer.Core.FileSystem;

internal static class DllRunError
{
    private static readonly string ErrorDB = ServerMain.RunLocal + "Error.db";

    private static string ErrorConnStr;

    public static void Start()
    {
        ErrorConnStr = new SqliteConnectionStringBuilder("Data Source=" + ErrorDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        using var ErrorSQL = new SqliteConnection(ErrorConnStr);

        string sql = @"create table if not exists error (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `dll` text,
  `text` text,
  `time` text
);";
        ErrorSQL.Execute(sql);
    }

    public static void PutError(string dll, string text)
    {
        DateTime time = DateTime.Now;
        string stime = time.ToString();
        Task.Run(() =>
        {
            using var errorSQL = new SqliteConnection(ErrorConnStr);
            errorSQL.Execute("INSERT INTO error (dll,text,time) VALUES(@dll,@text,@time)", new { dll, text, time });
        });
    }
}
