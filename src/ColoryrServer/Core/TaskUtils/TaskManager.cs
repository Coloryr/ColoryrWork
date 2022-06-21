using ColoryrServer.SDK;
using System.Collections.Concurrent;

namespace ColoryrServer.Core.TaskUtils;

internal static class TaskManager
{
    private static readonly ConcurrentDictionary<string, TaskThread> Tasks = new();

    public static void Remove(string task)
    {
        Tasks.TryRemove(task, out var _);
    }

    private static void Stop()
    {
        foreach (var item in Tasks.Values)
        {
            item.Stop();
        }
    }

    public static void Start()
    {
        ServerMain.OnStop += Stop;
    }

    public static bool HaveTask(string name)
    {
        return Tasks.ContainsKey(name);
    }
    public static void StartTask(TaskUserArg arg)
    {
        if (Tasks.ContainsKey(arg.Name))
        {
            var item = Tasks[arg.Name];
            item.SetArg(arg);
            item.Run();
        }
        else
        {
            TaskThread obj = new(arg);
            Tasks.TryAdd(arg.Name, obj);
            obj.Run();
        }
    }
    public static void StopTask(string name)
    {
        if (Tasks.ContainsKey(name))
        {
            Tasks[name].Stop();
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
        return Tasks[name].Arg.Times;
    }
    public static void SetArg(string name, object[] arg)
    {
        if (Tasks.ContainsKey(name))
        {
            Tasks[name].Arg.Arg = arg;
        }
    }

    public static void Pause(string name)
    {
        if (Tasks.ContainsKey(name))
        {
            Tasks[name].Pause();
        }
    }

    public static void Cancel(string name)
    {
        if (Tasks.ContainsKey(name))
        {
            Tasks[name].Cancel();
        }
    }
}
