using ColoryrServer.Core.Dll;
using ColoryrServer.Core.Dll.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.Http;
using ColoryrServer.Core.Managers;
using ColoryrServer.Core.Utils;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ColoryrServer.Core.BuilderPost;

internal static class BuildDll
{
    /// <summary>
    /// 添加一个接口
    /// </summary>
    /// <param name="json">接口信息</param>
    /// <returns>结果</returns>
    public static ReMessage Add(BuildOBJ json)
    {
        string uuid = json.UUID.Replace('\\', '/');
        if (uuid.EndsWith("/"))
        {
            uuid = uuid[..^1];
        }
        if (CodeManager.GetDll(uuid) != null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Dll[{uuid}]已存在"
            };
        }
        if (HttpInvokeRoute.CheckBase(uuid))
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Dll[{uuid}]路由冲突"
            };
        }
        ServerMain.LogOut($"[{json.User}]正在创建Dll[{uuid}]");
        var time = string.Format("{0:s}", DateTime.Now);

        CSFileCode obj = new()
        {
            UUID = uuid,
            Type = CodeType.Dll,
            CreateTime = time,
            Code = Encoding.UTF8.GetString(DemoResource.Dll)
                .Replace(CodeDemo.Name, EnCode.SHA1(uuid))
        };
        CodeManager.StorageDll(obj, json.User);
        GenDll.StartGen(obj, json.User);
        ServerMain.LogOut($"[{json.User}]创建Dll[{uuid}]完成");

        return new ReMessage
        {
            Build = true,
            Message = $"Dll[{uuid}]已创建"
        };
    }

    public static CSFileList GetList()
    {
        var list = new CSFileList();
        foreach (var item in CodeManager.DllFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        CodeManager.RemoveFile(CodeType.Dll, json.UUID, json.User);
        AssemblyList.RemoveDll(json.UUID);

        return new ReMessage
        {
            Build = true,
            Message = $"Dll[{json.UUID}]已删除"
        };
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var obj = CodeManager.GetDll(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Dll[{json.UUID}]"
            };
        }
        if (obj.Version != json.Version)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Dll[{json.UUID}]版本号错误"
            };
        }

        var list = JsonUtils.ToObj<List<CodeEditObj>>(json.Code);
        obj.Code = FileUtils.StartEdit(obj.Code, list);
        obj.Text = json.Text;

        var arg = new PerBuildArg
        {
            CodeObj = obj,
            CodeType = CodeType.Dll,
            EditCode = obj.Code,
            User = json.User
        };

        DllRun.ServiceOnBuild(arg);

        var sw = new Stopwatch();
        sw.Start();
        var build = GenDll.StartGen(obj, json.User);
        sw.Stop();
        obj.Up();

        var arg1 = new PostBuildArg
        {
            CodeObj = obj,
            CodeType = CodeType.Dll,
            User = json.User,
            BuildRes = build.Isok
        };

        DllRun.ServiceOnBuild(arg1);

        return new ReMessage
        {
            Build = build.Isok,
            Message = build.Res,
            UseTime = sw.ElapsedMilliseconds.ToString(),
            Time = build.Time
        };
    }
}
