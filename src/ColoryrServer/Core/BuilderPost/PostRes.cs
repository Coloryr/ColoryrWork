using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.BuilderPost;

public static class PostRes
{
    public readonly static ReMessage Old = new()
    {
        Build = false,
        Message = "旧版编辑器，需要升级"
    };

    public readonly static ReMessage ArgError = new()
    {
        Build = false,
        Message = "参数错误"
    };

    public readonly static ReMessage UserError = new()
    {
        Build = false,
        Message = "账户或密码错误"
    };

    public readonly static ReMessage AutoError = new()
    {
        Build = false,
        Message = "自动登录检查错误"
    };

    public readonly static ReMessage Error = new()
    {
        Build = false,
        Message = "错误的操作"
    };

    public readonly static ReMessage LoginOut = new()
    {
        Build = false,
        Message = "登录失效"
    };

    public readonly static ReMessage RebuildGoing = new()
    {
        Build = false,
        Message = "已经在执行重新构建了"
    };

    public readonly static ReMessage RebuildStart = new()
    {
        Build = true,
        Message = "执行重构操作"
    };

    public readonly static ReMessage AddClient = new()
    {
        Build = true,
        Message = "监听创建成功"
    };

    public readonly static ReMessage ListError = new()
    {
        Build = false,
        Message = "获取服务器日志错误"
    };
}
