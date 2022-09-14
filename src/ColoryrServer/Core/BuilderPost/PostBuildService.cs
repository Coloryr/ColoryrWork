﻿using ColoryrServer.Core.DllManager;
using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ColoryrServer.Core.BuilderPost;

internal static class PostBuildService
{
    public static ReMessage Add(BuildOBJ json)
    {
        if (CodeFileManager.GetTask(json.UUID) != null)
            return new ReMessage
            {
                Build = false,
                Message = $"Service[{json.UUID}]已存在"
            };
        ServiceType? type = json.Code switch
        {
            "0" => ServiceType.Normal,
            "1" => ServiceType.OnlyOpen,
            "2" => ServiceType.ErrorDump,
            "3" => ServiceType.Builder,
            _ => null
        };
        if (type == null)
            return new ReMessage
            {
                Build = false,
                Message = $"创建Service[{json.UUID}]错误的类型"
            };
        ServerMain.LogOut($"[{json.User}]创建Service[{json.UUID}]");
        var time = string.Format("{0:s}", DateTime.Now);
        CSFileCode obj = new()
        {
            UUID = json.UUID,
            Type = CodeType.Service,
            CreateTime = time,
            Code = type! switch
            {
                ServiceType.Normal => DemoResource.Service1,
                ServiceType.ErrorDump => DemoResource.Service2,
                ServiceType.OnlyOpen => DemoResource.Service3,
                ServiceType.Builder => DemoResource.Service4,
                _ => ""
            }
        };
        obj.Code = obj.Code.Replace(CodeDemo.Name, json.UUID);
        CodeFileManager.StorageTask(obj, json.User);
        GenService.StartGen(obj, json.User);
        ServerMain.LogOut($"[{json.User}]创建Service[{json.UUID}]完成");

        return new ReMessage
        {
            Build = true,
            Message = $"Service[{json.UUID}]已创建"
        };
    }

    public static CSFileList GetList()
    {
        var list = new CSFileList();
        foreach (var item in CodeFileManager.TaskFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        CodeFileManager.RemoveFile(CodeType.Service, json.UUID, json.User);
        return new ReMessage
        {
            Build = true,
            Message = $"Task[{json.UUID}]已删除"
        };
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var obj = CodeFileManager.GetTask(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Task[{json.UUID}]"
            };
        }
        if (obj.Version != json.Version)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Task[{json.UUID}]版本号错误"
            };
        }

        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);
        obj.Code = FileEdit.StartEdit(obj.Code, list);
        obj.Text = json.Text;

        var sw = new Stopwatch();
        sw.Start();
        var build = GenService.StartGen(obj, json.User);
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
