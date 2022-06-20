using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
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
        var build = GenCode.StartGen(obj.UUID,
            CodeFileManager.GetClassCode(obj.UUID).Select(a =>
            CSharpSyntaxTree.ParseText(a.code, path: a.name, encoding: Encoding.UTF8)).ToList());
        if (!build.Isok)
        {
            build.Res = $"Class[{obj.UUID}]" + build.Res;
            return build;
        }

        build.MS.Seek(0, SeekOrigin.Begin);
        build.MSPdb.Seek(0, SeekOrigin.Begin);

        var res = LoadClass.Load(obj.UUID, build.MS, build.MSPdb);
        if (res != null)
            return res;

        //保存文件
        Task.Run(() =>
        {
            build.MS.Seek(0, SeekOrigin.Begin);
            build.MSPdb.Seek(0, SeekOrigin.Begin);

            using (var FileStream = new FileStream(
                DllStongeManager.LocalClass + obj.UUID + ".dll", FileMode.OpenOrCreate))
            {
                FileStream.Write(build.MS.ToArray());
                FileStream.Flush();
            }

            using (var FileStream = new FileStream(
                DllStongeManager.LocalClass + obj.UUID + ".pdb", FileMode.OpenOrCreate))
            {
                FileStream.Write(build.MSPdb.ToArray());
                FileStream.Flush();
            }

            build.MSPdb.Close();
            build.MSPdb.Dispose();

            build.MS.Close();
            build.MS.Dispose();

            GenCode.LoadClass(DllStongeManager.LocalClass + obj.UUID + ".dll");

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
