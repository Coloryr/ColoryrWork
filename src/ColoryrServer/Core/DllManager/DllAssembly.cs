using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ColoryrServer.Core.DllManager;

/// <summary>
/// 编译后存储
/// </summary>
public class DllAssembly : AssemblyLoadContext
{
    public DllAssembly(CodeType type, string name)
           : base(name, true)
    {
        DllType = type;
    }
    public Type SelfType { get; set; }
    public CodeType DllType { get; init; }
    public Dictionary<string, MethodInfo> MethodInfos { get; } = new();
    protected override Assembly Load(AssemblyName name)
    {
        var item = DllStonge.FindClass(name);
        if (item == null)
        {
            return null;
        }
        DllUseSave.AddSave(item, this);
        var list = item.Assemblies.Where(a => a.GetName().FullName == name.FullName);
        return list.First();
    }
}
