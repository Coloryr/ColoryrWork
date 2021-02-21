﻿using ColoryrServer.TaskUtils;
using System;

namespace ColoryrServer.SDK
{
    public class TaskSDK
    {
        /// <summary>
        /// 是否存在任务
        /// </summary>
        /// <param name="name">任务名字</param>
        /// <returns>是否存在</returns>
        public static bool HaveTask(string name)
            => TaskThread.HaveTask(name);
        /// <summary>
        /// 添加一个任务
        /// 如果存在则会覆盖
        /// </summary>
        /// <param name="arg">任务参数</param>
        public static void StartTask(TaskUserArg arg)
            => TaskThread.StartTask(arg);
        /// <summary>
        /// 停止一个任务
        /// </summary>
        /// <param name="name">任务名字</param>
        public static void StopTask(string name)
            => TaskThread.StopTask(name);
        /// <summary>
        /// 获取一个任务的状态
        /// </summary>
        /// <param name="name">任务名字</param>
        /// <returns>状态</returns>
        public static TaskState GetTaskState(string name)
            => TaskThread.GetTaskState(name);
        /// <summary>
        /// 获取任务剩余执行次数
        /// </summary>
        /// <param name="name">任务名字</param>
        /// <returns>次数</returns>
        public static int GetTaskTime(string name)
            => TaskThread.GetTaskTime(name);
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
        /// 任务内容
        /// </summary>
        public Action Task { get; set; }
        /// <summary>
        /// 循环次数
        /// </summary>
        public int Times { get; set; }
    }
}
