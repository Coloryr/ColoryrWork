using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrWork.Lib.Build.Object;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ColoryrServer.DllManager.StartGen.GenType;

internal class GenWebSocket
{
    public static GenReOBJ StartGen(CSFileCode File)
    {
        var Res = GenCode.StartGen(File.UUID, new()
        {
            CSharpSyntaxTree.ParseText(File.Code)
        }, GenLib.Dll);
        Task.Run(() => CSFile.StorageWebSocket(File));
        if (!Res.Isok)
        {
            Res.Res = $"WebSocket[{ File.UUID }]" + Res.Res;
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
                Res = $"WebSocket[{ File.UUID }]类名错误"
            };

        AssemblySave.DllType = list.First();

        foreach (var item in AssemblySave.DllType.GetMethods())
        {
            if (item.Name is CodeDemo.WebSocketMessage or CodeDemo.WebSocketOpen or CodeDemo.WebSocketClose)
                AssemblySave.MethodInfos.Add(item.Name, item);
        }

        if (AssemblySave.MethodInfos.Count == 0)
            return new GenReOBJ
            {
                Isok = false,
                Res = $"WebSocket[{ File.UUID }]没有主方法"
            };

        DllStonge.AddWebSocket(File.UUID, AssemblySave);

        Task.Run(() =>
        {
            Res.MS.Seek(0, SeekOrigin.Begin);
            Res.MSPdb.Seek(0, SeekOrigin.Begin);

            using (var FileStream = new FileStream(
                DllStonge.WebSocketLocal + File.UUID + ".dll", FileMode.OpenOrCreate))
            {
                FileStream.Write(Res.MS.ToArray());
                FileStream.Flush();
            }

            using (var FileStream = new FileStream(
                DllStonge.WebSocketLocal + File.UUID + ".pdb", FileMode.OpenOrCreate))
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
            Res = $"WebSocket[{ File.UUID }]编译完成",
            Time = File.UpdataTime
        };
    }
}
