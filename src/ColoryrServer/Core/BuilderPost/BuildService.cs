﻿using ColoryrServer.Core.Dll;
using ColoryrServer.Core.Dll.Gen;
using ColoryrServer.Core.FileSystem;
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

internal static class BuildService
{
    public static ReMessage Add(BuildObj json)
    {
        if (CodeManager.GetService(json.UUID) != null)
            return new ReMessage
            {
                Build = false,
                Message = $"Service[{json.UUID}]已存在"
            };
        ServiceType? type = json.Code switch
        {
            "1" => ServiceType.Normal,
            "2" => ServiceType.ErrorDump,
            "3" => ServiceType.OnlyOpen,
            "4" => ServiceType.Builder,
            _ => null
        };
        if (type == null)
            return new ReMessage
            {
                Build = false,
                Message = $"创建Service[{json.UUID}]错误的类型"
            };
        ServerMain.LogOut($"[{json.User}]正在创建Service[{json.UUID}]");
        var time = string.Format("{0:s}", DateTime.Now);
        CSFileCode obj = new()
        {
            UUID = json.UUID,
            Type = CodeType.Service,
            CreateTime = time,
            Code = type! switch
            {
                ServiceType.Normal => Encoding.UTF8.GetString(DemoResource.Service1),
                ServiceType.ErrorDump => Encoding.UTF8.GetString(DemoResource.Service2),
                ServiceType.OnlyOpen => Encoding.UTF8.GetString(DemoResource.Service3),
                ServiceType.Builder => Encoding.UTF8.GetString(DemoResource.Service4),
                _ => ""
            }
        };
        obj.Code = obj.Code.Replace(CodeDemo.Name, json.UUID);
        CodeManager.StorageService(obj, json.User);
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
        foreach (var item in CodeManager.ServiceFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildObj json)
    {
        CodeManager.RemoveFile(CodeType.Service, json.UUID, json.User);
        AssemblyList.RemoveService(json.UUID);

        return new ReMessage
        {
            Build = true,
            Message = $"Task[{json.UUID}]已删除"
        };
    }

    public static ReMessage Updata(BuildObj json)
    {
        var obj = CodeManager.GetService(json.UUID);
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

        var list = JsonUtils.ToObj<List<CodeEditObj>>(json.Code);
        if (list == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Task[{json.UUID}]代码补丁错误"
            };
        }
        obj.Code = FileUtils.StartEdit(obj.Code, list);
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
