using ColoryrServer.Core.Dll.Service;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ColoryrServer.Core.Dll;

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
    /// <summary>
    /// 类
    /// </summary>
    public Type SelfType { get; set; }
    /// <summary>
    /// DLL类型
    /// </summary>
    public CodeType DllType { get; set; }
    /// <summary>
    /// 输出报错
    /// </summary>
    public bool Debug { get; set; }
    /// <summary>
    /// 类方法
    /// </summary>
    public Dictionary<string, MethodInfo> MethodInfos { get; } = new();
    protected override Assembly Load(AssemblyName name)
    {
        var item = AssemblyList.FindClass(name);
        if (item == null)
        {
            return null;
        }
        DllUseSave.AddSave(item, this);
        var list = item.Assemblies.Where(a => a.GetName().FullName == name.FullName);
        return list.First();
    }
}

public class HttpDllAssembly : DllAssembly
{
    public bool Static;
    public HttpDllAssembly(CodeType type, string name) : base(type, name) { }
}

public class RobotDllAssembly : DllAssembly
{
    private List<int> Events;
    public RobotDllAssembly(CodeType type, string name) : base(type, name) { }

    public void SetPack(List<int> events)
    {
        Events = events;
    }

    public bool Check(int id)
    {
        return Events.Contains(id);
    }
}

public class SocketDllAssembly : DllAssembly
{
    public bool Netty;
    public SocketDllAssembly(CodeType type, string name) : base(type, name) { }
}

public class ServiceDllAssembly : DllAssembly
{
    public bool AutoStart;
    public ServiceType ServiceType;
    public ServiceDllAssembly(CodeType type, string name) : base(type, name) { }

    new public void Unload()
    {
        ServiceManager.Remove(Name);
        base.Unload();
    }
}
