﻿using ColoryrServer.Core.Dll;
using ColoryrServer.Core.Dll.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.Managers;
using ColoryrServer.Core.Utils;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ColoryrServer.Core.BuilderPost;

internal static class PostBuildWebSocket
{
    public static ReMessage Add(BuildOBJ json)
    {
        if (CodeManager.GetWebSocket(json.UUID) != null)
            return new ReMessage
            {
                Build = false,
                Message = $"WebSocket[{json.UUID}]已存在"
            };
        ServerMain.LogOut($"[{json.User}]正在创建WebSocket[{json.UUID}]");
        var time = string.Format("{0:s}", DateTime.Now);
        CSFileCode obj = new()
        {
            UUID = json.UUID,
            Type = CodeType.WebSocket,
            CreateTime = time,
            Code = DemoResource.WebSocket
            .Replace(CodeDemo.Name, json.UUID)
        };
        CodeManager.StorageWebSocket(obj, json.User);
        GenWebSocket.StartGen(obj, json.User);
        ServerMain.LogOut($"[{json.User}]创建WebSocket[{json.UUID}]完成");

        return new ReMessage
        {
            Build = true,
            Message = $"WebSocket[{json.UUID}]已创建"
        };
    }

    public static CSFileList GetList()
    {
        var list = new CSFileList();
        foreach (var item in CodeManager.WebSocketFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        CodeManager.RemoveFile(CodeType.WebSocket, json.UUID, json.User);
        AssemblyList.RemoveWebSocket(json.UUID);

        return new ReMessage
        {
            Build = true,
            Message = $"WebSocket[{json.UUID}]已删除"
        };
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var obj = CodeManager.GetWebSocket(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个WebSocket[{json.UUID}]"
            };
        }
        if (obj.Version != json.Version)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"WebSocket[{json.UUID}]版本号错误"
            };
        }

        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
        obj.Code = FileUtils.StartEdit(obj.Code, list);
        obj.Text = json.Text;

        var sw = new Stopwatch();
        sw.Start();
        var build = GenWebSocket.StartGen(obj, json.User);
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
