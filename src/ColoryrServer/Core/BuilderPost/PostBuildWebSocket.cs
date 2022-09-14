using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ColoryrServer.Core.BuilderPost;

internal class PostBuildWebSocket
{
    public static ReMessage Add(BuildOBJ json)
    {
        ReMessage res;
        if (CodeFileManager.GetWebSocket(json.UUID) == null)
        {
            var time = string.Format("{0:s}", DateTime.Now);
            CSFileCode obj = new()
            {
                UUID = json.UUID,
                Type = CodeType.WebSocket,
                CreateTime = time,
                Code = DemoResource.WebSocket
                .Replace(CodeDemo.Name, json.UUID)
            };
            CodeFileManager.StorageWebSocket(obj, json.User);
            res = new ReMessage
            {
                Build = true,
                Message = $"WebSocket[{json.UUID}]已创建"
            };
            GenWebSocket.StartGen(obj, json.User);
            ServerMain.LogOut($"[{json.User}]创建WebSocket[{json.UUID}]");
        }
        else
            res = new ReMessage
            {
                Build = false,
                Message = $"WebSocket[{json.UUID}]已存在"
            };

        return res;
    }

    public static CSFileList GetList()
    {
        var list = new CSFileList();
        foreach (var item in CodeFileManager.WebSocketFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        CodeFileManager.RemoveFile(CodeType.WebSocket, json.UUID, json.User);
        return new ReMessage
        {
            Build = true,
            Message = $"WebSocket[{json.UUID}]已删除"
        };
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var obj = CodeFileManager.GetWebSocket(json.UUID);
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
        obj.Code = FileEdit.StartEdit(obj.Code, list);
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
