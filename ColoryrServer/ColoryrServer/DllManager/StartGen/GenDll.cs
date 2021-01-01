using ColoryrServer.FileSystem;
using Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager
{
    class GenDll
    {
        public static GenReOBJ StartGen(CSFileCode CodeFile)
        {
            var Code = CSharpSyntaxTree.ParseText(CodeFile.Code);
            var Res = GenTask.StartGen(CodeFile.UUID, new List<SyntaxTree> { Code }, GenLib.Dll);
            if (!Res.Isok)
            {
                return Res;
            }

            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            var AssemblySave = new AssemblySave
            {
                Assembly = new AssemblyLoadContext(CodeFile.UUID, true)
            };
            AssemblySave.Assembly.LoadFromStream(Res.MS, Res.MSPdb);
            var list = AssemblySave.Assembly.Assemblies.First().GetTypes()
                           .Where(x => x.Name == "app_" + CodeFile.UUID);

            if (!list.Any())
                return new GenReOBJ
                {
                    Isok = false,
                    Res = "UUID错误"
                };

            AssemblySave.Type = list.First();
            foreach (var item in AssemblySave.Type.GetMethods())
            {
                if (item.Name == "Main" || item.Name == "GetType" || item.Name == "ToString"
                    || item.Name == "Equals" || item.Name == "GetHashCode")
                    continue;
                AssemblySave.MethodInfos.Add(item.Name, item);
            }

            if (AssemblySave.MethodInfos.Count == 0)
                return new GenReOBJ
                {
                    Isok = false,
                    Res = "没有方法"
                };

            DllStonge.AddDll(CodeFile.UUID, AssemblySave);

            Task.Run(() =>
            {
                Res.MS.Seek(0, SeekOrigin.Begin);
                Res.MSPdb.Seek(0, SeekOrigin.Begin);

                using (var FileStream = new FileStream(
                    DllStonge.DllLocal + CodeFile.UUID + ".dll", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MS.ToArray());
                    FileStream.Flush();
                }

                using (var FileStream = new FileStream(
                    DllStonge.DllLocal + CodeFile.UUID + ".pdb", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MSPdb.ToArray());
                    FileStream.Flush();
                }

                CSFile.StorageDll(CodeFile);
                Config.Save();

                Res.MSPdb.Close();
                Res.MSPdb.Dispose();

                Res.MS.Close();
                Res.MS.Dispose();
                GC.Collect();
            });

            return new GenReOBJ
            {
                Isok = true,
                Res = "编译完成"
            };
        }
    }
}
