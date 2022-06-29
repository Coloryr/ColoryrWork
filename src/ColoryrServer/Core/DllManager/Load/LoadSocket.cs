﻿using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.PortServer;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ColoryrServer.Core.DllManager.DllLoad;

internal static class LoadSocket
{
    /// <summary>
    /// 加载并验证.dll
    /// </summary>
    /// <param name="uuid">UUID</param>
    /// <param name="ms">.dll文件</param>
    /// <param name="pdb">.psd文件</param>
    /// <returns>验证信息</returns>
    public static GenReOBJ Load(string uuid, Stream ms, Stream pdb = null)
    {
        var assembly = new DllAssembly(CodeType.Socket, uuid);
        assembly.LoadFromStream(ms, pdb);
        var list = assembly.Assemblies.First()
                       .GetTypes().Where(x => x.GetCustomAttribute<DLLIN>(true) != null);

        if (!list.Any())
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Socket[{uuid}]类名错误"
            };

        assembly.SelfType = list.First();

        if (!PortNettyManager.Check(assembly.SelfType))
        {
            foreach (var item in assembly.SelfType.GetMethods())
            {
                if (item.Name is CodeDemo.SocketTcp or CodeDemo.SocketUdp && item.IsPublic)
                    assembly.MethodInfos.Add(item.Name, item);
            }

            if (assembly.MethodInfos.Count == 0)
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Socket[{uuid}]没有方法"
                };
        }
        else
        {
            assembly.MethodInfos.Add("Netty", null);
        }

        DllStongeManager.AddSocket(uuid, assembly);

        return null;
    }

    /// <summary>
    /// 从文件加载.dll
    /// </summary>
    /// <param name="info">文件信息</param>
    public static void LoadFile(FileInfo info)
    {
        using var FileStream = new FileStream(info.FullName, FileMode.Open, FileAccess.Read);
        string uuid = info.Name.Replace(".dll", "");
        ServerMain.LogOut("加载Socket：" + uuid);

        var pdb = info.FullName.Replace(".dll", ".pdb");
        if (File.Exists(pdb))
        {
            using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
            Load(uuid, FileStream, FileStream1);
        }
        else
            Load(uuid, FileStream);
    }

    /// <summary>
    /// 重载.dll
    /// </summary>
    /// <param name="name">文件名字</param>
    public static void Reload(string name)
    {
        FileInfo info = new(DllStongeManager.LocalSocket + name + ".dll");
        LoadFile(info);
    }
}
