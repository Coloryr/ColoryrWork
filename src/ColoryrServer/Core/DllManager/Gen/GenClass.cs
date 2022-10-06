using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.Core.FileSystem.Database;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.Gen;

internal class GenClass
{
    /// <summary>
    /// 编译Class
    /// </summary>
    /// <param name="obj">构建信息</param>
    /// <returns>编译结果</returns>
    public static GenReOBJ StartGen(CSFileCode obj)
    {
        ServerMain.LogOut($"开始编译Class[{obj.UUID}]");
        bool release = false;
        var build = GenCode.StartGen(obj.UUID,
            CodeDatabase.GetClassCode(obj.UUID).Select(a =>
            {
                if (!release)
                    release = a.code.Contains(@"//ColoryrServer_Release");
                return CSharpSyntaxTree.ParseText(a.code, path: a.name, encoding: Encoding.UTF8);
            }).ToList(),
            release ? OptimizationLevel.Release : OptimizationLevel.Debug,
            true);
        obj.UpdateTime = DateTime.Now.ToString();
        if (!build.Isok)
        {
            build.Res = $"Class[{obj.UUID}]" + build.Res;
            ServerMain.LogWarn($"编译Class[{obj.UUID}]错误");
            return build;
        }

        build.MS.Seek(0, SeekOrigin.Begin);
        build.MSPdb.Seek(0, SeekOrigin.Begin);

        ServerMain.LogOut($"编译Class[{obj.UUID}]完成");

        var res = LoadClass.Load(obj.UUID, build.MS, build.MSPdb);
        if (res != null)
            return res;

        //保存文件
        Task.Run(() =>
        {
            build.MS.Seek(0, SeekOrigin.Begin);
            build.MSPdb.Seek(0, SeekOrigin.Begin);

            using (var FileStream = new FileStream(
                FileSystem.Managers.DllFileManager.LocalClass + obj.UUID + ".dll", FileMode.OpenOrCreate))
            {
                FileStream.Write(build.MS.ToArray());
                FileStream.Flush();
            }

            using (var FileStream = new FileStream(
                FileSystem.Managers.DllFileManager.LocalClass + obj.UUID + ".pdb", FileMode.OpenOrCreate))
            {
                FileStream.Write(build.MSPdb.ToArray());
                FileStream.Flush();
            }

            build.MSPdb.Close();
            build.MSPdb.Dispose();

            build.MS.Close();
            build.MS.Dispose();

            GenCode.LoadClass(FileSystem.Managers.DllFileManager.LocalClass + obj.UUID + ".dll");

            GC.Collect();
        });

        return new GenReOBJ
        {
            Isok = true,
            Res = $"Class[{obj.UUID}]编译完成\n{build.Res}",
            Time = string.Format("{0:s}", DateTime.Now)
        };
    }
}
