using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.DllLoad;

internal class LoadClass
{
    public static GenReOBJ Load(string uuid, Stream ms, Stream pdb = null) 
    {
        var AssemblySave = new DllBuildSave(DllType.Class, uuid);
        AssemblySave.LoadFromStream(ms, pdb);
        var list = AssemblySave.Assemblies.First()
                       .GetTypes().Where(x => x.Name == uuid);

        if (!list.Any())
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Class[{uuid}]类名错误"
            };

        AssemblySave.DllType = list.First();
        var listM = AssemblySave.DllType.GetMethods();
        List<NotesSDK> obj = new();
        foreach (var item in listM)
        {
            var listA = item.GetCustomAttributes(true);
            foreach (var item1 in listA)
            {
                if (item1 is NotesSDK)
                {
                    obj.Add(item1 as NotesSDK);
                }
            }
        }

        NoteFile.StorageClass(uuid, obj);
        DllStonge.AddClass(uuid, AssemblySave);

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
        FileInfo info = new(DllStonge.ClassLocal + name + ".dll");
        LoadFile(info);
    }
}
