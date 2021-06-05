using ColoryrServer.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.FileSystem
{
    internal class NoteFile
    {
        private static readonly string DllFileLocal = ServerMain.RunLocal + @"Notes/Dll/";
        private static readonly string ClassFileLocal = ServerMain.RunLocal + @"Notes/Class/";

        public static void Start()
        {
            if (!Directory.Exists(DllFileLocal))
            {
                Directory.CreateDirectory(DllFileLocal);
            }
            if (!Directory.Exists(ClassFileLocal))
            {
                Directory.CreateDirectory(ClassFileLocal);
            }
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

        public static void StorageDll(string UUID, List<NotesSDK> obj)
        {
            var url = DllFileLocal + UUID + ".json";
            Storage(url, obj);
        }
        public static void StorageClass(string UUID, List<NotesSDK> obj)
        {
            var url = ClassFileLocal + UUID + ".json";
            Storage(url, obj);
        }
    }
}
