using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Build;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColoryrBuild.PostBuild;

public partial class HttpBuild : HttpUtilsBase
{
    /// <summary>
    /// 设置Vue项目
    /// </summary>
    /// <param name="obj">项目</param>
    /// <param name="set">设置值</param>
    /// <returns>服务器结果</returns>
    public async Task<ReMessage?> SetIsVue(WebObj obj, bool set)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.WebSetIsVue,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Code = set.ToString()
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await SetIsVue(obj, set);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
    /// <summary>
    /// 代码压缩包
    /// </summary>
    /// <param name="obj">项目</param>
    /// <param name="file">压缩包内容</param>
    /// <returns>服务器结果</returns>
    public async Task<ReMessage?> WebCodeZIP(WebObj obj, string file)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.WebCodeZIP,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Code = file
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await WebCodeZIP(obj, file);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
    /// <summary>
    /// 修改文件
    /// </summary>
    /// <param name="obj">项目</param>
    /// <param name="name">文件名</param>
    /// <param name="list">修改那日</param>
    /// <returns>服务器结果</returns>
    public async Task<ReMessage?> WebFileEdit(WebObj obj, string name, List<CodeEditObj> list)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.UpdataWeb,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Temp = name,
            Code = JsonUtils.ToString(list)
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await WebFileEdit(obj, name, list);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="obj">工程对象</param>
    /// <param name="name">文件名</param>
    /// <returns>结果</returns>
    public async Task<ReMessage?> WebDownloadFile(WebObj obj, string name)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.WebDownloadFile,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Code = name
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await WebDownloadFile(obj, name);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
    /// <summary>
    /// 添加Web项目
    /// </summary>
    /// <param name="name">名字</param>
    /// <param name="isVue">Vue项目</param>
    /// <returns>结果</returns>
    public async Task<ReMessage?> AddWeb(string name, bool isVue)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.AddWeb,
            UUID = name,
            Code = isVue.ToString()
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await AddWeb(name, isVue);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
    /// <summary>
    /// 获取Web代码
    /// </summary>
    /// <param name="name">文件名</param>
    /// <returns>结果</returns>
    public async Task<WebObj?> GetWebCode(string name)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.CodeWeb,
            UUID = name
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await GetWebCode(name);
        }
        return JsonUtils.ToObj<WebObj>(data);
    }
    /// <summary>
    /// 构建Web项目
    /// </summary>
    /// <param name="obj">Web工程</param>
    /// <returns>结果</returns>
    public async Task<ReMessage?> BuildWeb(CSFileObj obj)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.WebBuild,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await BuildWeb(obj);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
    /// <summary>
    /// 获取构建Web项目结果
    /// </summary>
    /// <param name="obj">Web工程</param>
    /// <returns>结果</returns>
    public async Task<ReMessage?> BuildWebRes(CSFileObj obj)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.WebBuildRes,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await BuildWebRes(obj);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
    /// <summary>
    /// 添加Web代码
    /// </summary>
    /// <param name="obj">Web工程</param>
    /// <param name="name">文件名</param>
    /// <returns>结果</returns>
    public async Task<ReMessage?> AddWebCode(CSFileObj obj, string name)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.WebAddCode,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Code = name
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await AddWebCode(obj, name);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
    /// <summary>
    /// 添加Web文件
    /// </summary>
    /// <param name="obj">Web工程</param>
    /// <param name="name">文件名</param>
    /// <param name="by">数据</param>
    /// <returns>结果</returns>
    public async Task<ReMessage?> AddWebFile(CSFileObj obj, string name, string by)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.WebAddFile,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Code = name,
            Temp = by
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await AddWebFile(obj, name, by);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
    /// <summary>
    /// 删除Web文件
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns>结果</returns>
    public async Task<ReMessage?> WebRemoveFile(CSFileObj obj, string name)
    {
        var data = await DoPost(new BuildOBJ
        {
            User = App.Config.Name,
            Token = App.Config.Token,
            Mode = PostBuildType.WebRemoveFile,
            UUID = obj.UUID,
            Version = obj.Version,
            Text = obj.Text,
            Code = name
        });
        if (data == null)
            return null;
        if (!CheckLogin(data))
        {
            return await WebRemoveFile(obj, name);
        }
        return JsonUtils.ToObj<ReMessage>(data);
    }
}
