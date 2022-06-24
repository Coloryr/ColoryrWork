using ColoryrServer.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.Core.FileSystem;

internal static class NoteFile
{
    private static readonly string DllFileLocal = ServerMain.RunLocal + "Notes/Dll/";
    private static readonly string ClassFileLocal = ServerMain.RunLocal + "Notes/Class/";

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

    public static void StorageDll(string uuid, List<NotesSDK> obj)
    {
        Storage($"{DllFileLocal}{uuid}.json", obj);
    }
    public static void StorageClass(string uuid, List<NotesSDK> obj)
    {
        Storage($"{ClassFileLocal}{uuid}.json", obj);
    }
}
