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
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager.StartGen.GenType
{
    class GenTask
    {
        public static GenReOBJ StartGen(CSFileCode File)
        {
            var Res = GenCode.StartGen(File.UUID, new List<SyntaxTree>
            {
                CSharpSyntaxTree.ParseText(File.Code)
            }, GenLib.Dll);
            Task.Run(() => CSFile.StorageTask(File));
            if (!Res.Isok)
            {
                Res.Res = $"Task[{File.UUID}]" + Res.Res;
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
                           .Where(x => x.Name == File.UUID);

            if (!list.Any())
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Task[{File.UUID}]类名错误"
                };

            var list1 = AssemblySave.Assembly.Assemblies.First().GetTypes()
                           .Where(x => x.Name == "Note");

            AssemblySave.DllType = list.First();

            foreach (var item in AssemblySave.DllType.GetMethods())
            {
                if (item.Name is CodeDemo.TaskRun && item.IsPublic)
                {
                    AssemblySave.MethodInfos.Add(item.Name, item);
                    break;
                }
            }

            if (!AssemblySave.MethodInfos.ContainsKey(CodeDemo.TaskRun))
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Task[{File.UUID}]没有主方法"
                };

            try
            {
                if (list1.Any())
                {
                    AssemblySave.NoteType = list1.First();
                    if (Activator.CreateInstance(AssemblySave.NoteType) is not NotesSDK obj)
                    {
                        return new GenReOBJ
                        {
                            Isok = false,
                            Res = $"Task[{File.UUID}]注释类错误"
                        };
                    }
                    NoteFile.StorageTask(File.UUID, obj);
                }
            }
            catch
            {
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Task[{File.UUID}]注释出错"
                };
            }


            DllStonge.AddDll(File.UUID, AssemblySave);

            var time = string.Format("{0:s}", DateTime.Now);
            File.UpdataTime = time;

            Task.Run(() =>
            {
                Res.MS.Seek(0, SeekOrigin.Begin);
                Res.MSPdb.Seek(0, SeekOrigin.Begin);

                using (var FileStream = new FileStream(
                    DllStonge.TaskLocal + File.UUID + ".dll", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MS.ToArray());
                    FileStream.Flush();
                }

                using (var FileStream = new FileStream(
                    DllStonge.TaskLocal + File.UUID + ".pdb", FileMode.OpenOrCreate))
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
                Time = File.UpdataTime
            };
        }
    }
}
