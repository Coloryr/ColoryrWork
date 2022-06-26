﻿using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ColoryrBuild.PostBuild;

public partial class HttpBuild : HttpUtilsBase
{
    /// <summary>
    /// 获取代码列表
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>代码列表</returns>
    public async Task<CSFileList> GetList(CodeType type)
    {
        var reType = type switch
        {
            CodeType.Class => PostBuildType.GetClass,
            CodeType.Socket => PostBuildType.GetSocket,
            CodeType.Robot => PostBuildType.GetRobot,
            CodeType.WebSocket => PostBuildType.GetWebSocket,
            CodeType.Mqtt => PostBuildType.GetMqtt,
            CodeType.Task => PostBuildType.GetTask,
            CodeType.Web => PostBuildType.GetWeb,
            CodeType.Dll => PostBuildType.GetDll,
        };
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = reType
        });
        if (!CheckLogin(data))
        {
            return await GetList(type);
        }
        return JsonConvert.DeserializeObject<CSFileList>(data);
    }
    /// <summary>
    /// Class代码编辑
    /// </summary>
    /// <param name="obj">代码类型</param>
    /// <param name="file">文件名</param>
    /// <param name="list">修改内容</param>
    /// <returns>结果</returns>
    public async Task<ReMessage> ClassFileEdit(CSFileCode obj, string file, List<CodeEditObj> list)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.UpdataClass,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Temp = file,
            Code = JsonConvert.SerializeObject(list)
        });
        if (!CheckLogin(data))
        {
            return await ClassFileEdit(obj, file, list);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 添加代码项目
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">名字</param>
    /// <param name="arg">附加参数</param>
    /// <returns>结果</returns>
    public async Task<ReMessage> AddObj(CodeType type, string name, string arg = null)
    {
        var reType = type switch
        {
            CodeType.Class => PostBuildType.AddClass,
            CodeType.Socket => PostBuildType.AddSocket,
            CodeType.Robot => PostBuildType.AddRobot,
            CodeType.WebSocket => PostBuildType.AddWebSocket,
            CodeType.Mqtt => PostBuildType.AddMqtt,
            CodeType.Task => PostBuildType.AddTask,
            CodeType.Web => PostBuildType.AddWeb,
            _ => PostBuildType.AddDll,
        };
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = reType,
            UUID = name,
            Code = arg
        });
        if (!CheckLogin(data))
        {
            return await AddObj(type, name);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 删除代码项目
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="obj">代码项目</param>
    /// <returns>结果</returns>
    public async Task<ReMessage> RemoveObj(CodeType type, CSFileObj obj)
    {
        var reType = type switch
        {
            CodeType.Class => PostBuildType.RemoveClass,
            CodeType.Socket => PostBuildType.RemoveSocket,
            CodeType.Robot => PostBuildType.RemoveRobot,
            CodeType.WebSocket => PostBuildType.RemoveWebSocket,
            CodeType.Mqtt => PostBuildType.RemoveMqtt,
            CodeType.Task => PostBuildType.RemoveTask,
            CodeType.Web => PostBuildType.RemoveWeb,
            _ => PostBuildType.RemoveDll,
        };
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = reType,
            UUID = obj.UUID
        });
        if (!CheckLogin(data))
        {
            return await RemoveObj(type, obj);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 获取项目代码
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">名字</param>
    /// <returns>结果</returns>
    public async Task<CSFileCode> GetCode(CodeType type, string name)
    {
        var reType = type switch
        {
            CodeType.Class => PostBuildType.CodeClass,
            CodeType.Socket => PostBuildType.CodeSocket,
            CodeType.Robot => PostBuildType.CodeRobot,
            CodeType.WebSocket => PostBuildType.CodeWebSocket,
            CodeType.Mqtt => PostBuildType.CodeMqtt,
            CodeType.Task => PostBuildType.CodeTask,
            _ => PostBuildType.CodeDll,
        };
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = reType,
            UUID = name
        });
        if (!CheckLogin(data))
        {
            return await GetCode(type, name);
        }
        return JsonConvert.DeserializeObject<CSFileCode>(data);
    }
    /// <summary>
    /// 构建代码
    /// </summary>
    /// <param name="obj">代码对象</param>
    /// <param name="type">类型</param>
    /// <param name="list">修改内容</param>
    /// <returns>结果</returns>
    public async Task<ReMessage> Build(CSFileObj obj, CodeType type, List<CodeEditObj> list)
    {
        var reType = type switch
        {
            CodeType.Class => PostBuildType.BuildClass,
            CodeType.Socket => PostBuildType.UpdataSocket,
            CodeType.Robot => PostBuildType.UpdataRobot,
            CodeType.WebSocket => PostBuildType.UpdataWebSocket,
            CodeType.Mqtt => PostBuildType.UpdataMqtt,
            CodeType.Task => PostBuildType.UpdataTask,
            _ => PostBuildType.UpdataDll,
        };
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = reType,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Code = JsonConvert.SerializeObject(list)
        });
        if (!CheckLogin(data))
        {
            return await Build(obj, type, list);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 获取API代码
    /// </summary>
    /// <returns>结果</returns>
    public async Task<APIFileObj> GetApi()
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.GetApi
        });
        if (!CheckLogin(data))
        {
            return await GetApi();
        }
        return JsonConvert.DeserializeObject<APIFileObj>(data);
    }
    /// <summary>
    /// 添加Class文件
    /// </summary>
    /// <param name="obj">代码对象</param>
    /// <param name="file">文件名</param>
    /// <returns>结果</returns>
    public async Task<ReMessage> AddClassFile(CSFileObj obj, string file)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.AddClassFile,
            UUID = obj.UUID,
            Temp = file
        });
        if (!CheckLogin(data))
        {
            return await AddClassFile(obj, file);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 删除Class文件
    /// </summary>
    /// <param name="obj">代码对象</param>
    /// <param name="file">文件名</param>
    /// <returns>结果</returns>
    public async Task<ReMessage> RemoveClassFile(CSFileObj obj, string file)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.RemoveClassFile,
            UUID = obj.UUID,
            Temp = file
        });
        if (!CheckLogin(data))
        {
            return await AddClassFile(obj, file);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 获取Class代码
    /// </summary>
    /// <param name="obj">代码对象</param>
    /// <returns>结果</returns>
    public async Task<ClassCodeGetObj> GetClassCode(CSFileObj obj)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.CodeClass,
            UUID = obj.UUID
        });
        if (!CheckLogin(data))
        {
            return await GetClassCode(obj);
        }
        return JsonConvert.DeserializeObject<ClassCodeGetObj>(data);
    }
}
