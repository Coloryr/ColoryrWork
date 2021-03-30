using ColoryrServer.DllManager;
using ColoryrServer.SDK;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace ColoryrServer.TaskUtils
{
    internal class TaskThread
    {
        private static Thread[] Threads;
        private static bool IsRun;
        private static readonly ConcurrentDictionary<string, TaskObj> Tasks = new();
        private static readonly CancellationTokenSource Cancel = new();
        private static readonly EventWaitHandle eventWait = new(true, EventResetMode.ManualReset);

        public static void Start()
        {
            Threads = new Thread[ServerMain.Config.TaskConfig.ThreadNumber];
            for (int a = 0; a < ServerMain.Config.TaskConfig.ThreadNumber; a++)
            {
                Thread item = new(ThreadDo);
                Threads[a] = item;
                item.Start();
            }
            IsRun = true;
            //Task.Run(Remove, Cancel.Token);
        }

        public static void Stop()
        {
            IsRun = false;
            Cancel.Cancel(false);
        }
        private static void Remove()
        {
            while (!IsRun)
            {
                Thread.Sleep(50);
            }
            while (IsRun)
            {
                try
                {
                    eventWait.Reset();
                    var list = from items in Tasks.Values
                               where items.State == TaskState.Done
    || items.State == TaskState.Error || items.State == TaskState.TimeOut || items.State == TaskState.Cancel
                               select items.Name;
                    foreach (var item in list)
                    {
                        Tasks.TryRemove(item, out var temp);
                    }
                    eventWait.Set();
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            }
        }
        private static void ThreadDo()
        {
            while (!IsRun)
            {
                Thread.Sleep(50);
            }
            while (IsRun)
            {
                TaskObj obj = null;
                try
                {
                    eventWait.WaitOne();
                    if (!Tasks.IsEmpty)
                    {
                        foreach (var item in Tasks)
                        {
                            lock (item.Value)
                            {
                                if (item.Value.State == TaskState.Ready)
                                {
                                    obj = item.Value;
                                    obj.State = TaskState.Going;
                                }
                            }
                            obj.Cancel = CancellationTokenSource.CreateLinkedTokenSource(Cancel.Token);
                            bool ok = false;
                            obj.RunTask = new Task(() =>
                            {
                                ok = DllRun.TaskGo(obj);
                            }, obj.Cancel.Token);
                            var delay = Task.Delay(ServerMain.Config.TaskConfig.MaxTime);
                            var res = Task.WhenAny(obj.RunTask, delay).Result;
                            if (res == delay)
                            {
                                obj.State = TaskState.TimeOut;
                                obj.Cancel.Cancel(false);
                            }
                            else if (obj.Cancel.IsCancellationRequested)
                            {
                                obj.State = TaskState.Cancel;
                            }
                            else if (!ok)
                            {
                                obj.State = TaskState.Error;
                            }
                            else if (obj.Times <= 0)
                            {
                                obj.State = TaskState.Done;
                            }
                            else
                            {
                                obj.Times--;
                                obj.State = TaskState.Ready;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                    if (obj != null)
                    {
                        obj.Cancel.Cancel();
                        obj.State = TaskState.Error;
                    }
                }
                Thread.Sleep(50);
            }
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
                    item.Times++;
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
    }
}
