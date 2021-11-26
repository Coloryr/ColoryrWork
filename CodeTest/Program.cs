using ColoryrServer.Core.FileSystem;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

Stopwatch stopwatch = new();
stopwatch.Start();
var Conn = new MySqlConnection("SslMode=none;Server=127.0.0.1;Port=3306;User ID=root;Password=123456;Charset=utf8;Pooling=false;");
Conn.Open();
stopwatch.Stop();
Console.WriteLine(stopwatch.Elapsed);

MySqlCommand Sql = new MySqlCommand("select * from citypipeerror");

Sql.Connection = Conn;
Sql.Connection.ChangeDatabase("citypipe");
MySqlDataReader reader = Sql.ExecuteReader();
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

