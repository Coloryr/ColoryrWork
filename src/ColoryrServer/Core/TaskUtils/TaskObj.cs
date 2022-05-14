using ColoryrServer.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.TaskUtils;

internal class TaskObj : TaskUserArg
{
    public TaskObj(TaskUserArg arg)
    {
        Name = arg.Name;
        Arg = arg.Arg;
        Dll = arg.Dll;
        Times = arg.Times;
    }
    public TaskState State { get; set; }
    public Task RunTask { get; set; }
    public CancellationTokenSource Cancel { get; set; }
}
