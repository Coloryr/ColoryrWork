using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
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
        public static GenReOBJ StartGen(CSFileCode File)
        {
            var Res = GenTask.StartGen(File.UUID, new List<SyntaxTree>
            {
                CSharpSyntaxTree.ParseText(File.Code)
            }, GenLib.Dll);
            Task.Run(() => CSFile.StorageDll(File));
            if (!Res.Isok)
            {
                Res.Res = $"Dll[{File.UUID}]" + Res.Res;
                return Res;
            }

            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            var AssemblySave = new DllBuildSave
            {
                Assembly = new AssemblyLoadContext(File.UUID, true)
            };
            AssemblySave.Assembly.LoadFromStream(Res.MS, Res.MSPdb);
            var list = AssemblySave.Assembly.Assemblies.First().GetTypes()
                           .Where(x => x.Name == "app_" + File.UUID);

            if (!list.Any())
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Dll[{File.UUID}]类名错误"
                };

            var list1 = AssemblySave.Assembly.Assemblies.First().GetTypes()
                           .Where(x => x.Name == "Note");

            if (!list1.Any())
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Dll[{File.UUID}]没有注释类"
                };

            AssemblySave.DllType = list.First();
            AssemblySave.NoteType = list1.First();

            foreach (var item in AssemblySave.DllType.GetMethods())
            {
                if (item.Name is "Main" or "GetType" or "ToString"
                    or "Equals" or "GetHashCode" || !item.IsPublic)
                    continue;
                AssemblySave.MethodInfos.Add(item.Name, item);
            }

            if (AssemblySave.MethodInfos.Count == 0)
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Dll[{File.UUID}]没有方法"
                };

            if (!AssemblySave.MethodInfos.ContainsKey(CodeDemo.DllMain))
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Dll[{File.UUID}]没有主方法"
                };

            if (Activator.CreateInstance(AssemblySave.NoteType) is not NotesSDK obj)
            {
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Dll[{File.UUID}]注释类错误"
                };
            }
            foreach (var item in AssemblySave.MethodInfos)
            {
                if (!obj.Function.Keys.Contains(item.Key))
                {
                    return new GenReOBJ
                    {
                        Isok = false,
                        Res = $"Dll[{File.UUID}]的方法[{item}]没有注释"
                    };
                }
            }

            NoteFile.StorageDll(File.UUID, obj);
            DllStonge.AddDll(File.UUID, AssemblySave);

            var time = string.Format("{0:s}", DateTime.Now);
            File.UpdataTime = time;

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
}
