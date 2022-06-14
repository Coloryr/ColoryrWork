using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ColoryrServer.Core.FileSystem.Html;

public class WebFileManager
{
    private static readonly string CodeWebDB = ServerMain.RunLocal + "Codes/CodeWeb.db";
    private static readonly string CodeWebLogDB = ServerMain.RunLocal + "Codes/CodeWebLog.db";
    private static readonly string WebCodeLocal = ServerMain.RunLocal + "Codes/Static/";
    private static readonly string WebRemoveLocal = ServerMain.RunLocal + "Removes/Static/";
    private static readonly string WebNodeJS = WebCodeLocal + "node_modules/";

    private static string CodeWebConnStr;
    private static string CodeWebLogConnStr;
    public static ConcurrentDictionary<string, WebObj> HtmlCodeList { get; } = new();

    private class QWebObj : CSFileObj
    {
        public bool IsVue { get; set; }
        public string Codes { get; set; }
        public string Files { get; set; }

        public WebObj ToWeb()
        {
            return new()
            {
                UUID = UUID,
                Text = Text,
                Version = Version,
                CreateTime = CreateTime,
                UpdateTime = UpdateTime,
                IsVue = IsVue,
                Codes = new(),
                Files = new()
            };
        }
    }

    public static WebObj GetHtml(string uuid)
    {
        if (!HtmlCodeList.TryGetValue(uuid, out var item))
        {
            return null;
        }
        return item;
    }

    public static byte[] ReadFile(string uuid, string name)
    {
        string local = WebCodeLocal + uuid + "/" + name;
        if (File.Exists(local))
            return File.ReadAllBytes(local);
        return null;
    }

    public static void Start()
    {
        if (!Directory.Exists(WebCodeLocal))
            Directory.CreateDirectory(WebCodeLocal);
        if (!Directory.Exists(WebRemoveLocal))
            Directory.CreateDirectory(WebRemoveLocal);
        if (!Directory.Exists(WebNodeJS))
            Directory.CreateDirectory(WebNodeJS);

        CodeWebConnStr = new SqliteConnectionStringBuilder("Data Source=" + CodeWebDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        CodeWebLogConnStr = new SqliteConnectionStringBuilder("Data Source=" + CodeWebLogDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        using var CodeSQL = new SqliteConnection(CodeWebConnStr);
        using var CodeLogSQL = new SqliteConnection(CodeWebLogConnStr);

        CodeSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Version` integer,
  `IsVue` integer,
  `Codes` text,
  `Files` text,
  `CreateTime` text,
  `UpdateTime` text
);");
        CodeLogSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `File` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);");

        var list = CodeSQL.Query<QWebObj>("SELECT UUID,Text,Version,CreateTime,UpdateTime,IsVue,Codes,Files FROM web");

        foreach (var item in list)
        {
            try
            {
                var obj = item.ToWeb();

                string dir = WebCodeLocal + item.UUID + "/";

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var info = new DirectoryInfo(dir);

                if (item.Codes == null)
                    item.Codes = "[]";
                var list1 = JsonConvert.DeserializeObject<List<string>>(item.Codes);
                foreach (var item1 in list1)
                {
                    string local = dir + item1;
                    if (File.Exists(local))
                    {
                        obj.Codes.Add(item1, File.ReadAllText(local));
                    }
                }

                if (item.Files == null)
                    item.Files = "[]";
                list1 = JsonConvert.DeserializeObject<List<string>>(item.Files);
                if (obj.IsVue)
                {
                    foreach (var item1 in list1)
                    {
                        string local = dir + item1;
                        if (File.Exists(local))
                        {
                            obj.Files.Add(item1, null);
                        }
                    }
                }
                else
                {
                    WebBinManager.LoadWeb(obj, list1);
                }

                HtmlCodeList.TryAdd(item.UUID, obj);
                Storage(obj);

                ServerMain.LogOut($"加载Web：{item.UUID}");
            }
            catch (Exception e)
            {
                ServerMain.LogOut($"加载Web：{item.UUID}错误");
                ServerMain.LogError(e);
            }
        }
    }
    public static void SetIsVue(WebObj obj, bool IsVue)
    {
        obj.IsVue = IsVue;
        Storage(obj);
    }
    public static void DeleteAll(WebObj obj)
    {
        string time = string.Format("{0:s}", DateTime.Now).Replace(":", ".");
        string dir = WebRemoveLocal + $"{obj.UUID}-{time}" + "/";
        Directory.CreateDirectory(dir);
        string info =
$@"UUID:{obj.UUID},
Text:{obj.Text},
Version:{obj.Version}
";
        File.WriteAllText(dir + "info.txt", info);
        File.WriteAllText(dir + obj.UUID + ".json", JsonConvert.SerializeObject(obj));

        HtmlCodeList.TryRemove(obj.UUID, out var temp1);

        WebBinManager.DeleteAll(obj, dir);

        ServerMain.LogOut($"Web[{obj.UUID}]删除");
    }
    public static void SaveVue(WebObj obj, string name, string code, bool isCode = true, byte[] file = null)
    {
        if (isCode)
        {
            string old = obj.Codes[name];
            obj.Codes[name] = code;
            StorageCode(obj, name, old, code, true, null);
        }
        else
        {
            StorageCode(obj, name, null, code, false, file);
        }

        obj.Up();
        Storage(obj);
    }
    public static void Save(WebObj obj, string name, string code, string old)
    {
        obj.Codes[name] = code;
        StorageCode(obj, name, old, code, true, null);
        obj.Up();
        Storage(obj);
        WebBinManager.Save(obj, name);
    }
    public static void SaveFile(WebObj obj, string name, byte[] data)
    {
        WebBinManager.StorageWeb(obj, name, null, false, data);
        obj.Up();
        Storage(obj);
    }


    public static void AddFile(WebObj obj, string Name, byte[] data)
    {
        HtmlCodeList[obj.UUID].Files.Add(Name, null);

        obj.Up();
        Storage(obj);
    }

    private static void StorageCode(WebObj obj, string name, string old, string code, bool isCode, byte[] file = null)
    {
        try
        {
            string local = WebCodeLocal + obj.UUID + "/" + name;
            FileInfo info = new(local);
            if (!Directory.Exists(info.DirectoryName))
            {
                Directory.CreateDirectory(info.DirectoryName);
            }
            if (old != null)
            {
                using var CodeLogSQL = new SqliteConnection(CodeWebLogConnStr);
                CodeLogSQL.Execute($"INSERT INTO web (UUID,Text,Version,CreateTime,UpdateTime,File,Code) VALUES(@UUID,@Text,@Version,@CreateTime,@UpdateTime,@File,@Code)", new
                {
                    obj.UUID,
                    obj.Text,
                    obj.Version,
                    obj.CreateTime,
                    obj.UpdateTime,
                    File = name,
                    Code = old
                });
            }

            if (isCode)
            {
                if (!obj.Codes.ContainsKey(name))
                {
                    obj.Codes.Add(name, code);
                }
                else
                {
                    obj.Codes[name] = code;
                }
                File.WriteAllText(local, code);
            }
            else
            {
                File.WriteAllBytes(local, file);
            }
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }

    private static void Storage(WebObj obj)
    {
        Task.Run(() =>
        {
            try
            {
                using var CodeSQL = new SqliteConnection(CodeWebConnStr);
                var list = CodeSQL.Query<QWebObj>($"SELECT UUID,Text,Version,CreateTime,UpdateTime,Files,Codes,IsVue FROM web WHERE UUID=@UUID",
                    new { obj.UUID });

                var obj1 = new
                {
                    obj.UUID,
                    obj.Text,
                    obj.Version,
                    obj.CreateTime,
                    obj.UpdateTime,
                    obj.IsVue,
                    Codes = JsonConvert.SerializeObject(obj.Codes.Keys),
                    Files = JsonConvert.SerializeObject(obj.Files)
                };

                if (list.Any())
                {
                    CodeSQL.Execute($"UPDATE web SET Text=@Text,Version=@Version,CreateTime=@CreateTime,UpdateTime=@UpdateTime,Files=@Files,Codes=@Codes,IsVue=@IsVue WHERE UUID=@UUID", obj1);
                }
                else
                {
                    CodeSQL.Execute($"INSERT INTO web (UUID,Text,Version,CreateTime,UpdateTime,Files,Codes,IsVue) VALUES(@UUID,@Text,@Version,@CreateTime,@UpdateTime,@Files,@Codes,@IsVue)", obj1);
                }
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        });
    }

    public static void New(WebObj obj)
    {
        HtmlCodeList.TryAdd(obj.UUID, obj);
        foreach(var item in )
        StorageCode(obj, "index.html", "", obj.Codes["index.html"], true);
        StorageCode(obj, "js.js", "", obj.Codes["js.js"], true);
        obj.Up();
        Storage(obj);
    }

    public static void AddCode(WebObj obj, string name, string code)
    {
        obj.Codes.Add(name, code);
        Save(obj, name, code, null);
    }

    public static void Remove(WebObj obj, string name)
    {
        var temp1 = name.Split('.');
        if (temp1.Length != 1)
        {
            if (temp1[1] is "html" or "css" or "js" or "json" or "txt")
            {
                HtmlCodeList[obj.UUID].Codes.Remove(name);
            }
            else
                HtmlCodeList[obj.UUID].Files.Remove(name);
        }
        obj.Up();
        Storage(obj);
    }
}
