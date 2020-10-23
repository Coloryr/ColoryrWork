using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;

namespace ColoryrServer.DllManager
{
    class GenTask
    {
        public static Dictionary<string, string> Token = new Dictionary<string, string>();
        private static List<MetadataReference> References = new List<MetadataReference>();
        public static GenReOBJ StartGen(string Name, SyntaxTree Code)
        {
            CSharpCompilation compilation = CSharpCompilation.Create(
                Name,
                syntaxTrees: new[] { Code },
                references: References,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var MS = new MemoryStream();
            var MSPdb = new MemoryStream();
            EmitResult Result = compilation.Emit(MS, MSPdb);
            if (Result.Success == false)
            {
                string Res = "编译错误\n";
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
        public GenTask()
        {
            var list = AppDomain.CurrentDomain.GetAssemblies();
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
    }
}