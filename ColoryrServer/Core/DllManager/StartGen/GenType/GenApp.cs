using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager.StartGen.GenType
{
    internal class GenApp
    {
        public static GenReOBJ StartGen(AppFileObj File)
        {
            var list = new List<SyntaxTree>();
            foreach (var item in File.Codes)
            {
                list.Add(CSharpSyntaxTree.ParseText(item.Value));
            }
            var Res = GenCode.StartGen(File.UUID, list, GenLib.App);
            Task.Run(() => CSFile.StorageApp(File));
            if (!Res.Isok)
            {
                Res.Res = $"App[{File.UUID}]" + Res.Res;
                return Res;
            }

            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            var save = new AppBuildSave
            {
                Dll = Res.MS.ToArray(),
                Pdb = Res.MSPdb.ToArray()
            };

            DllStonge.AddApp(File.UUID, save);

            Res.MSPdb.Close();
            Res.MSPdb.Dispose();

            Res.MS.Close();
            Res.MS.Dispose();

            Task.Run(() =>
            {
                string dir = DllStonge.AppLocal + File.UUID + "\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using FileStream FileStream = new FileStream(dir + "app.dll", FileMode.OpenOrCreate);
                FileStream.Write(save.Dll);
                FileStream.Flush();

                using FileStream FileStream1 = new FileStream(dir + "app.pdb", FileMode.OpenOrCreate);
                FileStream1.Write(save.Pdb);
                FileStream1.Flush();

                foreach (var item in File.Xamls)
                {
                    using var FileStream2 = new FileStream(dir + item.Key + ".xaml", FileMode.OpenOrCreate);
                    FileStream2.Write(Encoding.UTF8.GetBytes(item.Value));
                    FileStream2.Flush();
                }
                GC.Collect();
            });

            return new GenReOBJ
            {
                Isok = true,
                Res = $"App[{File.UUID}]编译完成",
                Time = File.UpdataTime
            };
        }
    }
}
