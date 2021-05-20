using ColoryrServer.SDK;
using Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.FileSystem
{
    internal class HtmlUtils
    {
        private static readonly string HtmlLocal = ServerMain.RunLocal + @"Static\";
        private static readonly string HtmlCodeLocal = ServerMain.RunLocal + @"Codes\Static\";
        private static readonly string HtmlRemoveLocal = ServerMain.RunLocal + @"Removes\Static\";

        private static readonly ConcurrentDictionary<string, Dictionary<string, byte[]>> HtmlList = new();
        public static ConcurrentDictionary<string, WebObj> HtmlCodeList { get; private set; } = new();

        private static byte[] HtmlIndex;
        public static byte[] Html404 { get; private set; }

        public static WebObj GetHtml(string uuid)
        {
            if (!HtmlCodeList.TryGetValue(uuid, out var item))
            {
                return null;
            }
            return item;
        }

        public static byte[] GetFile(string local)
        {
            var temp = local.Split('/');
            string uuid = temp[1];
            if (temp.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(uuid))
                {
                    return HtmlIndex;
                }
                if (HtmlList.ContainsKey(uuid))
                {
                    if (HtmlList[uuid].TryGetValue("index.html", out var temp1))
                        return temp1;
                }
            }
            else
            {
                string name = temp[2];
                if (HtmlList.ContainsKey(uuid))
                {
                    if (HtmlList[uuid].TryGetValue(name, out var temp1))
                        return temp1;
                }
            }
            return null;
        }

        public static void Start()
        {
            if (!Directory.Exists(HtmlLocal))
                Directory.CreateDirectory(HtmlLocal);
            if (!Directory.Exists(HtmlCodeLocal))
                Directory.CreateDirectory(HtmlCodeLocal);
            if (!Directory.Exists(HtmlRemoveLocal))
                Directory.CreateDirectory(HtmlRemoveLocal);

            if (!File.Exists(HtmlLocal + "index.html"))
            {
                File.WriteAllText(HtmlLocal + "index.html", ColoryrServer_Resource.IndexHtml, Encoding.UTF8);
            }

            if (!File.Exists(HtmlLocal + "404.html"))
            {
                File.WriteAllText(HtmlLocal + "404.html", ColoryrServer_Resource._404Html, Encoding.UTF8);
            }

            HtmlIndex = File.ReadAllBytes(HtmlLocal + "index.html");
            Html404 = File.ReadAllBytes(HtmlLocal + "404.html");

            var list = new DirectoryInfo(HtmlCodeLocal).GetFiles();
            foreach (var item in list)
            {
                try
                {
                    WebObj obj = JsonConvert.DeserializeObject<WebObj>(File.ReadAllText(item.FullName));
                    HtmlCodeList.TryAdd(obj.UUID, obj);
                    string dir = HtmlLocal + obj.UUID + "\\";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        foreach (var item1 in obj.Codes)
                        {
                            File.WriteAllText(dir + item1.Key, item1.Value);
                        }
                        var list1 = new List<string>();
                        foreach (var item1 in obj.Files)
                        {
                            if (!File.Exists(dir + item1.Key))
                            {
                                list1.Add(item1.Key);
                            }
                        }
                        if (list1.Count != 0)
                        {
                            foreach (var item1 in list1)
                            {
                                obj.Files.Remove(item1);
                            }
                        }
                    }
                    Dictionary<string, byte[]> list2 = new();
                    foreach (var item1 in new DirectoryInfo(dir).GetFiles())
                    {
                        list2.Add(item1.Name, File.ReadAllBytes(item1.FullName));
                    }
                    HtmlList.TryAdd(obj.UUID, list2);
                    ServerMain.LogOut($"加载网页{item.Name}");
                }
                catch (Exception e)
                {
                    ServerMain.LogOut($"加载网页{item.Name}错误");
                    ServerMain.LogError(e);
                }
            }
        }
        private static void DeleteAll(string UUID)
        {
            string time = string.Format("{0:s}", DateTime.Now).Replace(":", ".");
            string dir = HtmlRemoveLocal + $"[{UUID}]-{time}" + "\\";
            Directory.CreateDirectory(dir);
            var obj = HtmlCodeList[UUID];
            string info =
$@"/*
UUID:{obj.UUID},
Text:{obj.Text},
Version:{obj.Version}
*/
";
            File.WriteAllText(dir + "info.txt", info);
            string temp = HtmlLocal + UUID + "\\";
            if (Directory.Exists(temp))
            {
                File.Delete(temp + UUID);
            }

            temp = HtmlCodeLocal + UUID + "\\";
            if (Directory.Exists(temp))
            {
                var info1 = new DirectoryInfo(temp);
                var list = info1.GetFiles();
                foreach (var item in list)
                {
                    item.Delete();
                }
                info1.Delete();
            }
            foreach (var item in HtmlList[UUID])
            {
                File.WriteAllBytes(dir + item.Key, item.Value);
            }
            HtmlList.TryRemove(UUID, out var temp1);
        }
        private static void Delete(string UUID, string Name)
        {
            string temp = HtmlLocal + UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);
            var temp1 = Name.Split('.');
            if (temp1.Length != 1)
            {
                if (temp1[1] is "html" or "css" or "js" or "json" or "txt")
                    HtmlCodeList[UUID].Codes.Remove(Name);
                else
                    HtmlCodeList[UUID].Files.Remove(Name);
            }
            if (File.Exists(temp + Name))
                File.Delete(temp + Name);
            HtmlList[UUID].Remove(Name);
            
            Storage(HtmlCodeLocal + UUID + ".json", HtmlCodeList[UUID]);
        }


        public static void Save(WebObj obj, string name, string code)
        {
            obj.Codes[name] = code;
            Save(obj.UUID, name, code);
            HtmlList[obj.UUID][name] = File.ReadAllBytes(HtmlLocal + obj.UUID + "\\" + name);
            Storage(HtmlCodeLocal + obj.UUID + ".json", obj);
        }
        private static void Save(string UUID, string Name, string Code)
        {
            string temp = HtmlLocal + UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            var temp1 = Name.Split('.');
            if (temp1.Length == 1)
            {
                File.WriteAllText(temp + Name, Code, Encoding.UTF8);
            }
            else
            {
                if (temp1[1] is "html")
                {
                    Code = Tools.CompressHTML(Code);
                    File.WriteAllText(temp + Name, Code, Encoding.UTF8);
                }
                else if (temp1[1] is "css")
                {
                    Code = Tools.CompressCSS(Code);
                    File.WriteAllText(temp + Name, Code, Encoding.UTF8);
                }
                else if (temp1[1] is "js")
                {
                    Code = Tools.CompressJS(Code);
                    File.WriteAllText(temp + Name, Code, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllText(temp + Name, Code, Encoding.UTF8);
                }
            }
        }

        public static void AddFile(string UUID, string Name, byte[] Data)
        {
            string temp = HtmlLocal + UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            File.WriteAllBytes(temp + Name, Data);

            if (!HtmlList[UUID].ContainsKey(Name))
                HtmlList[UUID].Add(Name, Data);
            else
                HtmlList[UUID][Name] = Data;

            if (!HtmlCodeList[UUID].Files.ContainsKey(Name))
                HtmlCodeList[UUID].Files.Add(Name, Name);
        }

        private static void Storage(string Local, object obj)
        {
            Task.Run(() =>
            {
                try
                {
                    File.WriteAllText(Local, JsonConvert.SerializeObject(obj, Formatting.Indented));
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
            Save(obj.UUID, "index.html", obj.Codes["index.html"]);
            Save(obj.UUID, "js.js", obj.Codes["js.js"]);
            Storage(HtmlCodeLocal + obj.UUID + ".json", obj);
            Dictionary<string, byte[]> list2 = new();
            foreach (var item1 in new DirectoryInfo(HtmlLocal + obj.UUID).GetFiles())
            {
                list2.Add(item1.Name, File.ReadAllBytes(item1.FullName));
            }
            HtmlList.TryAdd(obj.UUID, list2);
        }

        public static void AddCode(WebObj obj, string Name, string Code)
        {
            obj.Codes.Add(Name, Code);
            Save(obj, Name, Code);
            Storage(HtmlCodeLocal + obj.UUID + ".json", obj);
        }

        public static void Remove(WebObj obj, string Name)
        {
            Delete(obj.UUID, Name);
        }
    }
}
