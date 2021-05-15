using Lib.Server;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ColoryrServer.DllManager.StartGen.GenUtils
{
    internal enum GenLib
    {
        Dll, App
    }
    internal class GenCode
    {
        public static readonly List<MetadataReference> References = new();
        public static readonly List<MetadataReference> AppReferences = new();

        private static readonly string AppLibLocal = ServerMain.RunLocal + "Libs/App/";
        private static readonly string DllLibLocal = ServerMain.RunLocal + "Libs/Dll/";

        public static GenReOBJ StartGen(string Name, List<SyntaxTree> Code, GenLib lib)
        {
            List<MetadataReference> refs = lib switch
            {
                GenLib.App => AppReferences,
                _ => References,
            };
            CSharpCompilation compilation = CSharpCompilation.Create(
                Name,
                syntaxTrees: Code,
                references: refs,
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
                    foreach(var item1 in list)
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
           
            //导入DLL
            foreach (var Item in list)
            {
                if (Item.IsDynamic)
                    continue;
                if (string.IsNullOrWhiteSpace(Item.Location))
                    continue;
                References.Add(MetadataReference.CreateFromFile(Item.Location));
            }

            if (!Directory.Exists(AppLibLocal))
            {
                Directory.CreateDirectory(AppLibLocal);
            }
            var Dlls = Function.GetPathFileName(AppLibLocal);

            foreach (var Item in Dlls)
            {
                AppReferences.Add(MetadataReference.CreateFromFile(Item.FullName));
            }
        }
    }
}