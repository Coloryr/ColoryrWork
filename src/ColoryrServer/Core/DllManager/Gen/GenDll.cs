﻿using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.Gen;

internal class GenDll
{
    public static GenReOBJ StartGen(CSFileCode File)
    {
        var Res = GenCode.StartGen(File.UUID, new List<SyntaxTree>
        {
            CSharpSyntaxTree.ParseText(File.Code)
        });
        Task.Run(() => CSFile.StorageDll(File));
        if (!Res.Isok)
        {
            Res.Res = $"Dll[{File.UUID}]" + Res.Res;
            return Res;
        }

        Res.MS.Seek(0, SeekOrigin.Begin);
        Res.MSPdb.Seek(0, SeekOrigin.Begin);

        var res = Load(File.UUID, Res.MS, Res.MSPdb);
        if (res != null)
            return res;

        Task.Run(() =>
        {
            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            using (var FileStream = new FileStream(
                DllStonge.DllLocal + File.UUID + ".dll", FileMode.OpenOrCreate))
            {
                FileStream.Write(Res.MS.ToArray());
                FileStream.Flush();
            }

            using (var FileStream = new FileStream(
                DllStonge.DllLocal + File.UUID + ".pdb", FileMode.OpenOrCreate))
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
            Res = $"Dll[{File.UUID}]编译完成",
            Time = File.UpdataTime
        };
    }
}
