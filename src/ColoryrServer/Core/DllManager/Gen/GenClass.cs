using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.Gen;

internal class GenClass
{
    public static GenReOBJ StartGen(CSFileCode obj)
    {
        var Res = GenCode.StartGen(obj.UUID,
            CodeFileManager.GetClassCode(obj.UUID).Select(a =>
            CSharpSyntaxTree.ParseText(a.Code)).ToList());
        if (!Res.Isok)
        {
            Res.Res = $"Class[{obj.UUID}]" + Res.Res;
            return Res;
        }

        Res.MS.Seek(0, SeekOrigin.Begin);
        Res.MSPdb.Seek(0, SeekOrigin.Begin);

        var res = LoadClass.Load(obj.UUID, Res.MS, Res.MSPdb);
        if (res != null)
            return res;

        Task.Run(() =>
        {
            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            using (var FileStream = new FileStream(
                DllStonge.LocalClass + obj.UUID + ".dll", FileMode.OpenOrCreate))
            {
                FileStream.Write(Res.MS.ToArray());
                FileStream.Flush();
            }

            using (var FileStream = new FileStream(
                DllStonge.LocalClass + obj.UUID + ".pdb", FileMode.OpenOrCreate))
            {
                FileStream.Write(Res.MSPdb.ToArray());
                FileStream.Flush();
            }

            Res.MSPdb.Close();
            Res.MSPdb.Dispose();

            Res.MS.Close();
            Res.MS.Dispose();

            GenCode.LoadClass(DllStonge.LocalClass + obj.UUID + ".dll");

            GC.Collect();
        });

        return new GenReOBJ
        {
            Isok = true,
            Res = $"Class[{obj.UUID}]编译完成",
            Time = string.Format("{0:s}", DateTime.Now)
        };
    }
}
