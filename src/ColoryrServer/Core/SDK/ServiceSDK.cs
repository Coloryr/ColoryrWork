using ColoryrServer.Core.Dll.Service;
using ColoryrWork.Lib.Build.Object;
using System;

namespace ColoryrServer.SDK;

public static class ServiceSDK
{
    /// <summary>
    /// 是否存在服务
    /// </summary>
    /// <param name="name">服务名字</param>
    /// <returns>是否存在</returns>
    public static bool Have(string name)
        => ServiceManager.Have(name);
    /// <summary>
    /// 启动一个服务
    /// </summary>
    /// <param name="name">服务名字</param>
    /// <param name="arg">服务参数</param>
    public static void Start(string name)
        => ServiceManager.Start(name);
    /// <summary>
    /// 停止一个服务
    /// </summary>
    /// <param name="name">服务名字</param>
    public static void Stop(string name)
        => ServiceManager.Stop(name);
    /// <summary>
    /// 暂停一个服务
    /// </summary>
    /// <param name="name">服务名字</param>
    public static void Pause(string name)
        => ServiceManager.Pause(name);
    /// <summary>
    /// 获取一个服务的状态
    /// </summary>
    /// <param name="name">服务名字</param>
    /// <returns>状态</returns>
    public static ServiceState GetState(string name)
        => ServiceManager.GetState(name);
    /// <summary>
    /// 设置服务参数
    /// </summary>
    /// <param name="name">服务名字</param>
    /// <param name="arg">参数</param>
    public static void SetArg(string name, object[] arg)
        => ServiceManager.SetArg(name, arg);
    /// <summary>
    /// 获取服务错误信息
    /// </summary>
    /// <param name="name">服务名字</param>
    /// <returns>错误信息</returns>
    public static Exception GetError(string name)
        => ServiceManager.GetError(name);
}
/// <summary>
/// 构建代码前/修改代码前
/// </summary>
public record PerBuildArg
{
    /// <summary>
    /// 编辑的用户
    /// </summary>
    public string User;
    /// <summary>
    /// 代码类型
    /// </summary>
    public CodeType CodeType;
    /// <summary>
    /// 代码对象
    /// </summary>
    public CSFileObj CodeObj;
    /// <summary>
    /// 修改后的代码
    /// </summary>
    public string EditCode;
    /// <summary>
    /// 文件名
    /// </summary>
    public string File;
}
/// <summary>
/// 构建后
/// </summary>
public record PostBuildArg
{
    /// <summary>
    /// 代码类型
    /// </summary>
    public CodeType CodeType;
    /// <summary>
    /// 代码对象
    /// </summary>
    public CSFileObj CodeObj;
    /// <summary>
    /// 构建结果
    /// </summary>
    public bool BuildRes;
    /// <summary>
    /// 编辑的用户
    /// </summary>
    public string User;
}

public enum ServiceState
{
    Init, Start, Ready, Going, Pause, Stop, Error, WaitArg
}

public enum ServiceNextState
{
    Continue, Pause, Stop, WaitArg
}
