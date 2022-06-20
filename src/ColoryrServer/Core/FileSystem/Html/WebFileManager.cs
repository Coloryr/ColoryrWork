using ColoryrServer.Core.Http;
using ColoryrServer.Utils;
using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ColoryrServer.Core.FileSystem.Html;

public class WebFileManager
{
    /// <summary>
    /// Web项目储存
    /// </summary>
    private static readonly string WebDataDB = ServerMain.RunLocal + "Codes/WebData.db";
    /// <summary>
    /// Web代码储存
    /// </summary>
    private static readonly string WebCodeDB = ServerMain.RunLocal + "Codes/WebCode.db";
    /// <summary>
    /// Web代码日志储存
    /// </summary>
    private static readonly string WebLogDB = ServerMain.RunLocal + "Codes/WebLog.db";
    /// <summary>
    /// Web文件储存
    /// </summary>
    private static readonly string WebFileDB = ServerMain.RunLocal + "Codes/WebFile.db";
    /// <summary>
    /// 代码存放
    /// </summary>
    private static readonly string WebCodeLocal = ServerMain.RunLocal + "Codes/Static/";
    /// <summary>
    /// Web项目删除
    /// </summary>
    private static readonly string WebRemoveLocal = ServerMain.RunLocal + "Removes/Web/";
    /// <summary>
    /// Vue编译nodejs
    /// </summary>
    private static readonly string WebNodeJS = WebCodeLocal + "node_modules/";

    /// <summary>
    /// Web项目储存
    /// </summary>
    private static string WebDataConnStr;
    /// <summary>
    /// Web代码储存
    /// </summary>
    private static string WebCodeConnStr;
    /// <summary>
    /// Web代码日志储存
    /// </summary>
    private static string WebLogConnStr;
    /// <summary>
    /// Web文件储存
    /// </summary>
    private static string WebFileConnStr;

    /// <summary>
    /// Web项目储存
    /// </summary>
    public static ConcurrentDictionary<string, WebObj> HtmlCodeList { get; } = new();
    /// <summary>
    /// 软路由路径
    /// </summary>
    public static ConcurrentDictionary<string, WebObj> Rote { get; set; } = new();

    /// <summary>
    /// 获取Web项目
    /// </summary>
    /// <param name="uuid">UUID</param>
    /// <returns></returns>
    public static WebObj GetHtml(string uuid)
    {
        if (!HtmlCodeList.TryGetValue(uuid, out var item))
        {
            return null;
        }
        return item;
    }

    public static void Start()
    {
        if (!Directory.Exists(WebCodeLocal))
            Directory.CreateDirectory(WebCodeLocal);
        if (!Directory.Exists(WebRemoveLocal))
            Directory.CreateDirectory(WebRemoveLocal);
        if (!Directory.Exists(WebNodeJS))
            Directory.CreateDirectory(WebNodeJS);

        WebDataConnStr = new SqliteConnectionStringBuilder("Data Source=" + WebDataDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        WebCodeConnStr = new SqliteConnectionStringBuilder("Data Source=" + WebCodeDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        WebFileConnStr = new SqliteConnectionStringBuilder("Data Source=" + WebFileDB)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        using var dataSQL = new SqliteConnection(WebDataConnStr);
        using var codeSQL = new SqliteConnection(WebCodeConnStr);
        using var fileSQL = new SqliteConnection(WebFileConnStr);

        dataSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `version` integer,
  `vue` integer,
  `createtime` text,
  `updatetime` text
);");

        codeSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `code` text,
  `time` text
);");

        fileSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `data` blob,
  `time` text
);");

        //日志模式
        if (ServerMain.Config.CodeSetting.CodeLog)
        {
            WebLogConnStr = new SqliteConnectionStringBuilder("Data Source=" + WebLogDB)
            {
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();

            using var logSQL = new SqliteConnection(WebLogConnStr);
            //Web设定
            logSQL.Execute(@"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `text` text,
  `version` integer,
  `vue` integer,
  `time` text
);");

            logSQL.Execute(@"create table if not exists code (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `code` text,
  `time` text
);");

            logSQL.Execute(@"create table if not exists file (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `uuid` text,
  `name` text,
  `data` blob,
  `time` text
);");
        }

        var list = dataSQL.Query<QWebObj>("SELECT uuid,text,version,vue,createtime,updatetime FROM web");

        foreach (var item in list)
        {
            try
            {
                var obj = item.ToWeb();

                string dir = WebCodeLocal + item.uuid + "/";

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var info = new DirectoryInfo(dir);
                var list1 = codeSQL.Query<CWebObj>("SELECT name,code FROM web WHERE uuid=@uuid",
                    new { item.uuid });
                foreach (var item1 in list1)
                {
                    obj.Codes.Add(item1.name, item1.code);
                }
                var list2 = fileSQL.Query<FWebObj>("SELECT name,data FROM web WHERE uuid=@uuid", 
                    new { item.uuid });
                foreach (var item1 in list2) 
                {
                    obj.Files.Add(item1.name, item1.data);
                }

                HtmlCodeList.TryAdd(item.uuid, obj);
                Storage(obj);

                HttpInvokeRoute.AddWeb(item.uuid, obj);

                ServerMain.LogOut($"加载Web：{item.uuid}");
            }
            catch (Exception e)
            {
                ServerMain.LogOut($"加载Web：{item.uuid}错误");
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
        File.WriteAllText(dir + "info.txt", $@"UUID:{obj.UUID}
Text:{obj.Text}
IsVue:{obj.IsVue}
Version:{obj.Version}
CreateTime:{obj.CreateTime}
UpdateTime:{obj.UpdateTime}");
        foreach (var item in obj.Codes)
        {
            var info = new FileInfo(dir + item.Key);
            Directory.CreateDirectory(info.DirectoryName);
            File.WriteAllText(dir + item.Key, item.Value);
        }
        foreach (var item in obj.Files)
        {
            var info = new FileInfo(dir + item.Key);
            Directory.CreateDirectory(info.DirectoryName);
            File.WriteAllBytes(info.FullName, item.Value);
        }

        HtmlCodeList.TryRemove(obj.UUID, out var temp1);
        using var dataSQL = new SqliteConnection(WebDataConnStr);
        using var fileSQL = new SqliteConnection(WebFileConnStr);
        using var codeSQL = new SqliteConnection(WebCodeConnStr);
        var arg = new { uuid = obj.UUID };
        fileSQL.Execute("DELETE FROM web WHERE uuid=@uuid", arg);
        codeSQL.Execute("DELETE FROM web WHERE uuid=@uuid", arg);
        dataSQL.Execute("DELETE FROM web WHERE uuid=@uuid", arg);

        ServerMain.LogOut($"Web[{obj.UUID}]删除");
    }
    public static void AddItem(WebObj obj, string file, bool isCode, string code = null, byte[] data = null)
    {
        if (isCode)
        {
            HtmlCodeList[obj.UUID].Codes.Add(file, code);
            StorageCode(obj, file, code);
        }
        else
        {
            HtmlCodeList[obj.UUID].Files.Add(file, data);
            StorageFile(obj, file, data);
        }


        obj.Up();
        Storage(obj);
    }
    /// <summary>
    /// 保存代码文件
    /// </summary>
    /// <param name="obj">项目</param>
    /// <param name="name">文件名</param>
    /// <param name="code">内容</param>
    public static void StorageCode(WebObj obj, string name, string code)
    {
        try
        {
            string local = WebCodeLocal + obj.UUID + "/" + name;
            FileInfo info = new(local);
            if (!Directory.Exists(info.DirectoryName))
            {
                Directory.CreateDirectory(info.DirectoryName);
            }
            using var CodeSQL = new SqliteConnection(WebCodeConnStr);
            var list = CodeSQL.Query<CWebObj>("SELECT code FROM web WHERE uuid=@uuid AND name=@name", new { uuid = obj.UUID, name });
            string time = DateTime.Now.ToString();
            if (list.Any())
            {
                CodeSQL.Execute("UPDATE web SET code=@code,time=@time WHERE uuid=@uuid AND name=@name",
                    new { uuid = obj.UUID, name, code, time });
                if (ServerMain.Config.CodeSetting.CodeLog)
                {
                    Task.Run(() =>
                    {
                        using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                        CodeLogSQL.Execute($"INSERT INTO code (uuid,name,code,time) VALUES(@uuid,@name,@code,@time)", new
                        {
                            uuid = obj.UUID,
                            name,
                            list.First().code,
                            time
                        });
                    });
                }
            }
            else
            {
                CodeSQL.Execute("INSERT INTO web (uuid,name,code,time) VALUES(@uuid,@name,@code,@time)",
                    new { uuid = obj.UUID, name, code, time });
            }

            if (!obj.Codes.ContainsKey(name))
            {
                obj.Codes.Add(name, code);
            }
            else
            {
                obj.Codes[name] = code;
            }
            //更新bin
            Task.Run(() =>
            {
                if (obj.IsVue)
                {
                    File.WriteAllText(local, code);
                }
                else
                {
                    if (name.ToLower().EndsWith(".css") &&
                        ServerMain.Config.CodeSetting.MinifyCSS)
                    {
                        code = CodeCompress.CSS(code);
                        File.WriteAllText(local, code);
                    }

                    if (name.ToLower().EndsWith(".js") &&
                        ServerMain.Config.CodeSetting.MinifyJS)
                    {
                        code = CodeCompress.JS(code);
                        File.WriteAllText(local, code);
                    }

                    if (name.ToLower().EndsWith(".html") &&
                        ServerMain.Config.CodeSetting.MinifyHtml)
                    {
                        code = CodeCompress.JS(code);
                        File.WriteAllText(local, code);
                    }
                }
            });
        }
        catch (Exception e)
        {
            ServerMain.LogError(e);
        }
    }

    private static void StorageFile(WebObj obj, string name, byte[] data)
    {
        using var fileSQL = new SqliteConnection(WebFileConnStr);
        string time = DateTime.Now.ToString();
        var list = fileSQL.Query<FWebObj>("SELECT data FROM web WHERE uuid=@uuid AND name=@name", new { uuid=obj.UUID, name });
        if (list.Any())
        {
            fileSQL.Execute("UPDATE web SET data=@data,time=@time WHERE uuid=@uuid AND name=@name",
                new { uuid = obj.UUID, data, name, time });

            if (ServerMain.Config.CodeSetting.CodeLog)
            {
                using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                byte[] old = list.First().data;
                CodeLogSQL.Execute($"INSERT INTO file (uuid,name,data,time) VALUES(@uuid,@name,@data,@time)", new
                {
                    uuid = obj.UUID,
                    name,
                    data = old,
                    time
                });
            }
        }
        else
        {
            fileSQL.Execute("INSERT INTO web (uuid,name,data,time) VALUES(@uuid,@name,@data,@time)",
                new { uuid = obj.UUID, name, data, time });
        }
    }

    private static void Storage(WebObj obj)
    {
        Task.Run(() =>
        {
            try
            {
                using var codeSQL = new SqliteConnection(WebDataConnStr);
                var list = codeSQL.Query<QWebObj>($"SELECT text,version,vue,createtime,updatetime FROM web WHERE uuid=@uuid",
                    new { uuid = obj.UUID });

                var obj1 = new
                {
                    uuid = obj.UUID,
                    text = obj.Text,
                    version = obj.Version,
                    createtime = obj.CreateTime,
                    updatetime = obj.UpdateTime,
                    vue = obj.IsVue
                };

                if (list.Any())
                {
                    codeSQL.Execute($"UPDATE web SET text=@text,version=@version,createtime=@createtime,updatetime=@updatetime,vue=@vue WHERE uuid=@uuid", obj1);

                    if (ServerMain.Config.CodeSetting.CodeLog)
                    {
                        using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                        var old = list.First();
                        CodeLogSQL.Execute($"INSERT INTO web (uuid,text,version,vue,time) VALUES(@uuid,@text,@version,@vue,@time)",
                        new
                        {
                            uuid = obj.UUID,
                            text = obj.Text,
                            version = obj.Version,
                            time = DateTime.Now.ToString(),
                            vue = obj.IsVue
                        });
                    }
                }
                else
                {
                    codeSQL.Execute($"INSERT INTO web (uuid,text,version,vue,createtime,updatetime) VALUES(@uuid,@text,@version,@vue,@createtime,@updatetime)", obj1);
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
        foreach (var item in obj.Codes)
        {
            StorageCode(obj, item.Key, item.Value);
        }
        foreach (var item in obj.Files)
        {
            StorageFile(obj, item.Key, item.Value);
        }
        obj.Up();
        Storage(obj);
    }

    public static void RemoveItem(WebObj obj, string name, bool isCode)
    {
        if (isCode)
        {
            HtmlCodeList[obj.UUID].Codes.Remove(name);
            RemoveCode(obj, name);
        }
        else
        {
            HtmlCodeList[obj.UUID].Files.Remove(name);
            RemoveFile(obj, name);
        }
        obj.Up();
        Storage(obj);
    }

    private static void RemoveCode(WebObj obj, string file)
    {
        using var codeSQL = new SqliteConnection(WebCodeConnStr);
        var arg = new { obj.UUID, File = file };
        var list = codeSQL.Query<CWebObj>("SELECT code FROM web WHERE uuid=@uuid AND name=@name", arg);
        if (list.Any())
        {
            codeSQL.Execute("DELETE FROM web WHERE uuid=@uuid AND name=@name", arg);
            if (ServerMain.Config.CodeSetting.CodeLog)
            {
                Task.Run(() =>
                {
                    using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                    var old = list.First().code;
                    CodeLogSQL.Execute($"INSERT INTO code (uuid,name,code,time) VALUES(@uuid,@name,@code,@time)", new
                    {
                        uuid = obj.UUID,
                        name = file,
                        code = old,
                        time = DateTime.Now.ToString()
                    });
                });
            }
        }
    }

    private static void RemoveFile(WebObj obj, string file)
    {
        using var fileSQL = new SqliteConnection(WebFileConnStr);
        var arg = new { obj.UUID, File = file };
        var list = fileSQL.Query<FWebObj>("SELECT data FROM web WHERE uuid=@uuid AND name=@name", arg);
        if (list.Any())
        {
            fileSQL.Execute("DELETE FROM web WHERE uuid=@uuid AND name=@name", arg);
            if (ServerMain.Config.CodeSetting.CodeLog)
            {
                Task.Run(() =>
                {
                    using var CodeLogSQL = new SqliteConnection(WebLogConnStr);
                    var old = list.First().data;
                    CodeLogSQL.Execute($"INSERT INTO file (uuid,name,data,time) VALUES(@uuid,@name,@data,@time)", new
                    {
                        uuid = obj.UUID,
                        name = file,
                        data = old,
                        time = DateTime.Now.ToString()
                    });
                });
            }
        }
    }
}