using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.TaskUtils
{
    class TaskObj : TaskUserArg
    {
        public TaskObj(TaskUserArg arg)
        {
            Name = arg.Name;
            Task = arg.Task;
            Times = arg.Times;
        }
        public TaskState State { get; set; }
        public Task RunTask { get; set; }
        public CancellationTokenSource Cancel { get; set; }
    }
}
