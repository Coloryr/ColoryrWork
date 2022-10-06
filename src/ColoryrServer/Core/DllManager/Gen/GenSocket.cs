using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.Gen;

internal static class GenSocket
{
    /// <summary>
    /// 编译Socket
    /// </summary>
    /// <param name="obj">构建信息</param>
    /// <returns>编译结果</returns>
    public static GenReOBJ StartGen(CSFileCode obj, string user)
    {
        ServerMain.LogOut($"开始编译Socket[{obj.UUID}]");
        bool release = obj.Code.Contains(@"//ColoryrServer_Release");
        var build = GenCode.StartGen(obj.UUID, new()
        {
            CSharpSyntaxTree.ParseText(obj.Code)
        }, release ? OptimizationLevel.Release : OptimizationLevel.Debug);
        obj.UpdateTime = DateTime.Now.ToString();
        CodeFileManager.StorageSocket(obj, user);
        if (!build.Isok)
        {
            build.Res = $"Socket[{obj.UUID}]" + build.Res;
            ServerMain.LogWarn($"编译Socket[{obj.UUID}]错误");
            return build;
        }

        build.MS.Seek(0, SeekOrigin.Begin);
        build.MSPdb.Seek(0, SeekOrigin.Begin);

        ServerMain.LogOut($"编译Socket[{obj.UUID}]完成");

        var res = LoadSocket.Load(obj.UUID, build.MS, build.MSPdb);
        if (res != null)
            return res;

        Task.Run(() =>
        {
            build.MS.Seek(0, SeekOrigin.Begin);
            build.MSPdb.Seek(0, SeekOrigin.Begin);

            DllFileManager.SaveSocket(obj.UUID, build);

            build.MSPdb.Close();
            build.MSPdb.Dispose();

            build.MS.Close();
            build.MS.Dispose();

            GC.Collect();
        });

        return new GenReOBJ
        {
            Isok = true,
            Res = $"Socket[{obj.UUID}]编译完成\n{build.Res}",
            Time = obj.UpdateTime
        };
    }
}
