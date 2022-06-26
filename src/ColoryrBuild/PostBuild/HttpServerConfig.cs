using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace ColoryrBuild.PostBuild;

public partial class HttpBuild : HttpUtilsBase
{
    /// <summary>
    /// 获取机器人设置参数
    /// </summary>
    /// <returns>结果</returns>
    public async Task<RobotObj> GetRobotConfig()
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.GetRobotConfig
        });
        if (!CheckLogin(data))
        {
            return await GetRobotConfig();
        }
        return JsonConvert.DeserializeObject<RobotObj>(data);
    }
    /// <summary>
    /// 获取端口设置参数
    /// </summary>
    /// <returns>结果</returns>
    public async Task<SocketObj> GetSocketConfig()
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.GetServerSocketConfig
        });
        if (!CheckLogin(data))
        {
            return await GetSocketConfig();
        }
        return JsonConvert.DeserializeObject<SocketObj>(data);
    }
    /// <summary>
    /// 获取Http设置参数
    /// </summary>
    /// <returns>结果</returns>
    public async Task<HttpListObj> GetHttpConfigList()
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.GetServerHttpConfigList
        });
        if (!CheckLogin(data))
        {
            return await GetHttpConfigList();
        }
        return JsonConvert.DeserializeObject<HttpListObj>(data);
    }
    /// <summary>
    /// 添加Http设置
    /// </summary>
    /// <param name="ip">IP</param>
    /// <param name="port">端口</param>
    /// <returns>设置结果</returns>
    public async Task<ReMessage> AddHttpConfig(string ip, int port)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.AddServerHttpConfig,
            Code = ip,
            Version = port
        });
        if (!CheckLogin(data))
        {
            return await AddHttpConfig(ip, port);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 删除Http设置
    /// </summary>
    /// <param name="ip">IP</param>
    /// <param name="port">端口</param>
    /// <returns>设置结果</returns>
    public async Task<ReMessage> RemoveHttpConfig(string ip, int port)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.RemoveServerHttpConfig,
            Code = ip,
            Version = port
        });
        if (!CheckLogin(data))
        {
            return await RemoveHttpConfig(ip, port);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 添加Http转发设置
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="obj">设置</param>
    /// <returns>设置结果</returns>
    public async Task<ReMessage> AddHttpRoute(string key, RouteConfigObj obj)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.AddServerHttpRoute,
            Temp = key,
            Code = JsonUtils.ToString(obj)
        });
        if (!CheckLogin(data))
        {
            return await AddHttpRoute(key, obj);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 删除Http转发设置
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>设置结果</returns>
    public async Task<ReMessage> RemoveHttpRoute(string key)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.RemoveServerHttpRoute,
            Code = key
        });
        if (!CheckLogin(data))
        {
            return await RemoveHttpRoute(key);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 添加HttpUrl转发
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="obj">值</param>
    /// <returns>设置结果</returns>
    public async Task<ReMessage> AddHttpUrlRoute(string key, RouteConfigObj obj)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.AddServerHttpUrlRoute,
            Temp = key,
            Code = JsonUtils.ToString(obj)
        });
        if (!CheckLogin(data))
        {
            return await AddHttpUrlRoute(key, obj);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 删除HttpUrl转发
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>设置结果</returns>
    public async Task<ReMessage> RemoveHttpUrlRoute(string key)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.RemoveServerHttpUrlRoute,
            Code = key
        });
        if (!CheckLogin(data))
        {
            return await RemoveHttpUrlRoute(key);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 设置服务器启用
    /// </summary>
    /// <param name="enable">启用</param>
    /// <param name="type">类型</param>
    /// <returns>设置结果</returns>
    public async Task<ReMessage> SetServerEnable(bool enable, string type)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.SetServerEnable,
            Code = enable.ToString(),
            Text = type
        });
        if (!CheckLogin(data))
        {
            return await SetServerEnable(enable, type);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 设置服务器端口
    /// </summary>
    /// <param name="ip">IP</param>
    /// <param name="port">端口</param>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public async Task<ReMessage> SetSocket(string ip, int port, string type)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.ServerConfigSetSocket,
            Code = ip,
            Version = port,
            Text = type
        });
        if (!CheckLogin(data))
        {
            return await SetSocket(ip, port, type);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 设置机器人配置
    /// </summary>
    /// <param name="ip">IP</param>
    /// <param name="port">端口</param>
    /// <param name="packs">订阅的包</param>
    /// <returns></returns>
    public async Task<ReMessage> SetRobotConfig(string ip, int port, List<int> packs)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.SetRobotConfig,
            Code = ip,
            Version = port,
            Text = JsonConvert.SerializeObject(packs)
        });
        if (!CheckLogin(data))
        {
            return await SetRobotConfig(ip, port, packs);
        }
        return JsonConvert.DeserializeObject<ReMessage>(data);
    }
    /// <summary>
    /// 重启服务器
    /// </summary>
    public async Task Reboot()
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.ServerReboot
        });
        if (!CheckLogin(data))
        {
            await Reboot();
        }
    }
}