using ColoryrServer.SDK;
using Lib.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace ColoryrServer.FileSystem
{
    internal class FileRam
    {
        public static string Local;

        public static void Start()
        {
            Local = ServerMain.RunLocal + @"/FileRam/";
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
        }

        public static ConcurrentDictionary<string, dynamic> Load(string uuid)
        {
            try
            {
                var data = File.ReadAllText(Local + uuid);
                return JsonConvert.DeserializeObject<ConcurrentDictionary<string, dynamic>>(data);
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }

        public static void Save(string uuid, ConcurrentDictionary<string, dynamic> data)
        {
            try
            {
                var temp = JsonConvert.SerializeObject(data);
                File.WriteAllText(Local + uuid, temp);
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }

        public static List<string> GetAll()
        {
            try
            {
                var list = new List<string>();
                var data = Function.GetPathFileName(Local);
                foreach (var item in data)
                {
                    list.Add(item.Name);
                }
                return list;
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }

        public static void Remove(string temp1)
        {
            try
            {
                File.Delete(Local + temp1);
            }
            catch (Exception e)
            {
                throw new VarDump(e);
            }
        }
    }
}
