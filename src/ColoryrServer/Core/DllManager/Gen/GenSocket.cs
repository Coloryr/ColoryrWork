using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrWork.Lib.Build.Object;
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
        var build = GenCode.StartGen(obj.UUID, new()
        {
            CSharpSyntaxTree.ParseText(obj.Code)
        });
        obj.UpdateTime = DateTime.Now.ToString();
        CodeFileManager.StorageSocket(obj,user);
        if (!build.Isok)
        {
            build.Res = $"Socket[{obj.UUID}]" + build.Res;
            return build;
        }

        build.MS.Seek(0, SeekOrigin.Begin);
        build.MSPdb.Seek(0, SeekOrigin.Begin);

        var res = LoadSocket.Load(obj.UUID, build.MS, build.MSPdb);
        if (res != null)
            return res;

        Task.Factory.StartNew(() =>
        {
            build.MS.Seek(0, SeekOrigin.Begin);
            build.MSPdb.Seek(0, SeekOrigin.Begin);

            using (var FileStream = new FileStream(
                DllStongeManager.LocalSocket + obj.UUID + ".dll", FileMode.OpenOrCreate))
            {
                FileStream.Write(build.MS.ToArray());
                FileStream.Flush();
            }

            using (var FileStream = new FileStream(
                DllStongeManager.LocalSocket + obj.UUID + ".pdb", FileMode.OpenOrCreate))
            {
                FileStream.Write(build.MSPdb.ToArray());
                FileStream.Flush();
            }

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
