using ColoryrServer.SDK;
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
        private static readonly ConcurrentDictionary<string, Dictionary<string, string>> HtmlCodeList = new();

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

            var list = new DirectoryInfo(HtmlCodeLocal).GetDirectories();
            foreach (var item in list)
            {
                ServerMain.LogOut($"加载网页{item.Name}");
                var list1 = new DirectoryInfo(item.FullName).GetFiles();
                var list2 = new Dictionary<string, string>();
                foreach (var item1 in list1)
                {
                    if (item1.Extension is ".html" or ".css" or ".js" or ".json" or ".txt")
                        list2.Add(item1.Name, File.ReadAllText(item1.FullName));
                    else
                        list2.Add(item1.Name, null);
                }
                HtmlCodeList.TryAdd(item.Name, list2);
            }

            list = new DirectoryInfo(HtmlLocal).GetDirectories();
            foreach (var item in list)
            {
                var list1 = new DirectoryInfo(item.FullName).GetFiles();
                var list2 = new Dictionary<string, byte[]>();
                if (!HtmlCodeList.ContainsKey(item.Name))
                {
                    continue;
                }
                foreach (var item1 in list1)
                {
                    if (!(item1.Extension is ".html" or ".css" or ".js" or ".json" or ".txt"))
                    {
                        if (!HtmlCodeList[item.Name].ContainsKey(item1.Name))
                            HtmlCodeList[item.Name].Add(item1.Name, null);
                    }
                        
                    var temp = File.ReadAllBytes(item1.FullName);
                    list2.Add(item1.Name, temp);
                }
                HtmlList.TryAdd(item.Name, list2);
            }
        }
        private static void DeleteAll(string UUID)
        {
            string time = string.Format("{0:s}", DateTime.Now).Replace(":", ".");
            string dir = HtmlRemoveLocal + $"[{UUID}]-{time}" + "\\";
            Directory.CreateDirectory(dir);
            foreach (var item in HtmlCodeList[UUID])
            {
                File.WriteAllText(dir + item.Key, item.Value);
            }
            foreach (var item in HtmlList[UUID])
            {
                if (File.Exists(dir + item.Key))
                {
                    continue;
                }
                File.WriteAllBytes(dir + item.Key, item.Value);
            }

            string temp = HtmlLocal + UUID + "\\";
            if (Directory.Exists(temp))
            {
                var info = new DirectoryInfo(temp);
                var list = info.GetFiles();
                foreach (var item in list)
                {
                    item.Delete();
                }
                info.Delete();
            }

            temp = HtmlCodeLocal + UUID + "\\";
            if (Directory.Exists(temp))
            {
                var info = new DirectoryInfo(temp);
                var list = info.GetFiles();
                foreach (var item in list)
                {
                    item.Delete();
                }
                info.Delete();
            }
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
            }
            else if(temp1[1] is ".html" or ".css" or ".js" or ".json" or ".txt")
            {
                temp = HtmlCodeLocal + UUID + "\\";
                if (File.Exists(temp + Name))
                    File.Delete(temp + Name);
            }
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

            if (!HtmlCodeList[UUID].ContainsKey(Name))
                HtmlCodeList[UUID].Add(Name, null);
        }

        public static void AddCode(string UUID, string Name, string Code)
        { 
            Dictionary<string, string> list;
            if (HtmlCodeList.ContainsKey(UUID))
            {
                list = HtmlCodeList[UUID];
            }
            else
            {
                list = new();
                HtmlCodeList.TryAdd(Name, list);
            }

            if (list.ContainsKey(Name))
            {
                list.Remove(Name);
            }

            list.Add(Name, Code);
            Save(UUID, Name, Code);
        }

        public static void Remove(string UUID, string Name)
        {
            if (HtmlCodeList.ContainsKey(UUID))
            {
                HtmlCodeList[UUID].Remove(Name);
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
