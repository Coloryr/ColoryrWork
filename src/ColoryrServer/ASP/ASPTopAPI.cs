using ColoryrServer.Core;
using ColoryrServer.Core.Http.PostBuild;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;

namespace ColoryrServer.ASP;

public class ASPTopAPI : ITopAPI
{
    public HttpListObj GetHttpConfigList(BuildOBJ json)
    {
        var list = new HttpListObj()
        {
            HttpList = new(),
            RouteList = new(),
            UrlRouteList = new(),
        };
        foreach (var item in ASPServer.Config.Http)
        {
            list.HttpList.Add(item);
        }
        foreach (var item in ASPServer.Config.Routes)
        {
            list.RouteList.Add(item.Key, item.Value);
        }
        foreach (var item in ASPServer.Config.UrlRoutes)
        {
            list.UrlRouteList.Add(item.Key, item.Value);
        }

        return list;
    }

    public ReMessage AddHttpConfig(BuildOBJ json)
    {
        SocketConfig item = new()
        {
            IP = json.Code,
            Port = json.Version
        };

        if (ASPServer.Config.Http.Contains(item))
        {
            return new ReMessage()
            {
                Build = false,
                Message = "已存在IP和端口"
            };
        }

        ASPServer.Config.Http.Add(item);
        ServerMain.ConfigUtil.Save();

        return new ReMessage()
        {
            Build = true,
            Message = "已添加Http配置"
        };
    }

    public ReMessage RemoveHttpConfig(BuildOBJ json)
    {
        if (ASPServer.Config.Http.Count == 1)
        {
            return new ReMessage()
            {
                Build = false,
                Message = "至少要有一个Http配置"
            };
        }
        int port = json.Version;
        var list = ASPServer.Config.Http.Where(a => a.IP == json.Code && a.Port == port);
        if (list.Any())
        {
            return new ReMessage()
            {
                Build = false,
                Message = "没有找到Http配置"
            };
        }

        ASPServer.Config.Http.Remove(list.First());
        ServerMain.ConfigUtil.Save();

        return new ReMessage()
        {
            Build = true,
            Message = "已删除Http配置"
        };
    }

    public ReMessage AddHttpRouteConfig(BuildOBJ json)
    {
        string targe = json.Temp;
        RouteConfigObj item = new()
        {
            Url = json.Code,
            Heads = JsonUtils.ToObj<Dictionary<string, string>>(json.Text)
        };

        if (ASPServer.Config.Routes.ContainsKey(targe))
        {
            return new ReMessage()
            {
                Build = false,
                Message = "已存在反代"
            };
        }

        ASPServer.Config.Routes.Add(targe, item);
        ServerMain.ConfigUtil.Save();

        return new ReMessage()
        {
            Build = true,
            Message = "已添加反代配置"
        };
    }

    public ReMessage RemoveHttpRouteConfig(BuildOBJ json)
    {
        string targe = json.Code;
        if (!ASPServer.Config.Routes.ContainsKey(targe))
        {
            return new ReMessage()
            {
                Build = false,
                Message = "不存在反代"
            };
        }

        ASPServer.Config.Routes.Remove(targe);
        ServerMain.ConfigUtil.Save();

        return new ReMessage()
        {
            Build = true,
            Message = "已删除反代配置"
        };
    }

    public ReMessage AddHttpUrlRouteConfig(BuildOBJ json)
    {
        string targe = json.Temp;
        RouteConfigObj item = new()
        {
            Url = json.Code,
            Heads = JsonUtils.ToObj<Dictionary<string, string>>(json.Text)
        };

        if (ASPServer.Config.UrlRoutes.ContainsKey(targe))
        {
            return new ReMessage()
            {
                Build = false,
                Message = "已存在反代"
            };
        }

        ASPServer.Config.UrlRoutes.Add(targe, item);
        ServerMain.ConfigUtil.Save();

        return new ReMessage()
        {
            Build = true,
            Message = "已添加Url反代配置"
        };
    }

    public ReMessage RemoveHttpUrlRouteConfig(BuildOBJ json)
    {
        string targe = json.Code;
        if (!ASPServer.Config.UrlRoutes.ContainsKey(targe))
        {
            return new ReMessage()
            {
                Build = false,
                Message = "不存在反代"
            };
        }

        ASPServer.Config.UrlRoutes.Remove(targe);
        ServerMain.ConfigUtil.Save();

        return new ReMessage()
        {
            Build = true,
            Message = "已删除Url反代配置"
        };
    }

    public void Reboot()
    {
        Task.Run(() =>
        {
            Thread.Sleep(5000);
            ASPServer.Reboot();
        });
    }
}
