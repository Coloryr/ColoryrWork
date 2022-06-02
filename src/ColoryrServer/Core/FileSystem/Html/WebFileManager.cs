using ColoryrServer.Core.Utils;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using Dapper;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.FileSystem.Html;

public class WebFileManager
{
    private static readonly string CodeWebDB = ServerMain.RunLocal + "Codes/CodeWeb.db";
    private static readonly string CodeWebLogDB = ServerMain.RunLocal + "Codes/CodeWebLog.db";
    private static readonly string HtmlStatic = ServerMain.RunLocal + "Static/";
    private static readonly string WebLocal = ServerMain.RunLocal + "Web/";
    private static readonly string WebCodeLocal = ServerMain.RunLocal + "Codes/Static/";
    private static readonly string WebRemoveLocal = ServerMain.RunLocal + "Removes/Static/";
    private static readonly string WebNodeJS = WebLocal + "node_modules/";

    private static string CodeWebConnStr;
    private static string CodeWebLogConnStr;

    private static readonly ConcurrentDictionary<string, Dictionary<string, byte[]>> HtmlList = new();
    private static readonly ConcurrentDictionary<string, Dictionary<string, StaticTempFile>> HtmlFileList = new();

    public static StaticDir BaseDir { get; private set; }
    public static ConcurrentDictionary<string, WebObj> HtmlCodeList { get; } = new();

    private static bool IsRun;
    private static readonly Thread Thread = new(TickTask);
    private static void TickTask()
    {
        while (IsRun)
        {
            try
            {
                foreach (var item in HtmlFileList)
                {
                    foreach (var item1 in item.Value)
                    {
                        item1.Value.Tick();
                        if (item1.Value.Time <= 0)
                        {
                            item.Value.Remove(item1.Key);
                        }
                    }
                }
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
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

    public static byte[] GetByUUID(string uuid)
    {
        if (HtmlList.ContainsKey(uuid))
        {
            if (HtmlList[uuid].TryGetValue("index.html", out var temp1))
                return temp1;
        }
        return null;
    }

    public static byte[] GetFile(string uuid, string name)
    {
        if (HtmlList.ContainsKey(uuid))
        {
            int index = name.LastIndexOf(".");
            string type = name[index..];
            if (ServerMain.Config.Requset.Temp.Contains(type))
            {
                if (!HtmlFileList.ContainsKey(uuid))
                {
                    HtmlFileList.TryAdd(uuid, new());
                }
                if (HtmlFileList[uuid].ContainsKey(name))
                {
                    return HtmlFileList[uuid][name].Data;
                }
                else
                {
                    string local = WebLocal + uuid + "/" + name;
                    if (File.Exists(local))
                    {
                        var data = File.ReadAllBytes(local);
                        var obj = new StaticTempFile()
                        {
                            Data = data
                        };
                        obj.Reset();
                        HtmlFileList[uuid].TryAdd(name, obj);
                        return data;
                    }
                    return null;
                }
            }
            else
            {
                if (HtmlList[uuid].TryGetValue(name, out var temp1))
                    return temp1;
            }
        }
        return null;
    }

    public static HttpResponseStream GetStream(HttpRequest request, string arg)
    {
        return FileHttpStream.StartStream(request, $"{HtmlStatic}/{arg}");
    }

    public static void Start()
    {
        if (!Directory.Exists(HtmlStatic))
            Directory.CreateDirectory(HtmlStatic);
        if (!Directory.Exists(WebLocal))
            Directory.CreateDirectory(WebLocal);
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
        using var CodeLogSQL = new SqliteConnection(CodeWebConnStr);
        string sql = @"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `Version` integer,
  `IsVue` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeSQL.Execute(sql);

        sql = @"create table if not exists web (
  `id` integer NOT NULL PRIMARY KEY AUTOINCREMENT,
  `UUID` text,
  `Text` text,
  `File` text,
  `Code` text,
  `Version` integer,
  `CreateTime` text,
  `UpdateTime` text
);";
        CodeLogSQL.Execute(sql);

        if (!File.Exists(HtmlStatic + "index.html"))
        {
            File.WriteAllText(HtmlStatic + "index.html", WebResource.IndexHtml, Encoding.UTF8);
        }

        if (!File.Exists(HtmlStatic + "404.html"))
        {
            File.WriteAllText(HtmlStatic + "404.html", WebResource._404Html, Encoding.UTF8);
        }

        if (!File.Exists(HtmlStatic + "favicon.ico"))
        {
            File.WriteAllBytes(HtmlStatic + "favicon.ico", WebResource.Icon);
        }

        BaseDir = new StaticDir(HtmlStatic);

        var list = CodeSQL.Query<WebObj>("SELECT UUID,Text,Version,CreateTime,UpdateTime,IsVue FROM web");

        foreach (var item in list)
        {
            try
            {
                item.Codes = new();
                item.Files = new();

                string dir = WebCodeLocal + item.UUID + "/";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var info = new DirectoryInfo(dir);

                if (item.IsVue)
                {
                    List<FileInfo> list1 = new();
                    list1.AddRange(info.GetFiles());
                    foreach (var item1 in info.GetDirectories())
                    {
                        if (item1.Name is not "node_modules" or "runtime")
                        {
                            var files1 = FileUtils.GetDirectoryFile(item1);
                            foreach (var item2 in files1)
                            {
                                string name = item2.FullName.Replace(info.FullName, "");
                                item.Codes.Add(name, File.ReadAllText(item1.FullName));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var item1 in info.GetFiles())
                    {
                        if (item1.Extension is "json" or "html" or "js")
                        {
                            item.Codes.Add(item1.Name + "." + item1.Extension, File.ReadAllText(item1.FullName));
                        }
                    }

                    string dir1 = WebLocal + item.UUID + "/";
                    if (!Directory.Exists(dir1))
                    {
                        Directory.CreateDirectory(dir1);
                    }

                    var list1 = new List<string>();
                    var info1 = new DirectoryInfo(dir1);
                    foreach (var item1 in info1.GetFiles())
                    {
                        if (item1.Extension is not "json" or "html" or "js")
                        {
                            item.Files.Add(item1.Name, null);
                        }
                    }

                    Dictionary<string, byte[]> list2 = new();
                    foreach (var item1 in new DirectoryInfo(dir1).GetFiles())
                    {
                        if (ServerMain.Config.Requset.Temp.Contains(item1.Extension))
                            continue;
                        list2.Add(item1.Name, File.ReadAllBytes(item1.FullName));
                    }
                    HtmlCodeList.TryAdd(item.UUID, item);
                    HtmlList.TryAdd(item.UUID, list2);
                }

                ServerMain.LogOut($"加载Web：{item.UUID}");
            }
            catch (Exception e)
            {
                ServerMain.LogOut($"加载Web：{item.UUID}错误");
                ServerMain.LogError(e);
            }
        }
        IsRun = true;
        Thread.Start();
    }
    public static void Stop()
    {
        IsRun = false;
        BaseDir.Stop();
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
        string temp = WebLocal + obj.UUID + "/";
        foreach (var item in Directory.GetFiles(temp))
        {
            File.Delete(item);
        }
        Directory.Delete(temp);

        temp = WebCodeLocal + obj.UUID + ".json";
        File.Delete(temp);

        foreach (var item in HtmlList[obj.UUID])
        {
            File.WriteAllBytes(dir + item.Key, item.Value);
        }
        HtmlCodeList.TryRemove(obj.UUID, out var temp1);
        HtmlList.TryRemove(obj.UUID, out var temp2);
        ServerMain.LogOut($"Web[{obj.UUID}]删除");
    }
    public static void SaveVue(WebObj obj, string name, string code, bool isCode = true, byte[] file = null)
    {
        if (isCode)
            obj.Codes[name] = code;
        else
            obj.Files[name] = file;
        Save(obj.UUID, name, code, true, isCode);
        obj.Up();
        Storage(obj);
    }
    public static void Save(WebObj obj, string name, string code)
    {
        obj.Codes[name] = code;
        Save(obj.UUID, name, code, false, false);
        HtmlList[obj.UUID][name] = File.ReadAllBytes(WebLocal + obj.UUID + "/" + name);
        obj.Up();
        Storage(obj);
    }
    public static void SaveFile(WebObj obj, string name, byte[] data)
    {
        string temp = WebLocal + obj.UUID + "/";
        if (File.Exists(temp + name))
            File.Delete(temp + name);
        HtmlList[obj.UUID].Remove(name);
        File.WriteAllBytes(temp + name, data);
        HtmlList[obj.UUID].Add(name, data);
        obj.Up();
        Storage(obj);
    }
    private static void Save(string uuid, string name, string code, bool isVue, bool isCode = true, byte[] file = null)
    {
        if (isVue)
        {
            if (isCode)
            {
                File.WriteAllText(WebCodeLocal + uuid + "/" + name, code);
            }
            else
            {
                File.WriteAllBytes(WebCodeLocal + uuid + "/" + name, file);
            }
        }
        else
        {
            string temp = WebLocal + uuid + "/";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            var temp1 = name.Split('.');
            if (temp1.Length == 1)
            {
                File.WriteAllText(temp + name, code, Encoding.UTF8);
            }
            else
            {
                if (temp1[1] is "html")
                {
                    code = Tools.CompressHTML(code);
                    File.WriteAllText(temp + name, code, Encoding.UTF8);
                }
                else if (temp1[1] is "css")
                {
                    code = Tools.CompressCSS(code);
                    File.WriteAllText(temp + name, code, Encoding.UTF8);
                }
                else if (temp1[1] is "js")
                {
                    code = Tools.CompressJS(code);
                    File.WriteAllText(temp + name, code, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllText(temp + name, code, Encoding.UTF8);
                }
            }
        }
    }

    public static void AddFile(WebObj obj, string Name, byte[] data)
    {
        string temp = WebLocal + obj.UUID + "/";
        if (!Directory.Exists(temp))
            Directory.CreateDirectory(temp);

        File.WriteAllBytes(temp + Name, data);

        if (!obj.IsVue)
            HtmlList[obj.UUID].Add(Name, data);
        HtmlCodeList[obj.UUID].Files.Add(Name, data);

        obj.Up();
        Storage(obj);
    }

    private static void StorageFile(WebObj obj, string name, string old, string code, bool isVue = false, bool isCode = false, byte[] file = null)
    {
        try
        {
            using var CodeLogSQL = new SqliteConnection(CodeWebLogConnStr);
            CodeLogSQL.Execute($"INSERT INTO web (UUID,Text,Code,Version,CreateTime,UpdateTime,File,Code) VALUES(@UUID,@Text,@Version,@CreateTime,@UpdateTime,@File,@Code)", new
            {
                obj.UUID,
                obj.Text,
                obj.Version,
                obj.CreateTime,
                obj.UpdateTime,
                File = name,
                Code = old
            });

            if (code == null)
                return;

            if (isVue && isCode)
            {
                obj.Codes[name] = code;
                File.WriteAllText(WebCodeLocal + obj.UUID + "/" + name, code);
            }
            else
            {
                File.WriteAllBytes(WebCodeLocal + obj.UUID + "/" + name, file);
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
                var list = CodeSQL.Query<CSFileObj>($"SELECT UUID,Text,Version,CreateTime,UpdateTime FROM web WHERE UUID=@UUID",
                    new { obj.UUID });
                if (list.Any())
                {
                    CodeSQL.Execute($"UPDATE web SET Text=@Text,Version=@Version,CreateTime=@CreateTime,UpdateTime=@UpdateTime WHERE UUID=@UUID", obj);
                }
                else
                {
                    CodeSQL.Execute($"INSERT INTO web (UUID,Text,Version,CreateTime,UpdateTime) VALUES(@UUID,@Text,@Version,@CreateTime,@UpdateTime)", obj);
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
        string temp = WebLocal + obj.UUID + "/";
        if (!Directory.Exists(temp))
            Directory.CreateDirectory(temp);
        HtmlCodeList.TryAdd(obj.UUID, obj);
        StorageFile(obj, "index.html", "", obj.Codes["index.html"]);
        StorageFile(obj, "js.js", "", obj.Codes["js.js"]);
        obj.Up();
        Storage(obj);
        Dictionary<string, byte[]> list2 = new();
        foreach (var item1 in new DirectoryInfo(WebLocal + obj.UUID).GetFiles())
        {
            list2.Add(item1.Name, File.ReadAllBytes(item1.FullName));
        }
        HtmlList.TryAdd(obj.UUID, list2);
    }

    public static void AddCode(WebObj obj, string name, string code)
    {
        obj.Codes.Add(name, code);
        Save(obj, name, code);
    }

    public static void Remove(WebObj obj, string name)
    {
        string temp = WebLocal + obj.UUID + "/";
        if (!Directory.Exists(temp))
            Directory.CreateDirectory(temp);

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
        if (File.Exists(temp + name))
            File.Delete(temp + name);
        HtmlList[obj.UUID].Remove(name);

        obj.Up();
        Storage(obj);
    }
}
