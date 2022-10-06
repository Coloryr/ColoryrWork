using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.SDK;
using System;
using System.IO;

namespace ColoryrServer.Core.FileSystem.Managers;

internal static class FileDllManager
{
    public static readonly string LocalDll = ServerMain.RunLocal + "Dll/Dll/";
    public static readonly string LocalClass = ServerMain.RunLocal + "Dll/Class/";
    public static readonly string LocalSocket = ServerMain.RunLocal + "Dll/Socket/";
    public static readonly string LocalWebSocket = ServerMain.RunLocal + "Dll/WebSocket/";
    public static readonly string LocalRobot = ServerMain.RunLocal + "Dll/Robot/";
    public static readonly string LocalMqtt = ServerMain.RunLocal + "Dll/Mqtt/";
    public static readonly string LocalService = ServerMain.RunLocal + "Dll/Service/";

    private static void Save(string name, GenReOBJ build)
    {
        using var steam1 = new FileStream(name + ".dll", FileMode.OpenOrCreate);
        steam1.Write(build.MS.ToArray());
        steam1.Flush();

        using var stream2 = new FileStream(name + ".pdb", FileMode.OpenOrCreate);
        stream2.Write(build.MSPdb.ToArray());
        stream2.Flush();
    }

    public static void SaveDll(string name, GenReOBJ build)
    {
        Save(LocalDll + name, build);
    }

    public static void SaveMqtt(string name, GenReOBJ build)
    {
        Save(LocalMqtt + name, build);
    }

    public static void SaveService(string name, GenReOBJ build)
    {
        Save(LocalService + name, build);
    }

    public static void SaveRobot(string name, GenReOBJ build)
    {
        Save(LocalRobot + name, build);
    }

    public static void SaveWebSocket(string name, GenReOBJ build)
    {
        Save(LocalWebSocket + name, build);
    }

    public static void SaveSocket(string name, GenReOBJ build)
    {
        Save(LocalSocket + name, build);
    }

    public static void SaveClass(string name, GenReOBJ build)
    {
        Save(LocalClass + name, build);
    }

    public static void RemoveDll(string uuid)
    {
        RemoveAll(LocalDll + EnCode.SHA1(uuid));
    }

    public static void RemoveClass(string uuid)
    {
        RemoveAll(LocalClass + uuid);
    }

    public static void RemoveSocket(string uuid)
    {
        RemoveAll(LocalSocket + uuid);
    }

    public static void RemoveWebSocket(string uuid)
    {
        RemoveAll(LocalWebSocket + uuid);
    }

    public static void RemoveRobot(string uuid)
    {
        RemoveAll(LocalRobot + uuid);
    }

    public static void RemoveMqtt(string uuid)
    {
        RemoveAll(LocalMqtt + uuid);
    }

    public static void RemoveService(string uuid)
    {
        RemoveAll(LocalRobot + uuid);
    }

    public static void Start()
    {
        if (!Directory.Exists(LocalDll))
        {
            Directory.CreateDirectory(LocalDll);
        }
        if (!Directory.Exists(LocalClass))
        {
            Directory.CreateDirectory(LocalClass);
        }
        if (!Directory.Exists(LocalSocket))
        {
            Directory.CreateDirectory(LocalSocket);
        }
        if (!Directory.Exists(LocalWebSocket))
        {
            Directory.CreateDirectory(LocalWebSocket);
        }
        if (!Directory.Exists(LocalRobot))
        {
            Directory.CreateDirectory(LocalRobot);
        }
        if (!Directory.Exists(LocalMqtt))
        {
            Directory.CreateDirectory(LocalMqtt);
        }
        if (!Directory.Exists(LocalService))
        {
            Directory.CreateDirectory(LocalService);
        }
        if (!Directory.Exists(LocalMqtt))
        {
            Directory.CreateDirectory(LocalMqtt);
        }

        //加载class
        var dirs = Directory.GetFiles(LocalClass);
        foreach (var item in dirs)
        {
            try
            {
                if (item.Contains(".pdb"))
                    continue;
                LoadClass.LoadFile(item);
                GenCode.LoadClass(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        //加载dll
        foreach (var item in CodeFileManager.DllFileList.Keys)
        {
            string name = LocalDll + EnCode.SHA1(item) + ".dll";
            if (File.Exists(name))
            {
                try
                {
                    LoadDll.LoadFile(item, name);
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
        }

        dirs = Directory.GetFiles(LocalSocket);
        foreach (var item in dirs)
        {
            try
            {
                if (item.Contains(".pdb"))
                    continue;
                LoadSocket.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Directory.GetFiles(LocalWebSocket);
        foreach (var item in dirs)
        {
            try
            {
                if (item.Contains(".pdb"))
                    continue;
                LoadWebSocket.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Directory.GetFiles(LocalRobot);
        foreach (var item in dirs)
        {
            try
            {
                if (item.Contains(".pdb"))
                    continue;
                LoadRobot.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Directory.GetFiles(LocalMqtt);
        foreach (var item in dirs)
        {
            try
            {
                if (item.Contains(".pdb"))
                    continue;
                LoadMqtt.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
        dirs = Directory.GetFiles(LocalService);
        foreach (var item in dirs)
        {
            try
            {
                if (item.Contains(".pdb"))
                    continue;
                LoadService.LoadFile(item);
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }
    }

    private static void RemoveAll(string dir)
    {
        if (File.Exists(dir + ".dll"))
        {
            File.Delete(dir + ".dll");
        }
        if (File.Exists(dir + ".pdb"))
        {
            File.Delete(dir + ".pdb");
        }
    }

    public static FileInfo GetClassLocal(string name)
    {
        return new(LocalClass + name + ".dll");
    }
}
