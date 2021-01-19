﻿using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
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
    internal class GenRobot
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
                    Res = "类名错误"
                };

            AssemblySave.Type = list.First();

            foreach (var Item in AssemblySave.Type.GetMethods())
            {
                if (Item.Name is CodeDemo.RobotMessage or CodeDemo.RobotEvent or CodeDemo.RobotSend)
                    AssemblySave.MethodInfos.Add(Item.Name, Item);
            }

            if (AssemblySave.MethodInfos.Count == 0)
                return new GenReOBJ
                {
                    Isok = false,
                    Res = "没有主方法"
                };

            DllStonge.AddRobot(File.UUID, AssemblySave);

            Task.Run(() =>
            {
                Res.MS.Seek(0, SeekOrigin.Begin);
                Res.MSPdb.Seek(0, SeekOrigin.Begin);

                using (var FileStream = new FileStream(
                    DllStonge.RobotLocal + File.UUID + ".dll", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MS.ToArray());
                    FileStream.Flush();
                }

                using (var FileStream = new FileStream(
                    DllStonge.RobotLocal + File.UUID + ".pdb", FileMode.OpenOrCreate))
                {
                    FileStream.Write(Res.MSPdb.ToArray());
                    FileStream.Flush();
                }

                CSFile.StorageRobot(File);
                ConfigUtil.Save();

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
