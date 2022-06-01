using ColoryrServer.Core.DllManager.DllLoad;
using ColoryrServer.Core.FileSystem;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.Gen;

internal class GenTask
{
    public static GenReOBJ StartGen(CSFileCode File)
    {
        var Res = GenCode.StartGen(File.UUID, new List<SyntaxTree>
        {
            CSharpSyntaxTree.ParseText(File.Code)
        });
        Task.Run(() => CodeFile.StorageTask(File));
        if (!Res.Isok)
        {
            Res.Res = $"Task[{File.UUID}]" + Res.Res;
            return Res;
        }

        Res.MS.Seek(0, SeekOrigin.Begin);
        Res.MSPdb.Seek(0, SeekOrigin.Begin);

        var res = LoadTask.Load(File.UUID, Res.MS, Res.MSPdb);
        if (res != null)
            return res;

        Task.Run(() =>
        {
            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            using (var FileStream = new FileStream(
                DllStonge.LocalTask + File.UUID + ".dll", FileMode.OpenOrCreate))
            {
                FileStream.Write(Res.MS.ToArray());
                FileStream.Flush();
            }

            using (var FileStream = new FileStream(
                DllStonge.LocalTask + File.UUID + ".pdb", FileMode.OpenOrCreate))
            {
                FileStream.Write(Res.MSPdb.ToArray());
                FileStream.Flush();
            }

            Res.MSPdb.Close();
            Res.MSPdb.Dispose();

            Res.MS.Close();
            Res.MS.Dispose();
            GC.Collect();
        });

        return new GenReOBJ
        {
            Isok = true,
            Res = $"Task[{File.UUID}]编译完成",
            Time = File.UpdateTime
        };
    }
}
