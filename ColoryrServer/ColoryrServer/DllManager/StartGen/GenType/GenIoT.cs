﻿using ColoryrServer.DllManager.StartGen.GenUtils;
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
    internal class GenIoT
    {
        public static GenReOBJ StartGen(CSFileCode File)
        {
            var Res = GenCode.StartGen(File.UUID, new()
            {
                CSharpSyntaxTree.ParseText(File.Code)
            }, GenLib.Dll);
            Task.Run(() => CSFile.StorageIoT(File));
            if (!Res.Isok)
            {
                Res.Res = $"IoT[{ File.UUID }]" + Res.Res;
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
                    Res = $"IoT[{ File.UUID }]类名错误"
                };

            var list1 = AssemblySave.Assembly.Assemblies.First().GetTypes()
                           .Where(x => x.Name == "Note");

            if (list1.Any())
            {
                AssemblySave.NoteType = list1.First();
                if (Activator.CreateInstance(AssemblySave.NoteType) is NotesSDK obj)
                {
                    NoteFile.StorageIoT(File.UUID, obj);
                }
            }

            AssemblySave.DllType = list.First();

            foreach (var item in AssemblySave.DllType.GetMethods())
            {
                if (item.Name is CodeDemo.IoTTcp or CodeDemo.IoTUdp && item.IsPublic)
                    AssemblySave.MethodInfos.Add(item.Name, item);
            }

            if (AssemblySave.MethodInfos.Count == 0)
                return new GenReOBJ
                {
                    Isok = false,
                    Res = $"IoT[{File.UUID}]没有方法"
                };

            DllStonge.AddIoT(File.UUID, AssemblySave);

            var time = string.Format("{0:s}", DateTime.Now);
            File.UpdataTime = time;

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

                Res.MSPdb.Close();
                Res.MSPdb.Dispose();

                Res.MS.Close();
                Res.MS.Dispose();
                GC.Collect();
            });

            return new GenReOBJ
            {
                Isok = true,
                Res = $"IoT[{File.UUID}]编译完成",
                Time = File.UpdataTime
            };
        }
    }
}
