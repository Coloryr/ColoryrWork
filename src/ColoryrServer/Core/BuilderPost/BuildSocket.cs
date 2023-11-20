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

internal static class BuildSocket
{
    public static ReMessage Add(BuildOBJ json)
    {
        if (CodeManager.GetSocket(json.UUID) != null)
            return new ReMessage
            {
                Build = false,
                Message = $"Socket[{json.UUID}]已存在"
            };
        ServerMain.LogOut($"[{json.User}]正在创建Socket[{json.UUID}]");
        var time = string.Format("{0:s}", DateTime.Now);
        string code = Encoding.UTF8.GetString(json.Code.ToLower() == "true" ? DemoResource.Netty : DemoResource.Socket);
        CSFileCode obj = new()
        {
            UUID = json.UUID,
            Type = CodeType.Socket,
            CreateTime = time,
            Code = code
            .Replace(CodeDemo.Name, json.UUID)
        };
        CodeManager.StorageSocket(obj, json.User);
        GenSocket.StartGen(obj, json.User);
        ServerMain.LogOut($"[{json.User}]创建Socket[{json.UUID}]完成");

        return new ReMessage
        {
            Build = true,
            Message = $"Socket[{json.UUID}]已创建"
        };
    }

    public static CSFileList GetList()
    {
        var list = new CSFileList();
        foreach (var item in CodeManager.SocketFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        CodeManager.RemoveFile(CodeType.Socket, json.UUID, json.User);
        AssemblyList.RemoveSocket(json.UUID);

        return new ReMessage
        {
            Build = true,
            Message = $"Socket[{json.UUID}]已删除"
        };
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var obj = CodeManager.GetSocket(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Socket[{json.UUID}]"
            };
        }
        if (obj.Version != json.Version)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Socket[{json.UUID}]版本号错误"
            };
        }

        var list = JsonUtils.ToObj<List<CodeEditObj>>(json.Code);
        obj.Code = FileUtils.StartEdit(obj.Code, list);
        obj.Text = json.Text;

        var sw = new Stopwatch();
        sw.Start();
        var build = GenSocket.StartGen(obj, json.User);
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
