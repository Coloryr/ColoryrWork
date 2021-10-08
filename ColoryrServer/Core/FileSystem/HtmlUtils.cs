﻿using ColoryrServer.Core;
using ColoryrServer.SDK;
using Lib.Build.Object;
using Lib.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.FileSystem
{
    public class HtmlFileObj
    {
        public byte[] Data { get; set; }
        public int Time { get; private set; }
        public void Reset()
        {
            Time = ServerMain.Config.Requset.TempTime;
        }
        public void Tick()
        {
            Time--;
        }
    }
    public class HtmlUtils
    {
        private static readonly string HtmlLocal = ServerMain.RunLocal + @"Static/";
        private static readonly string HtmlCodeLocal = ServerMain.RunLocal + @"Codes/Static/";
        private static readonly string HtmlRemoveLocal = ServerMain.RunLocal + @"Removes/Static/";

        private static readonly ConcurrentDictionary<string, Dictionary<string, byte[]>> HtmlList = new();
        private static readonly ConcurrentDictionary<string, Dictionary<string, HtmlFileObj>> HtmlFileList = new();
        public static ConcurrentDictionary<string, WebObj> HtmlCodeList { get; } = new();

        private static Dictionary<string, byte[]> Index = new();
        public static byte[] HtmlIcon { get; private set; }
        public static byte[] Html404 { get; private set; }
        public static byte[] HtmlIndex { get; private set; }

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
                else if (uuid == "favicon.ico")
                {
                    return HtmlIcon;
                }
                else if (HtmlList.ContainsKey(uuid))
                {
                    if (HtmlList[uuid].TryGetValue("index.html", out var temp1))
                        return temp1;
                }
                else if (Index.TryGetValue(uuid, out var temp1))
                {
                    return temp1;
                }
                else
                    return Html404;
            }
            else
            {
                string name = temp[2];
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
                        string temp1 = HtmlLocal + uuid + "\\" + name;
                        if (File.Exists(temp1))
                        {
                            var data = File.ReadAllBytes(temp1);
                            var obj = new HtmlFileObj()
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
                    if (HtmlList.ContainsKey(uuid))
                    {
                        if (HtmlList[uuid].TryGetValue(name, out var temp1))
                            return temp1;
                    }
                }
            }
            return null;
        }

        public static byte[] GetFile(string uuid, string name)
        {
            if (HtmlList.ContainsKey(uuid))
            {
                if (HtmlList[uuid].TryGetValue(name, out var temp1))
                    return temp1;
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

            if (!File.Exists(HtmlLocal + "favicon.ico"))
            {
                File.WriteAllBytes(HtmlLocal + "favicon.ico", Function.ToByte(ColoryrServer_Resource.ColoryrWork));
            }

            var list = new DirectoryInfo(HtmlLocal).GetFiles();
            foreach (var item in list)
            {
                if (item.Name.ToLower() is "404.html")
                    Html404 = File.ReadAllBytes(item.FullName);
                else if (item.Name.ToLower() is "favicon.ico")
                    HtmlIcon = File.ReadAllBytes(item.FullName);
                else if (item.Name.ToLower() is "index.html")
                    HtmlIndex = File.ReadAllBytes(item.FullName);
                else 
                    Index.Add(item.Name, File.ReadAllBytes(item.FullName));
            }

            list = new DirectoryInfo(HtmlCodeLocal).GetFiles();
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
                        if (ServerMain.Config.Requset.Temp.Contains(item1.Extension))
                            continue;
                        list2.Add(item1.Name, File.ReadAllBytes(item1.FullName));
                    }
                    HtmlList.TryAdd(obj.UUID, list2);
                    ServerMain.LogOut($"加载Web：{item.Name}");
                }
                catch (Exception e)
                {
                    ServerMain.LogOut($"加载Web：{item.Name}错误");
                    ServerMain.LogError(e);
                }
            }
            IsRun = true;
            Thread.Start();
        }
        public static void Stop()
        {
            IsRun = false;
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

            temp = HtmlCodeLocal + obj.UUID + ".json";
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
        public static void SaveFile(WebObj obj, string name, byte[] data)
        {
            string temp = HtmlLocal + obj.UUID + "\\";
            if (File.Exists(temp + name))
                File.Delete(temp + name);
            HtmlList[obj.UUID].Remove(name);
            File.WriteAllBytes(temp + name, data);
            HtmlList[obj.UUID].Add(name, data);
            obj.Up();
            Storage(HtmlCodeLocal + obj.UUID + ".json", obj);
        }
        private static void Save(string uuid, string name, string code)
        {
            string temp = HtmlLocal + uuid + "\\";
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

        public static void AddFile(WebObj obj, string Name, byte[] data)
        {
            string temp = HtmlLocal + obj.UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);

            File.WriteAllBytes(temp + Name, data);

            HtmlList[obj.UUID].Add(Name, data);
            HtmlCodeList[obj.UUID].Files.Add(Name, Name);

            obj.Up();
            Storage(HtmlCodeLocal + obj.UUID + ".json", obj);
        }

        private static void Storage(string local, object obj)
        {
            Task.Run(() =>
            {
                try
                {
                    File.WriteAllText(local, JsonConvert.SerializeObject(obj, Formatting.Indented));
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            });
        }

        public static void New(WebObj obj)
        {
            string temp = HtmlLocal + obj.UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);
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

        public static void AddCode(WebObj obj, string name, string code)
        {
            obj.Codes.Add(name, code);
            Save(obj, name, code);
        }

        public static void Remove(WebObj obj, string name)
        {
            string temp = HtmlLocal + obj.UUID + "\\";
            if (!Directory.Exists(temp))
                Directory.CreateDirectory(temp);
            var temp1 = name.Split('.');
            if (temp1.Length != 1)
            {
                if (temp1[1] is "html" or "css" or "js" or "json" or "txt")
                    HtmlCodeList[obj.UUID].Codes.Remove(name);
                else
                    HtmlCodeList[obj.UUID].Files.Remove(name);
            }
            if (File.Exists(temp + name))
                File.Delete(temp + name);
            HtmlList[obj.UUID].Remove(name);

            obj.Up();
            Storage(HtmlCodeLocal + obj.UUID + ".json", HtmlCodeList[obj.UUID]);
        }
    }
}
