using ColoryrServer.DllManager.StartGen.GenUtils;
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

namespace ColoryrServer.DllManager.StartGen.GenType
{
    internal class GenDll
    {
        public static GenReOBJ StartGen(CSFileCode CodeFile)
        {
            var Res = GenTask.StartGen(CodeFile.UUID, new List<SyntaxTree> 
            {
                CSharpSyntaxTree.ParseText(CodeFile.Code) 
            }, GenLib.Dll);
            Task.Run(() => CSFile.StorageDll(CodeFile));
            if (!Res.Isok)
            {
                Res.Res = $"Dll[{CodeFile.UUID}]" + Res.Res;
                return Res;
            }

            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            var AssemblySave = new DllBuildSave
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
                    Res = $"Dll[{CodeFile.UUID}]没有主类"
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
                    Res = $"Dll[{CodeFile.UUID}]没有方法"
                };

            DllStonge.AddDll(CodeFile.UUID, AssemblySave);
            var time = string.Format("{0:s}", DateTime.Now);
            CodeFile.UpdataTime = time;
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

                Res.MSPdb.Close();
                Res.MSPdb.Dispose();

                Res.MS.Close();
                Res.MS.Dispose();
                GC.Collect();
            });

            return new GenReOBJ
            {
                Isok = true,
                Res = $"Dll[{CodeFile.UUID}]编译完成",
                Time = CodeFile.UpdataTime
            };
        }
    }
}
