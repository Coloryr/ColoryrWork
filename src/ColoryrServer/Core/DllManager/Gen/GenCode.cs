using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;

namespace ColoryrServer.Core.DllManager.Gen;

internal class GenCode
{
    public static readonly List<PortableExecutableReference> References = new();

    private static readonly string DllLibLocal = ServerMain.RunLocal + "Libs/";

    public static GenReOBJ StartGen(string Name, List<SyntaxTree> Code)
    {
        CSharpCompilation compilation = CSharpCompilation.Create(
            Name,
            syntaxTrees: Code,
            references: References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var MS = new MemoryStream();
        var MSPdb = new MemoryStream();
        EmitResult Result = compilation.Emit(MS, MSPdb);
        if (Result.Success == false)
        {
            string Res = "编译错误";
            foreach (var Item in Result.Diagnostics)
            {
                Res += "\n" + Item.ToString();
            }
            MS.Close();
            MS.Dispose();
            MSPdb.Close();
            MSPdb.Dispose();
            return new GenReOBJ
            {
                Isok = false,
                Res = Res
            };
        }
        else
        {
            return new GenReOBJ
            {
                Isok = true,
                MS = MS,
                MSPdb = MSPdb
            };
        }
    }
    public static void Start()
    {
        if (!Directory.Exists(DllLibLocal))
        {
            Directory.CreateDirectory(DllLibLocal);
        }
        var list = AppDomain.CurrentDomain.GetAssemblies();
        bool add;
        foreach (var item in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory))
        {
            add = true;
            if (item.EndsWith(".dll"))
            {
                foreach (var item2 in ServerMain.Config.NotInclude)
                {
                    if (item.Contains(item2))
                    {
                        add = false;
                        break;
                    }
                }
                if (!add)
                    continue;
                foreach (var item1 in list)
                {
                    if (item1.IsDynamic)
                        continue;
                    if (item1.Location == item)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    References.Add(MetadataReference.CreateFromFile(item));
                }
            }
        }
        foreach (var item in Directory.GetFiles(DllLibLocal))
        {
            add = true;
            if (item.EndsWith(".dll"))
            {
                foreach (var item2 in ServerMain.Config.NotInclude)
                {
                    if (item.Contains(item2))
                    {
                        add = false;
                        break;
                    }
                }
                if (!add)
                    continue;
                foreach (var item1 in list)
                {
                    if (item1.IsDynamic)
                        continue;
                    if (item1.Location == item)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    References.Add(MetadataReference.CreateFromFile(item));
                }

                using var FileStream = new FileStream(item, FileMode.Open, FileAccess.Read);
                var pdb = item.Replace(".dll", ".pdb");
                if (File.Exists(pdb))
                {
                    using var FileStream1 = new FileStream(pdb, FileMode.Open, FileAccess.Read);
                    AssemblyLoadContext.Default.LoadFromStream(FileStream, FileStream1);
                }
                else
                    AssemblyLoadContext.Default.LoadFromStream(FileStream);
            }
        }

        //导入DLL
        foreach (var Item in list)
        {
            if (Item.IsDynamic)
                continue;
            if (string.IsNullOrWhiteSpace(Item.Location))
                continue;
            References.Add(MetadataReference.CreateFromFile(Item.Location));
        }
    }

    public static void LoadClass(string local)
    {
        local = local.Replace("\\", "/");
        var item = References.Find(a =>
        {
            string temp = a.FilePath.Replace("\\", "/");
            return a.FilePath == local;
        });
        if (item != null)
        {
            References.Remove(item);
        }

        References.Add(MetadataReference.CreateFromFile(local));
    }
}
