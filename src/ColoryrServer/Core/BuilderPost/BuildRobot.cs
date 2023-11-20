using ColoryrServer.Core.Dll;
using ColoryrServer.Core.Dll.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.Managers;
using ColoryrServer.Core.Utils;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Build;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ColoryrServer.Core.BuilderPost;

internal static class BuildRobot
{
    public static ReMessage Add(BuildOBJ json)
    {
        if (CodeManager.GetRobot(json.UUID) != null)
            return new ReMessage
            {
                Build = false,
                Message = $"Robot[{json.UUID}]已存在"
            };
        ServerMain.LogOut($"[{json.User}]正在创建Robot[{json.UUID}]");
        var time = string.Format("{0:s}", DateTime.Now);
        CSFileCode obj = new()
        {
            UUID = json.UUID,
            Type = CodeType.Robot,
            CreateTime = time,
            Code = Encoding.UTF8.GetString(DemoResource.Robot)
                .Replace(CodeDemo.Name, json.UUID)
        };
        CodeManager.StorageRobot(obj, json.User);
        GenRobot.StartGen(obj, json.User);
        ServerMain.LogOut($"[{json.User}]创建Robot[{json.UUID}]完成");

        return new ReMessage
        {
            Build = true,
            Message = $"Robot[{json.UUID}]已创建"
        };
    }

    public static CSFileList GetList()
    {
        var list = new CSFileList();
        foreach (var item in CodeManager.RobotFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        CodeManager.RemoveFile(CodeType.Robot, json.UUID, json.User);
        AssemblyList.RemoveRobot(json.UUID);

        return new ReMessage
        {
            Build = true,
            Message = $"Robot[{json.UUID}]已删除"
        };
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var obj = CodeManager.GetRobot(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Robot[{json.UUID}]"
            };
        }
        if (obj.Version != json.Version)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Robot[{json.UUID}]版本号错误"
            };
        }

        var list = JsonUtils.ToObj<List<CodeEditObj>>(json.Code);
        obj.Code = FileUtils.StartEdit(obj.Code, list);
        obj.Text = json.Text;

        var sw = new Stopwatch();
        sw.Start();
        var build = GenRobot.StartGen(obj, json.User);
        sw.Stop();
        obj.Up();
        return new ReMessage
        {
            Build = build.Isok,
            Message = build.Res,
            UseTime = sw.ElapsedMilliseconds.ToString(),
            Time = build.Time
        };
    }
}
