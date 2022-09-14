﻿using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ColoryrServer.Core.BuilderPost;

internal static class PostBuildSocket
{
    public static ReMessage Add(BuildOBJ json)
    {
        if (CodeFileManager.GetSocket(json.UUID) != null)
            return new ReMessage
            {
                Build = false,
                Message = $"Socket[{json.UUID}]已存在"
            };
        ServerMain.LogOut($"[{json.User}]创建Socket[{json.UUID}]");
        var time = string.Format("{0:s}", DateTime.Now);
        string code = json.Code.ToLower() == "true" ? DemoResource.Netty : DemoResource.Socket;
        CSFileCode obj = new()
        {
            UUID = json.UUID,
            Type = CodeType.Socket,
            CreateTime = time,
            Code = code
            .Replace(CodeDemo.Name, json.UUID)
        };
        CodeFileManager.StorageSocket(obj, json.User);
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
        foreach (var item in CodeFileManager.SocketFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        CodeFileManager.RemoveFile(CodeType.Socket, json.UUID, json.User);
        return new ReMessage
        {
            Build = true,
            Message = $"Socket[{json.UUID}]已删除"
        };
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var obj = CodeFileManager.GetSocket(json.UUID);
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

        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
        obj.Code = FileEdit.StartEdit(obj.Code, list);
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
