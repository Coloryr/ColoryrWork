using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ColoryrServer.Core.DllManager.DllLoad;

internal static class LoadWebSocket
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
        ServerMain.LogOut($"正在加载WebSocket[{uuid}]");
        var assembly = new DllAssembly(CodeType.WebSocket, uuid);
        assembly.LoadFromStream(ms, pdb);
        var list = assembly.Assemblies.First()
                       .GetTypes().Where(x => x.GetCustomAttribute<WebSocketIN>(true) != null);

        if (!list.Any())
        {
            ServerMain.LogWarn($"加载WebSocket[{uuid}]错误");
            return new GenReOBJ
            {
                Isok = false,
                Res = $"WebSocket[{uuid}]类错误"
            };
        }

        assembly.SelfType = list.First();

        foreach (var item in assembly.SelfType.GetMethods())
        {
            if (item.IsPublic && (item.Name is CodeDemo.WebSocketMessage or
                CodeDemo.WebSocketOpen or CodeDemo.WebSocketClose))
                assembly.MethodInfos.Add(item.Name, item);
        }

        AssemblyList.AddWebSocket(uuid, assembly);

        ServerMain.LogOut($"加载WebSocket[{uuid}]完成");

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
        LoadFile(FileSystem.Managers.DllFileManager.LocalService + item + ".dll");
    }
}
