using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ColoryrServer.Core.DllManager.PostBuild;

internal static class PostBuildClass
{
    public static ReMessage Add(BuildOBJ json)
    {
        ReMessage res;
        if (CodeFileManager.GetClass(json.UUID) == null)
        {
            var time = string.Format("{0:s}", DateTime.Now);
            var obj = new CSFileCode()
            {
                UUID = json.UUID,
                Type = CodeType.Class,
                CreateTime = time
                .Replace(CodeDemo.Name, json.UUID)
            };
            CodeFileManager.StorageClass(obj, json.UUID, DemoResource.Class.Replace(CodeDemo.Name, json.UUID));
            res = new ReMessage
            {
                Build = true,
                Message = $"Class[{json.UUID}]已创建"
            };
            GenClass.StartGen(obj);
            ServerMain.LogOut($"Class[{json.UUID}]创建");
        }
        else
            res = new ReMessage
            {
                Build = false,
                Message = $"Class[{json.UUID}]已存在"
            };

        return res;
    }

    public static CSFileList GetList()
    {
        var list = new CSFileList();
        foreach (var item in CodeFileManager.ClassFileList)
        {
            list.List.Add(item.Key, item.Value);
        }

        return list;
    }

    public static ReMessage Remove(BuildOBJ json)
    {
        CodeFileManager.RemoveFile(CodeType.Class, json.UUID);
        return new ReMessage
        {
            Build = true,
            Message = $"Class[{json.UUID}]已删除"
        };
    }

    public static ClassCodeGetObj GetCode(BuildOBJ json)
    {
        var list = CodeFileManager.GetClassCode(json.UUID);
        var obj = CodeFileManager.GetClass(json.UUID);
        return new ClassCodeGetObj { Obj = obj, List = list };
    }

    public static ReMessage Updata(BuildOBJ json)
    {
        var obj = CodeFileManager.GetClass(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Class[{json.UUID}]"
            };
        }
        if (obj.Version != json.Version)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Class[{json.UUID}]版本号错误"
            };
        }
        var code = CodeFileManager.CheckClassCode(obj, json.Temp);
        if (code == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Class[{json.UUID}]没有文件{json.Temp}"
            };
        }

        var list = JsonConvert.DeserializeObject<List<CodeEditObj>>(json.Code);

        code.Code = FileEdit.StartEdit(code.Code, list);
        obj.Text = json.Text;

        obj.Next();
        CodeFileManager.StorageClass(obj, json.Temp, code.Code);

        return new ReMessage
        {
            Build = true
        };
    }

    public static ReMessage Build(BuildOBJ json)
    {
        var obj = CodeFileManager.GetClass(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Class[{json.UUID}]"
            };
        }

        var SW = new Stopwatch();
        SW.Start();
        var buildRes = GenClass.StartGen(obj);
        SW.Stop();
        obj.Up();
        return new ReMessage
        {
            Build = buildRes.Isok,
            Message = buildRes.Res,
            UseTime = SW.ElapsedMilliseconds.ToString(),
            Time = buildRes.Time
        };
    }

    public static ReMessage AddFile(BuildOBJ json)
    {
        var obj = CodeFileManager.GetClass(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Class[{json.UUID}]"
            };
        }
        var code = CodeFileManager.CheckClassCode(obj, json.Temp);
        if (code != null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Class[{json.UUID}]的代码文件{json.Temp}已存在"
            };
        }

        CodeFileManager.StorageClass(obj, json.Temp, DemoResource.Class.Replace(CodeDemo.Name, json.Temp));

        return new ReMessage
        {
            Build = true,
            Message = $"Class[{json.UUID}]的代码文件{json.Temp}添加成功"
        };
    }

    public static ReMessage RemoveFile(BuildOBJ json)
    {
        var obj = CodeFileManager.GetClass(json.UUID);
        if (obj == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"没有这个Class[{json.UUID}]"
            };
        }
        var code = CodeFileManager.CheckClassCode(obj, json.Temp);
        if (code == null)
        {
            return new ReMessage
            {
                Build = false,
                Message = $"Class[{json.UUID}]的代码文件{json.Temp}不存在"
            };
        }

        CodeFileManager.RemoveClassCode(json.UUID, json.Temp);

        return new ReMessage
        {
            Build = true,
            Message = $"Class[{json.UUID}]的代码文件{json.Temp}删除成功"
        };
    }
}
