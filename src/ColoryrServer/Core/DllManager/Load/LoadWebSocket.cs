﻿using ColoryrServer.Core.DllManager.Gen;
using ColoryrWork.Lib.Build.Object;
using System.IO;
using System.Linq;

namespace ColoryrServer.Core.DllManager.DllLoad;

internal class LoadWebSocket
{
    public static GenReOBJ Load(string uuid, Stream ms, Stream pdb = null)
    {
        var AssemblySave = new DllAssembly(CodeType.WebSocket, uuid);
        AssemblySave.LoadFromStream(ms, pdb);
        var list = AssemblySave.Assemblies.First()
                       .GetTypes().Where(x => x.Name == uuid);

        if (!list.Any())
            return new GenReOBJ
            {
                Isok = false,
                Res = $"WebSocket[{uuid}]类名错误"
            };

        AssemblySave.SelfType = list.First();

        foreach (var item in AssemblySave.SelfType.GetMethods())
        {
            if (item.Name is CodeDemo.WebSocketMessage or CodeDemo.WebSocketOpen or CodeDemo.WebSocketClose)
                AssemblySave.MethodInfos.Add(item.Name, item);
        }

        if (AssemblySave.MethodInfos.Count == 0)
            return new GenReOBJ
            {
                Isok = false,
                Res = $"WebSocket[{uuid}]没有主方法"
            };

        DllStonge.AddWebSocket(uuid, AssemblySave);

        return null;
    }
    public static void LoadFile(FileInfo FileItem)
    {
        using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
        string uuid = FileItem.Name.Replace(".dll", "");
        ServerMain.LogOut("加载WebSocket：" + uuid);

        var pdb = FileItem.FullName.Replace(".dll", ".pdb");
        if (File.Exists(pdb))
        {
            using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
            Load(uuid, FileStream, FileStream1);
        }
        else
            Load(uuid, FileStream);
    }

    public static void Reload(string name)
    {
        FileInfo info = new(DllStonge.LocalWebSocket + name + ".dll");
        LoadFile(info);
    }
}
