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
        public static void DeleteAll(WebObj obj)
        {
            string time = string.Format("{0:s}", DateTime.Now).Replace(":", ".");
            string dir = HtmlRemoveLocal + $"{obj.UUID}-{time}" + "\\";
            Directory.CreateDirectory(dir);
            string info =
$@"UUID:{obj.UUID},
Text:{obj.Text},
Version:{obj.Version}
";
            File.WriteAllText(dir + "info.txt", info);
            File.WriteAllText(dir + obj.UUID + ".json", JsonConvert.SerializeObject(obj));
            string temp = HtmlLocal + obj.UUID + "\\";
            foreach (var item in Directory.GetFiles(temp))
            {
                File.Delete(item);
            }
            Directory.Delete(temp);

            temp = HtmlCodeLocal + obj.UUID + ".json" ;
            File.Delete(temp);

            foreach (var item in HtmlList[obj.UUID])
            {
                File.WriteAllBytes(dir + item.Key, item.Value);
            }
            HtmlCodeList.TryRemove(obj.UUID, out var temp1);
            HtmlList.TryRemove(obj.UUID, out var temp2);
            ServerMain.LogOut($"Web[{obj.UUID}]删除");
        }
        public static void Save(WebObj obj, string name, string code)
        {
            obj.Codes[name] = code;
            Save(obj.UUID, name, code);
            HtmlList[obj.UUID][name] = File.ReadAllBytes(HtmlLocal + obj.UUID + "\\" + name);
            obj.Up();
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

        public static void AddFile(WebObj obj, string Name, byte[] Data)
        {
            string temp = HtmlLocal + obj.UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            File.WriteAllBytes(temp + Name, Data);

            HtmlList[obj.UUID].Add(Name, Data);
            HtmlCodeList[obj.UUID].Files.Add(Name, Name);

            obj.Up();
            Storage(HtmlCodeLocal + obj.UUID + ".json", obj);
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
            obj.Up();
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
        }

        public static void Remove(WebObj obj, string Name)
        {
            string temp = HtmlLocal + obj.UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);
            var temp1 = Name.Split('.');
            if (temp1.Length != 1)
            {
                if (temp1[1] is "html" or "css" or "js" or "json" or "txt")
                    HtmlCodeList[obj.UUID].Codes.Remove(Name);
                else
                    HtmlCodeList[obj.UUID].Files.Remove(Name);
            }
            if (File.Exists(temp + Name))
                File.Delete(temp + Name);
            HtmlList[obj.UUID].Remove(Name);

            obj.Up();
            Storage(HtmlCodeLocal + obj.UUID + ".json", HtmlCodeList[obj.UUID]);
        }
    }
}
