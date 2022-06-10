using ColoryrServer.Core.TaskUtils;

namespace ColoryrServer.SDK;

public class TaskSDK
{
    /// <summary>
    /// 是否存在任务
    /// </summary>
    /// <param name="name">任务名字</param>
    /// <returns>是否存在</returns>
    public static bool HaveTask(string name)
        => TaskManager.HaveTask(name);
    /// <summary>
    /// 添加一个任务
    /// 如果存在则会覆盖
    /// </summary>
    /// <param name="arg">任务参数</param>
    public static bool StartTask(TaskUserArg arg)
        => TaskManager.StartTask(arg);
    /// <summary>
    /// 停止一个任务
    /// </summary>
    /// <param name="name">任务名字</param>
    public static void StopTask(string name)
        => TaskManager.StopTask(name);
    /// <summary>
    /// 获取一个任务的状态
    /// </summary>
    /// <param name="name">任务名字</param>
    /// <returns>状态</returns>
    public static TaskState GetTaskState(string name)
        => TaskManager.GetTaskState(name);
    /// <summary>
    /// 获取任务剩余执行次数
    /// </summary>
    /// <param name="name">任务名字</param>
    /// <returns>次数</returns>
    public static int GetTaskTime(string name)
        => TaskManager.GetTaskTime(name);
    /// <summary>
    /// 设置任务参数
    /// </summary>
    /// <param name="arg"></param>
    public static void SetArg(string name, object[] arg)
        => TaskManager.SetArg(name, arg);
    /// <summary>
    /// 暂停任务
    /// </summary>
    /// <param name="name"></param>
    public static void Pause(string name)
        => TaskManager.Pause(name);
}
public enum TaskState
{
    Ready, Going, TimeOut, Done, Error, Cancel
}
public class TaskUserArg
{
    /// <summary>
    /// 任务名字
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 任务类名
    /// </summary>
    public string Dll { get; set; }
    /// <summary>
    /// 参数
    /// </summary>
    public object[] Arg { get; set; } = new object[1];
    /// <summary>
    /// 循环次数
    /// </summary>
    public int Times { get; set; }
    /// <summary>
    /// 循环间隔(ms)
    /// </summary>
    public int Sleep { get; set; }
    /// <summary>
    /// 复写参数
    /// </summary>
    public bool OverWrite { get; set; }
}
public class TaskRes 
{
    /// <summary>
    /// 新的循环次数
    /// </summary>
    public int NewTimes { get; set; }
    /// <summary>
    /// 删除任务
    /// </summary>
    public bool Delete { get; set; }
    /// <summary>
    /// 运行结果
    /// </summary>
    public bool Res { get; set; }
    /// <summary>
    /// 暂停
    /// </summary>
    public bool Pause { get; set; }
}