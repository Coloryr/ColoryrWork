using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ColoryrServer.Core.DllManager.DllLoad;

internal class LoadDll
{
    public static GenReOBJ Load(string uuid, Stream ms, Stream pdb = null)
    {
        var AssemblySave = new DllAssembly(CodeType.Dll, uuid);
        AssemblySave.LoadFromStream(ms, pdb);
        var list = AssemblySave.Assemblies.First().GetTypes()
                       .Where(x => x.Name == "app_" + uuid);

        if (!list.Any())
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Dll[{uuid}]类名错误"
            };

        AssemblySave.SelfType = list.First();

        foreach (var item in AssemblySave.SelfType.GetMethods())
        {
            if (item.Name is "GetType" or "ToString" or "debug"
                or "Equals" or "GetHashCode" || !item.IsPublic)
                continue;
            AssemblySave.MethodInfos.Add(item.Name, item);
        }

        if (AssemblySave.MethodInfos.Count == 0)
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Dll[{uuid}]没有方法"
            };

        if (!AssemblySave.MethodInfos.ContainsKey(CodeDemo.DllMain))
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Dll[{uuid}]没有主方法"
            };

        try
        {
            List<NotesSDK> obj = new();
            foreach (var item in AssemblySave.MethodInfos.Values)
            {
                var listA = item.GetCustomAttributes(true);
                bool have = false;
                foreach (var item1 in listA)
                {
                    if (item1 is NotesSDK)
                    {
                        have = true;
                        obj.Add(item1 as NotesSDK);
                        break;
                    }
                }
                if (!have)
                {
                    return new GenReOBJ
                    {
                        Isok = false,
                        Res = $"Dll[{uuid}]的方法[{item}]没有注释"
                    };
                }
            }
            NoteFile.StorageDll(uuid, obj);
        }
        catch
        {
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Dll[{uuid}]注释出错"
            };
        }

        DllStonge.AddDll(uuid, AssemblySave);

        return null;
    }

    public static void LoadFile(FileInfo FileItem)
    {
        using var FileStream = new FileStream(FileItem.FullName, FileMode.Open, FileAccess.Read);
        string uuid = FileItem.Name.Replace(".dll", "");
        ServerMain.LogOut("加载DLL：" + uuid);

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
        FileInfo info = new(DllStonge.LocalDll + name + ".dll");
        LoadFile(info);
    }
}
