using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace ColoryrServer.Core.FileSystem;

internal static class DllRunError
{
    private static readonly string ErrorDB = ServerMain.RunLocal + @"Error.db";

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
  `Dll` text,
  `Text` text,
  `Time` text
);";
        ErrorSQL.Execute(sql);
    }

    public static void PutError(string dll, string text)
    {
        DateTime time = DateTime.Now;
        string stime = time.ToString();
        Task.Run(() =>
        {
            using var ErrorSQL = new SqliteConnection(ErrorConnStr);
            ErrorSQL.Execute("INSERT INTO error (Dll,Text,Time) VALUES(@Dll,@Text,@Time)", new { Dll = dll, Text = text, Time = stime });
        });
    }
}
