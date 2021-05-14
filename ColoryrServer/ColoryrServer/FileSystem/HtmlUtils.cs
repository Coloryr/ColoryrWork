using ColoryrServer.SDK;
using Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
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
        private static readonly ConcurrentDictionary<string, WebObj> HtmlCodeList = new();

        private static byte[] Index;

        public static byte[] GetFile(string local)
        {
            var temp = local.Split('/');
            string uuid = temp[1];
            if (temp.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(uuid))
                {
                    return Index;
                }
                if (HtmlList.ContainsKey(uuid))
                {
                    return HtmlList[uuid]["index.html"];
                }
            }
            else
            {
                string name = temp[3];
                if (HtmlList.ContainsKey(uuid))
                {
                    if (HtmlList[uuid].ContainsKey(name))
                    {
                        return HtmlList[uuid][name];
                    }
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

            Index = File.ReadAllBytes(HtmlLocal + "index.html");

            var list = new DirectoryInfo(HtmlCodeLocal).GetFiles();
            foreach (var item in list)
            {
                try
                {
                    WebObj obj = JsonConvert.DeserializeObject<WebObj>(File.ReadAllText(item.FullName));
                    HtmlCodeList.TryAdd(item.Name, obj);
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
            if (temp1.Length == 1)
            {
                if (File.Exists(temp + Name))
                    File.Delete(temp + Name);
                HtmlCodeList[UUID].Files.Remove(Name);
            }
            else if (temp1[1] is ".html" or ".css" or ".js" or ".json" or ".txt")
            {
                HtmlCodeList[UUID].Codes.Remove(Name);
            }
            else
            {
                if (File.Exists(temp + Name))
                    File.Delete(temp + Name);
                HtmlCodeList[UUID].Files.Remove(Name);
            }
            Storage(HtmlCodeLocal + UUID, HtmlCodeList[UUID]);
        }

        private static void Save(string UUID, string Name, string Code)
        {
            string temp = HtmlCodeLocal + UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            File.WriteAllText(temp + Name, Code, Encoding.UTF8);

            temp = HtmlLocal + UUID + "\\";
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
            Storage(HtmlCodeLocal + obj.UUID, obj);
        }

        public static void AddCode(string UUID, string Name, string Code)
        { 
            WebObj obj = HtmlCodeList[UUID];
            if (obj.Codes.ContainsKey(Name))
            {
                obj.Codes.Remove(Name);
            }

            obj.Codes.Add(Name, Code);
            Save(UUID, Name, Code);
            Storage(HtmlCodeLocal + obj.UUID, obj);
        }

        public static void Remove(string UUID, string Name)
        {
            if (HtmlCodeList.ContainsKey(UUID))
            {
                HtmlCodeList[UUID].Codes.Remove(Name);
            }
            if (HtmlList.ContainsKey(UUID))
            {
                HtmlList[UUID].Remove(Name);
            }
            Delete(UUID, Name);
        }

        public static bool Have(string UUID)
        {
            return HtmlCodeList.ContainsKey(UUID);
        }
    }
}
