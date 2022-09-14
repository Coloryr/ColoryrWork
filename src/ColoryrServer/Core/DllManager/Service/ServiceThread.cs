using ColoryrServer.Core.FileSystem;
using ColoryrServer.SDK;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.DllManager.Service;

internal class ServiceThread : IService
{
    public delegate ServiceNextState ServiceRun(object[] args, CancellationToken token);
    public delegate void ServiceSt();

    private bool IsRun;
    private CancellationTokenSource Source;
    private readonly Thread Thread;
    private readonly Thread Worker;
    private readonly Semaphore Semaphore1 = new(0, 2);
    private readonly Semaphore Semaphore2 = new(0, 2);

    private readonly ServiceDllAssembly Assembly;
    private object[] NewArg = null;

    private ServiceNextState Next;

    public Exception LastError { get; private set; } = null;
    public string Name { get; private init; }
    public ServiceState State { get; private set; } = ServiceState.Init;
    public object[] Arg { get; private set; }

    public ServiceThread(string name, object[] arg)
    {
        Arg = arg;
        Name = name;
        Assembly = DllStongeManager.GetService(name);
        if (Assembly == null)
        {
            State = ServiceState.Error;
        }
        Thread = new(() =>
        {
            while (IsRun)
            {
                try
                {
                    if (State == ServiceState.WaitArg && NewArg != null)
                    {
                        Arg = NewArg;
                        NewArg = null;
                        State = ServiceState.Ready;
                    }

                    if (State == ServiceState.Ready)
                    {
                        State = ServiceState.Going;
                        Semaphore1.Release();
                        if (IsRun == false)
                            return;
                        Semaphore2.WaitOne();
                        if (State != ServiceState.Going)
                        {
                            continue;
                        }
                        switch (Next)
                        {
                            case ServiceNextState.Continue:
                                State = ServiceState.Ready;
                                break;
                            case ServiceNextState.Pause:
                                State = ServiceState.Pause;
                                break;
                            case ServiceNextState.Stop:
                                OnStop();
                                break;
                            case ServiceNextState.WaitArg:
                                State = ServiceState.WaitArg;
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Task.Run(() => DllRun.ServiceOnError(e));
                    DllRunLog.PutError($"[Service]{Name}", e.ToString());
                    ServerMain.LogError(e);
                }
                Thread.Sleep(50);
            }
        })
        {
            Name = $"Service[{Name}]Thread"
        };
        IsRun = true;
        Thread.Start();
        State = ServiceState.Init;
        Worker = new(() =>
        {
            while (IsRun)
            {
                Semaphore1.WaitOne();
                if (IsRun)
                    return;
                try
                {
                    if (!Assembly.MethodInfos.ContainsKey(CodeDemo.ServiceRun))
                        return;
                    MethodInfo mi = Assembly.MethodInfos[CodeDemo.ServiceRun];
                    if (mi.IsStatic)
                    {
                        var temp = Delegate.CreateDelegate(typeof(ServiceRun), mi) as ServiceRun;
                        Next = temp(Arg, Source.Token);
                    }
                    else
                    {
                        var obj1 = Activator.CreateInstance(Assembly.SelfType);
                        var temp = Delegate.CreateDelegate(typeof(ServiceRun), obj1, mi) as ServiceRun;
                        Next = temp(Arg, Source.Token);
                    }
                }
                catch (Exception e)
                {
                    LastError = e;
                    State = ServiceState.Error;
                    string error;
                    if (e.InnerException is ErrorDump Dump)
                    {
                        error = Dump.data;
                    }
                    else
                    {
                        error = e.ToString();
                    }
                    ServerMain.LogError(error);
                    Task.Run(() => DllRun.ServiceOnError(e));
                    DllRunLog.PutError($"[Service]{Name}", error);
                }
                Semaphore2.Release();
            }
        })
        {
            Name = $"Service[{Name}]Worker"
        };
        Worker.Start();
    }

    public void OnStart()
    {
        Source = new();
        if (State == ServiceState.Init)
        {
            State = ServiceState.Start;
            ServerMain.LogOut($"Service[{Name}]正在启动");
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
        State = ServiceState.Ready;
    }

    public void SetArg(object[] arg)
    {
        NewArg = arg;
    }

    public void OnStop()
    {
        ServerMain.LogOut($"Service[{Name}]正在停止");
        State = ServiceState.Stop;
        Semaphore2.Release();
        Source.Cancel();
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
        State = ServiceState.Init;
    }

    public void Close()
    {
        IsRun = false;
        OnStop();
        Thread.Join(TimeSpan.FromSeconds(30));
        Worker.Join(TimeSpan.FromSeconds(30));
    }

    public void Pause()
    {
        State = ServiceState.Pause;
    }
}
