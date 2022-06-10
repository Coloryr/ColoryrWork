using ColoryrServer.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core.TaskUtils;

internal static class TaskManager
{
    private static readonly ConcurrentDictionary<string, TaskThread> Tasks = new();
    private static Thread TickThread;

    private static void Stop() 
    {

    }

    public static void Start() 
    {
        
    }

    public static bool HaveTask(string name)
    {
        return Tasks.ContainsKey(name);
    }
    public static bool StartTask(TaskUserArg arg)
    {
        if (Tasks.ContainsKey(arg.Name))
        {
            var item = Tasks[arg.Name];
            if (item.Name == arg.Name)
            {
                item.State = TaskState.Ready;
                item.Dll = arg.Dll;
                item.Times = arg.Times;
                return true;
            }
            else
            {
                Tasks.TryRemove(arg.Name, out var temp);
                temp.State = TaskState.Cancel;
                temp.Cancel.Cancel(false);
            }
        }
        TaskObj obj = new(arg)
        {
            State = TaskState.Ready
        };
        Tasks.TryAdd(obj.Name, obj);
        return true;
    }
    public static void StopTask(string name)
    {
        if (Tasks.ContainsKey(name))
        {
            Tasks[name].Cancel?.Cancel();
        }
    }
    public static TaskState GetTaskState(string name)
    {
        if (!Tasks.ContainsKey(name))
        {
            return TaskState.Error;
        }
        return Tasks[name].State;
    }
    public static int GetTaskTime(string name)
    {
        if (!Tasks.ContainsKey(name))
        {
            return -1;
        }
        return Tasks[name].Times;
    }
    public static void SetArg(string name, object[] arg)
    {
        if (Tasks.ContainsKey(name))
        {
            Tasks[name].Arg = arg;
        }
    }

    public static void Pause(string name) 
    {
        
    }

    public static void Cancel(string name) 
    {
        
    }
}
