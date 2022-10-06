using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ColoryrServer.Core.DllManager.DllLoad;

internal static class LoadClass
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
        ServerMain.LogOut($"正在加载Class[{uuid}]");
        var assembly = new DllAssembly(CodeType.Class, uuid);
        assembly.LoadFromStream(ms, pdb);
        var list = assembly.Assemblies.First()
                       .GetTypes().Where(x => x.GetCustomAttribute<ClassIN>(true) != null);

        if (!list.Any())
        {
            ServerMain.LogWarn($"加载Class[{uuid}]错误");
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Class[{uuid}]类错误"
            };
        }

        assembly.SelfType = list.First();
        var listM = assembly.SelfType.GetMethods();
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
        AssemblyList.AddClass(uuid, assembly);

        ServerMain.LogOut($"加载Class[{uuid}]完成");

        return null;
    }

    /// <summary>
    /// 从文件加载.dll
    /// </summary>
    /// <param name="info">文件信息</param>
    public static void LoadFile(string local)
    {
        using var stream = new FileStream(local, FileMode.Open, FileAccess.Read);

        var pdb = local.Replace(".dll", ".pdb");
        if (File.Exists(pdb))
        {
            using var stream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
            Load(local, stream, stream1);
        }
        else
            Load(local, stream);
    }

    /// <summary>
    /// 重载.dll
    /// </summary>
    /// <param name="name">文件名字</param>
    public static void Reload(string item)
    {
        LoadFile(FileDllManager.LocalService + item + ".dll");
    }
}
