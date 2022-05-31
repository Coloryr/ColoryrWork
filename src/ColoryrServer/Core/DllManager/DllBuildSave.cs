using ColoryrServer.DllManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager;
public enum DllType
{
    Dll, Class, Mqtt, Robot, Task, WebSocket, Socket
}
/// <summary>
/// 编译后存储
/// </summary>
public class DllBuildSave : AssemblyLoadContext
{
    public DllBuildSave(DllType type, string name)
           : base(name, true)
    {
        SelfType = type;
    }
    public Type DllType { get; set; }
    public DllType SelfType { get; set; }
    public Dictionary<string, MethodInfo> MethodInfos { get; set; } = new();
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
