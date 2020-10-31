using ColoryrServer.FileSystem;
using Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager
{
    class GenApp
    {
        public static GenReOBJ StartGen(AppFileObj File)
        {
            var list = new List<SyntaxTree>();
            foreach (var item in File.Codes)
            { 
                list.Add( CSharpSyntaxTree.ParseText(item.Value));
            }
            var Res = GenTask.StartGen(File.UUID, list, GenLib.App);
            if (!Res.Isok)
            {
                return Res;
            }

            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            var save = new AppSave();
            save.Key = File.Key;
            save.Xamls = new Dictionary<string, string>(File.Xamls);
            save.Dll = Res.MS.ToArray();
            save.Pdb = Res.MSPdb.ToArray();
            
            DllStonge.AddApp(File.UUID, save);

            Res.MSPdb.Close();
            Res.MSPdb.Dispose();

            Res.MS.Close();
            Res.MS.Dispose();

            Task.Run(() =>
            {
                string dir = DllStonge.DllLocal + File.UUID + "\\";
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

                CSFile.StorageApp(File);
                Config.Save();

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
