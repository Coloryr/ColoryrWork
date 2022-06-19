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
    /// <summary>
    /// 编译引用
    /// </summary>
    private static readonly List<PortableExecutableReference> References = new();

    private static readonly string DllLibLocal = ServerMain.RunLocal + "Libs/";

    /// <summary>
    /// 编译成.dll
    /// </summary>
    /// <param name="name">文件名</param>
    /// <param name="codes">代码树</param>
    /// <returns></returns>
    public static GenReOBJ StartGen(string name, List<SyntaxTree> codes)
    {
        CSharpCompilation compilation = CSharpCompilation.Create(
            name,
            syntaxTrees: codes,
            references: References,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var ms = new MemoryStream();
        var msPdb = new MemoryStream();
        EmitResult result = compilation.Emit(ms, msPdb);
        if (result.Success == false)
        {
            string res = "编译错误";
            foreach (var item in result.Diagnostics)
            {
                res += "\n" + item.ToString();
            }
            ms.Close();
            ms.Dispose();
            msPdb.Close();
            msPdb.Dispose();
            return new GenReOBJ
            {
                Isok = false,
                Res = res
            };
        }
        else
        {
            string res = "没有警告";
            if (result.Diagnostics.Length > 0)
            {
                res = "编译警告";
                foreach (var item in result.Diagnostics)
                {
                    res += "\n" + item.ToString();
                }
            }

            return new GenReOBJ
            {
                Isok = true,
                Res = res,
                MS = ms,
                MSPdb = msPdb
            };
        }
    }
    /// <summary>
    /// 初始化编译
    /// </summary>
    public static void Start()
    {
        if (!Directory.Exists(DllLibLocal))
        {
            Directory.CreateDirectory(DllLibLocal);
        }
        var list = AppDomain.CurrentDomain.GetAssemblies();
        bool add;
        //从根目录加载.dll
        foreach (var item in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory))
        {
            add = true;
            if (item.EndsWith(".dll"))
            {
                foreach (var item2 in ServerMain.Config.CodeSetting.NotInclude)
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
        //从库文件夹加载.dll
        foreach (var item in Directory.GetFiles(DllLibLocal))
        {
            add = true;
            if (item.EndsWith(".dll"))
            {
                foreach (var item2 in ServerMain.Config.CodeSetting.NotInclude)
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

                //加入到运行环境中
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

    /// <summary>
    /// 加载Class的.dll
    /// </summary>
    /// <param name="local">.dll位置</param>
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
