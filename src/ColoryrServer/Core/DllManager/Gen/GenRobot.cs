using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.Gen;

internal class GenRobot
{
    /// <summary>
    /// 编译Robot
    /// </summary>
    /// <param name="obj">构建信息</param>
    /// <returns>编译结果</returns>
    public static GenReOBJ StartGen(CSFileCode obj, string user)
    {
        ServerMain.LogOut($"开始编译Robot[{obj.UUID}]");
        bool release = obj.Code.Contains(@"//ColoryrServer_Release");
        var build = GenCode.StartGen(obj.UUID, new()
        {
            CSharpSyntaxTree.ParseText(obj.Code)
        }, release ? OptimizationLevel.Release : OptimizationLevel.Debug);
        obj.UpdateTime = DateTime.Now.ToString();
        CodeFileManager.StorageRobot(obj, user);
        if (!build.Isok)
        {
            build.Res = $"Robot[{obj.UUID}]" + build.Res;
            ServerMain.LogWarn($"编译Robot[{obj.UUID}]错误");
            return build;
        }

        build.MS.Seek(0, SeekOrigin.Begin);
        build.MSPdb.Seek(0, SeekOrigin.Begin);

        ServerMain.LogOut($"编译Robot[{obj.UUID}]完成");

        var res = LoadRobot.Load(obj.UUID, build.MS, build.MSPdb);
        if (res != null)
            return res;

        Task.Run(() =>
        {
            build.MS.Seek(0, SeekOrigin.Begin);
            build.MSPdb.Seek(0, SeekOrigin.Begin);

            FileSystem.Managers.DllFileManager.SaveRobot(obj.UUID, build);

            build.MSPdb.Close();
            build.MSPdb.Dispose();

            build.MS.Close();
            build.MS.Dispose();
            GC.Collect();
        });

        return new GenReOBJ
        {
            Isok = true,
            Res = $"Robot[{obj.UUID}]编译完成\n{build.Res}",
            Time = obj.UpdateTime
        };
    }
}
