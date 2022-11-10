using ColoryrServer.Core.Dll;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.Core.Utils;
using ColoryrServer.SDK;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;

namespace ColoryrServer.Core.FileSystem;

public class PackageContext : AssemblyLoadContext
{
    public PackageContext(string dir)
    {
        LoadFromAssemblyPath(dir);
    }
}

public class PackageObj
{
    private PackageContext context;
    public static PackageObj Load(string path)
    {
        var obj = new PackageObj();
        obj.dir = path;
        obj.context = new(path);

        return obj;
    }

    public string dir;
}

public record PackageInfo
{
    public Dictionary<string, string> Dlls = new();
}

internal static class ServerPackage
{
    private static Dictionary<string, PackageContext> PackageContexts = new();
    private static string PackageDir = ServerMain.RunLocal + "Packages/";

    private static bool StartBuild = false;

    public static void Start()
    {
        if (!Directory.Exists(PackageDir))
        {
            Directory.CreateDirectory(PackageDir);
        }
    }

    public static void MakePackage()
    {
        if (StartBuild)
            return;

        StartBuild = true;
        try
        {
            string dir = ServerMain.RunLocal + "pack.zip";
            ServerMain.LogOut("开始打包服务器资源");
            var info = new PackageInfo();

            Crc32 crc32 = new();
            using ZipOutputStream stream = new(new FileStream(dir,
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None));

            stream.SetLevel(5);

            var list = AssemblyList.GetDll();
            DirectoryInfo dinfo = new(DllFileManager.LocalDll);
            var list1 = dinfo.GetFiles();
            ZipEntry entry;
            byte[] data;

            foreach (var item in list)
            {
                string name = EnCode.SHA1(item.Name);
                info.Dlls.Add(name, item.Name);
            }

            foreach (var item in list1)
            {
                data = FileUtils.LoadBytes(item.FullName);
                crc32.Reset();
                crc32.Update(data);
                entry = new("Dll/Dll/" + item.Name)
                {
                    Size = item.Length,
                    Crc = crc32.Value
                };

                stream.PutNextEntry(entry);
                stream.Write(data);
                stream.CloseEntry();
            }

            dinfo = new(DllFileManager.LocalClass);
            list1 = dinfo.GetFiles();

            foreach (var item in list1)
            {
                data = FileUtils.LoadBytes(item.FullName);
                crc32.Reset();
                crc32.Update(data);
                entry = new("Dll/Class/" + item.Name)
                {
                    Size = item.Length,
                    Crc = crc32.Value
                };

                stream.PutNextEntry(entry);
                stream.Write(data);
                stream.CloseEntry();
            }

            dinfo = new(DllFileManager.LocalMqtt);
            list1 = dinfo.GetFiles();

            foreach (var item in list1)
            {
                data = FileUtils.LoadBytes(item.FullName);
                crc32.Reset();
                crc32.Update(data);
                entry = new("Dll/Mqtt/" + item.Name)
                {
                    Size = item.Length,
                    Crc = crc32.Value
                };

                stream.PutNextEntry(entry);
                stream.Write(data);
                stream.CloseEntry();
            }

            dinfo = new(DllFileManager.LocalSocket);
            list1 = dinfo.GetFiles();

            foreach (var item in list1)
            {
                data = FileUtils.LoadBytes(item.FullName);
                crc32.Reset();
                crc32.Update(data);
                entry = new("Dll/Socket/" + item.Name)
                {
                    Size = item.Length,
                    Crc = crc32.Value
                };

                stream.PutNextEntry(entry);
                stream.Write(data);
                stream.CloseEntry();
            }

            dinfo = new(DllFileManager.LocalRobot);
            list1 = dinfo.GetFiles();

            foreach (var item in list1)
            {
                data = FileUtils.LoadBytes(item.FullName);
                crc32.Reset();
                crc32.Update(data);
                entry = new("Dll/Robot/" + item.Name)
                {
                    Size = item.Length,
                    Crc = crc32.Value
                };

                stream.PutNextEntry(entry);
                stream.Write(data);
                stream.CloseEntry();
            }

            dinfo = new(DllFileManager.LocalWebSocket);
            list1 = dinfo.GetFiles();

            foreach (var item in list1)
            {
                data = FileUtils.LoadBytes(item.FullName);
                crc32.Reset();
                crc32.Update(data);
                entry = new("Dll/WebSocket/" + item.Name)
                {
                    Size = item.Length,
                    Crc = crc32.Value
                };

                stream.PutNextEntry(entry);
                stream.Write(data);
                stream.CloseEntry();
            }

            dinfo = new(DllFileManager.LocalService);
            list1 = dinfo.GetFiles();

            foreach (var item in list1)
            {
                data = FileUtils.LoadBytes(item.FullName);
                crc32.Reset();
                crc32.Update(data);
                entry = new("Dll/Service/" + item.Name)
                {
                    Size = item.Length,
                    Crc = crc32.Value
                };

                stream.PutNextEntry(entry);
                stream.Write(data);
                stream.CloseEntry();
            }

            string data1 = Tools.ToJson(info);
            data = Encoding.UTF8.GetBytes(data1);
            crc32.Reset();
            crc32.Update(data);

            entry = new("info.json")
            {
                Size = data.Length,
                Crc = crc32.Value
            };

            stream.PutNextEntry(entry);
            stream.Write(data);
            stream.CloseEntry();

            ServerMain.LogOut($"服务器资源打包完成，资源储存在{dir}");
        }
        catch (Exception e)
        {
            ServerMain.LogError("服务器资源打包失败", e);
        }
        StartBuild = false;
    }
}
