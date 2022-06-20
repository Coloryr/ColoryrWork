using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ColoryrServer.Core.DllManager.DllLoad;

internal class LoadDll
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
        var assembly = new DllAssembly(CodeType.Dll, uuid);
        assembly.LoadFromStream(ms, pdb);
        var list = assembly.Assemblies.First().GetTypes()
                       .Where(x => x.GetCustomAttribute<DLLIN>(true) != null);

        if (!list.Any())
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Dll[{uuid}]类名错误"
            };

        assembly.SelfType = list.First();
        var attr = assembly.SelfType.GetCustomAttribute<DLLIN>(true);
        assembly.Debug = attr.Debug;

        foreach (var item in assembly.SelfType.GetMethods())
        {
            if (item.Name is "GetType" or "ToString" or "Equals" or "GetHashCode" 
                || !item.IsPublic)
                continue;
            assembly.MethodInfos.Add(item.Name, item);
        }

        if (assembly.MethodInfos.Count == 0)
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Dll[{uuid}]没有方法"
            };

        if (!assembly.MethodInfos.ContainsKey(CodeDemo.DllMain))
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Dll[{uuid}]没有主方法"
            };

        try
        {
            List<NotesSDK> obj = new();
            foreach (var item in assembly.MethodInfos.Values)
            {
                var listA = item.GetCustomAttribute<NotesSDK>(true);
                if (listA == null)
                {
                    return new GenReOBJ
                    {
                        Isok = false,
                        Res = $"Dll[{uuid}]的方法[{item}]没有注释"
                    };
                }
                else
                {
                    obj.Add(listA);
                }
            }
            NoteFile.StorageDll(EnCode.SHA1(uuid), obj);
        }
        catch
        {
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Dll[{uuid}]注释出错"
            };
        }

        DllStongeManager.AddDll(uuid, assembly);

        return null;
    }

    /// <summary>
    /// 从文件加载.dll
    /// </summary>
    /// <param name="info">文件信息</param>
    public static void LoadFile(string item, string local)
    {
        using var FileStream = new FileStream(local, FileMode.Open, FileAccess.Read);
        ServerMain.LogOut("加载DLL：" + item);

        var pdb = local.Replace(".dll", ".pdb");
        if (File.Exists(pdb))
        {
            using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
            Load(item, FileStream, FileStream1);
        }
        else
            Load(item, FileStream);
    }

    /// <summary>
    /// 重载.dll
    /// </summary>
    /// <param name="name">文件名字</param>
    public static void Reload(string item)
    {
        LoadFile(item, DllStongeManager.LocalDll + EnCode.SHA1(item) + ".dll");
    }
}
