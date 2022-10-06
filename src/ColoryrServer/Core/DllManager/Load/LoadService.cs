using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ColoryrServer.Core.DllManager.DllLoad;

internal static class LoadService
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
        ServerMain.LogOut($"正在加载Service[{uuid}]");
        var assembly = new ServiceDllAssembly(CodeType.Service, uuid);
        assembly.LoadFromStream(ms, pdb);
        var list = assembly.Assemblies.First()
                       .GetTypes().Where(x => x.GetCustomAttribute<ServiceIN>(true) != null);

        if (!list.Any())
        {
            ServerMain.LogWarn($"加载Service[{uuid}]错误");
            return new GenReOBJ
            {
                Isok = false,
                Res = $"Service[{uuid}]类名错误"
            };
        }

        assembly.SelfType = list.First();
        ServiceIN service = assembly.SelfType.GetCustomAttribute<ServiceIN>(true);
        assembly.AutoStart = service.AutoStart;
        assembly.ServiceType = service.ServiceType;

        foreach (var item in assembly.SelfType.GetMethods())
        {
            if (item.IsPublic && (item.Name is CodeDemo.ServiceRun or CodeDemo.ServiceStart
                or CodeDemo.ServiceStop or CodeDemo.ServiceError or CodeDemo.ServicePerBuild
                or CodeDemo.ServicePostBuild))
            {
                assembly.MethodInfos.Add(item.Name, item);
            }
        }

        switch (assembly.ServiceType)
        {
            case ServiceType.ErrorDump:
                if (!assembly.MethodInfos.ContainsKey(CodeDemo.ServiceError))
                {
                    ServerMain.LogWarn($"加载Service[{uuid}]错误");
                    return new GenReOBJ
                    {
                        Isok = false,
                        Res = $"Service[{uuid}]不存在方法"
                    };
                }
                break;
            case ServiceType.Builder:
                if (!assembly.MethodInfos.ContainsKey(CodeDemo.ServicePerBuild)
                    || !assembly.MethodInfos.ContainsKey(CodeDemo.ServicePostBuild))
                {
                    ServerMain.LogWarn($"加载Service[{uuid}]错误");
                    return new GenReOBJ
                    {
                        Isok = false,
                        Res = $"Service[{uuid}]不存在方法"
                    };
                }
                break;
            case ServiceType.Normal:
                if (!assembly.MethodInfos.ContainsKey(CodeDemo.ServiceRun)
                    || !assembly.MethodInfos.ContainsKey(CodeDemo.ServiceStart)
                    || !assembly.MethodInfos.ContainsKey(CodeDemo.ServiceStop))
                {
                    ServerMain.LogWarn($"加载Service[{uuid}]错误");
                    return new GenReOBJ
                    {
                        Isok = false,
                        Res = $"Service[{uuid}]不存在方法"
                    };
                }
                break;
            case ServiceType.OnlyOpen:
                if (!assembly.MethodInfos.ContainsKey(CodeDemo.ServiceStart)
                    || !assembly.MethodInfos.ContainsKey(CodeDemo.ServiceStop))
                {
                    ServerMain.LogWarn($"加载Service[{uuid}]错误");
                    return new GenReOBJ
                    {
                        Isok = false,
                        Res = $"Service[{uuid}]不存在方法"
                    };
                }
                break;
        }

        AssemblyList.AddService(uuid, assembly);

        ServerMain.LogOut($"加载Service[{uuid}]完成");

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
