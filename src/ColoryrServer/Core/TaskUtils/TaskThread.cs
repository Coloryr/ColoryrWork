using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.SDK;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.TaskUtils;

internal class TaskThread
{
    private Thread Thread;
    private Thread Worker;
    private bool IsRun;
    private readonly CancellationTokenSource Cancel = new();

    private DllAssembly Assembly;
    public TaskState State { get; private set; }
    public TaskUserArg Arg { get; private set; }

    public TaskThread(TaskUserArg arg) 
    {
        Arg = arg;
        Assembly = DllStonge.GetTask(arg.Dll);
        if (Assembly == null)
        {
            State = TaskState.Error;
        }
        Thread = new(ThreadDo)
        {
            Name = $"Task[{arg.Name}]Thread"
        };
        State = TaskState.Ready;
    }

    public void Run() 
    {
        IsRun = true;
        Thread.Start();
    }

    public void Stop()
    {
        
    }
    private void ThreadDo()
    {
        while (IsRun)
        {
            try
            {
                if (State == TaskState.Ready)
                {
                    bool runDone = false;
                    bool isError = false;
                    Exception e = null;
                    TaskRes ok = null;
                    Worker = new(new ParameterizedThreadStart((obj) =>
                    {
                        try
                        {
                            ok = DllRun.TaskGo(Arg);
                            runDone = true;
                        }
                        catch (Exception e1)
                        {
                            isError = true;
                            e = e1;
                        }
                    }))
                    {
                        Name = $"Task[{Arg.Name}]Worker"
                    };
                    Worker.Start(Cancel.Token);

                    var delay = Task.Delay(TimeSpan.FromSeconds(ServerMain.Config.TaskConfig.MaxTime));
                    delay.Wait();
                    if (!runDone)
                    {
                        State = TaskState.TimeOut;
                        Worker.Interrupt();
                    }
                    else if (isError)
                    {
                        string error;
                        if (e.InnerException is ErrorDump dump)
                        {
                            error = dump.data + "\n" + e.ToString();
                        }
                        else
                        {
                            error = e.ToString();
                        }
                        DllRunError.PutError($"[Task]{Arg.Name}", error);
                        State = TaskState.Error;
                    }
                    else if (ok == null)
                    {
                        State = TaskState.Error;
                    }
                    else if (Arg.Times <= 0)
                    {
                        State = TaskState.Done;
                    }
                    else
                    {
                        Arg.Times--;
                        State = TaskState.Ready;
                    }
                }

                Thread.Sleep(Arg.Sleep);
            }
            catch (Exception e)
            {
                DllRunError.PutError($"[Task]{Arg.Name}", e.ToString());
                ServerMain.LogError(e);
            }
            Thread.Sleep(50);
        }
    }
}
