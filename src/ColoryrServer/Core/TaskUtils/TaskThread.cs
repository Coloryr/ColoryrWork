using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.SDK;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.TaskUtils;

internal class TaskThread
{
    private Thread Thread;
    private Thread Worker;
    private bool IsRun;
    private bool RunDone = false;
    private readonly CancellationTokenSource CancelToken = new();

    private DllAssembly Assembly;
    private TaskUserArg NewArg = null;

    public TaskState State { get; private set; } = TaskState.Init;
    public TaskUserArg Arg { get; private set; }

    public TaskThread(TaskUserArg arg) 
    {
        Arg = arg;
        Assembly = DllStongeManager.GetTask(arg.Dll);
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

    public void Pause() 
    {
        State = TaskState.Pause;
    }

    public void SetArg(TaskUserArg arg) 
    {
        State = TaskState.Init;
        NewArg = arg;
    }

    public void Stop()
    {
        State = TaskState.Stop;
        CancelToken.Cancel();
        Worker?.Interrupt();
    }

    public void Cancel() 
    {
        CancelToken.Cancel();
        State = TaskState.Cancel;
    }
    private void ThreadDo()
    {
        while (IsRun)
        {
            try
            {
                if (State == TaskState.Init)
                {
                    if (NewArg?.OverWrite == true)
                    {
                        Arg.Dll = NewArg.Dll;
                        Arg.Sleep = NewArg.Sleep;
                        Arg.Times = NewArg.Times;
                        Arg.Arg = NewArg.Arg;
                        Assembly = DllStongeManager.GetTask(Arg.Dll);
                        if (Assembly == null)
                        {
                            State = TaskState.Error;
                            return;
                        }
                        State = TaskState.Ready;
                    }
                }

                if (State == TaskState.Ready)
                {
                    bool isError = false;
                    Exception e = null;
                    TaskRes ok = null;
                    Worker = new(() =>
                    {
                        try
                        {
                            RunDone = false;
                            ok = DllRun.TaskGo(Arg);
                            RunDone = true;
                        }
                        catch (Exception e1)
                        {
                            isError = true;
                            e = e1;
                        }
                    })
                    {
                        Name = $"Task[{Arg.Name}]Worker"
                    };
                    Worker.Start(CancelToken.Token);

                    var delay = Task.Delay(TimeSpan.FromSeconds(ServerMain.Config.TaskConfig.MaxTime));
                    delay.Wait();
                    if (!RunDone)
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
                    Worker = null;
                }

                if (State == TaskState.Stop || State == TaskState.Cancel)
                {
                    TaskManager.Remove(Arg.Name);
                    return;
                }

                Thread.Sleep(Arg.Sleep);
                continue;
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
