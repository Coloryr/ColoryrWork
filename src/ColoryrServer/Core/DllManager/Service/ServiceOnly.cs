using ColoryrServer.Core.FileSystem;
using ColoryrServer.SDK;
using System;
using System.Reflection;

namespace ColoryrServer.Core.DllManager.Service;

internal class ServiceOnly : IService
{
    public delegate void ServiceSt();

    private readonly ServiceDllAssembly Assembly;

    public string Name { get; private init; }
    public ServiceState State { get; private set; } = ServiceState.Init;
    public Exception LastError => null;

    public ServiceOnly(string name)
    {
        Name = name;
        Assembly = DllStongeManager.GetService(name);
        if (Assembly == null)
        {
            State = ServiceState.Error;
        }
    }

    public void Close()
    {

    }

    public void OnStart()
    {
        ServerMain.LogOut($"Service[{Name}]正在启动");
        State = ServiceState.Start;
        MethodInfo mi = Assembly.MethodInfos[CodeDemo.ServiceStart];
        if (mi.IsStatic)
        {
            (Delegate.CreateDelegate(typeof(ServiceSt), mi) as ServiceSt)();
        }
        else
        {
            var obj1 = Activator.CreateInstance(Assembly.SelfType);
            (Delegate.CreateDelegate(typeof(ServiceSt), obj1, mi) as ServiceSt)();
        }
    }

    public void OnStop()
    {
        ServerMain.LogOut($"Service[{Name}]正在停止");
        State = ServiceState.Stop;
        MethodInfo mi = Assembly.MethodInfos[CodeDemo.ServiceStop];
        if (mi.IsStatic)
        {
            (Delegate.CreateDelegate(typeof(ServiceSt), mi) as ServiceSt)();
        }
        else
        {
            var obj1 = Activator.CreateInstance(Assembly.SelfType);
            (Delegate.CreateDelegate(typeof(ServiceSt), obj1, mi) as ServiceSt)();
        }
    }

    public void Pause()
    {

    }

    public void SetArg(object[] arg)
    {

    }
}
