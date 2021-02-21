using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrServer.SDK;
using Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager.StartGen.GenType
{
    internal class GenClass
    {
        public static GenReOBJ StartGen(CSFileCode File)
        {
            var Res = GenTask.StartGen(File.UUID, new()
            {
                CSharpSyntaxTree.ParseText(File.Code)
            }, GenLib.Dll);
            Task.Run(() => CSFile.StorageClass(File));
            if (!Res.Isok)
            {
                Res.Res = $"Class[{File.UUID}]" + Res.Res;
                return Res;
            }

            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            var AssemblySave = new DllBuildSave
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
                    Res = $"Class[{File.UUID}]类名错误"
                };

            var list1 = AssemblySave.Assembly.Assemblies.First().GetTypes()
                           .Where(x => x.Name == "Note");

            if (!list1.Any())
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Class[{File.UUID}]没有注释类"
                };

            AssemblySave.DllType = list.First();
            AssemblySave.NoteType = list1.First();

            if (Activator.CreateInstance(AssemblySave.NoteType) is not NotesSDK obj)
            {
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"Class[{File.UUID}]注释类错误"
                };
            }

            NoteFile.StorageClass(File.UUID, obj);
            DllStonge.AddClass(File.UUID, AssemblySave);

            var time = string.Format("{0:s}", DateTime.Now);
            File.UpdataTime = time;

            Task.Run(() =>
            {
                Res.MS.Seek(0, SeekOrigin.Begin);
                Res.MSPdb.Seek(0, SeekOrigin.Begin);

                using (var FileStream = new FileStream(
                    DllStonge.ClassLocal + File.UUID + ".dll", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MS.ToArray());
                    FileStream.Flush();
                }

                using (var FileStream = new FileStream(
                    DllStonge.ClassLocal + File.UUID + ".pdb", FileMode.OpenOrCreate))
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
                Res = $"Class[{File.UUID}]编译完成",
                Time = File.UpdataTime
            };
        }
    }
}
