using ColoryrServer.FileSystem;
using Lib.Build.Object;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace ColoryrServer.DllManager
{
    class GenIoT
    {
        public static GenReOBJ StartGen(CSFileCode File)
        {
            var Res = GenTask.StartGen(File.UUID, new()
            { 
                CSharpSyntaxTree.ParseText(File.Code) 
            }, GenLib.Dll);
            if (!Res.Isok)
            {
                return Res;
            }
            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            var AssemblySave = new AssemblySave
            {
                Assembly = new AssemblyLoadContext(File.UUID, true)
            };
            AssemblySave.Assembly.LoadFromStream(Res.MS, Res.MSPdb);
            var list = AssemblySave.Assembly.Assemblies.First()
                           .GetTypes().Where(x => x.Name == File.UUID);

            if (!list.Any())
                return new GenReOBJ
                {
                    Isok = false,
                    Res = "类名错误"
                };

            AssemblySave.Type = list.First();

            foreach (var Item in AssemblySave.Type.GetMethods())
            {
                if (Item.Name == "main")
                    AssemblySave.MethodInfos.Add(Item.Name, Item);
            }

            if (AssemblySave.MethodInfos.Count == 0)
                return new GenReOBJ
                {
                    Isok = false,
                    Res = "没有主方法"
                };

            DllStonge.AddIoT(File.UUID, AssemblySave);

            Task.Factory.StartNew(() =>
            {
                Res.MS.Seek(0, SeekOrigin.Begin);
                Res.MSPdb.Seek(0, SeekOrigin.Begin);

                using (var FileStream = new FileStream(
                    DllStonge.IoTLocal + File.UUID + ".dll", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MS.ToArray());
                    FileStream.Flush();
                }

                using (var FileStream = new FileStream(
                    DllStonge.IoTLocal + File.UUID + ".pdb", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MSPdb.ToArray());
                    FileStream.Flush();
                }

                CSFile.StorageIoT(File);
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
