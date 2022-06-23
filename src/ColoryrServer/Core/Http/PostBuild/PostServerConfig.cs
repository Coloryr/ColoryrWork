using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrServer.Core.Http.PostBuild;

public interface TopAPI
{
    public HttpListObj GetHttpConfigList(BuildOBJ json);
    public ReMessage AddHttpConfig(BuildOBJ json);
    public ReMessage RemoveHttpConfig(BuildOBJ json);
    public ReMessage AddHttpRouteConfig(BuildOBJ json);
    public ReMessage RemoveHttpRouteConfig(BuildOBJ json);
    public ReMessage AddHttpUrlRouteConfig(BuildOBJ json);
    public ReMessage RemoveHttpUrlRouteConfig(BuildOBJ json);
    public void Reboot();
}

public static class PostServerConfig
{
    public static TopAPI top;
    public static void Init(TopAPI api)
    {
        top = api;
    }
    public static HttpListObj GetHttpConfigList(BuildOBJ json)
    {
        return top.GetHttpConfigList(json);
    }
    public static ReMessage AddHttpConfig(BuildOBJ json)
    {
        return top.AddHttpConfig(json);
    }
    public static ReMessage RemoveHttpConfig(BuildOBJ json)
    {
        return top.RemoveHttpConfig(json);
    }
    public static ReMessage AddHttpRouteConfig(BuildOBJ json)
    {
        return top.AddHttpRouteConfig(json);
    }
    public static ReMessage RemoveHttpRouteConfig(BuildOBJ json)
    {
        return top.RemoveHttpRouteConfig(json);
    }
    public static ReMessage AddHttpUrlRouteConfig(BuildOBJ json)
    {
        return top.AddHttpUrlRouteConfig(json);
    }
    public static ReMessage RemoveHttpUrlRouteConfig(BuildOBJ json)
    {
        return top.RemoveHttpUrlRouteConfig(json);
    }
    public static SocketObj GetSocketConfig(BuildOBJ json)
    {
        return new()
        {
            Socket = ServerMain.Config.Socket,
            Mqtt = ServerMain.Config.MqttConfig,
            WebSocket = ServerMain.Config.WebSocket
        };
    }
    public static ReMessage Reboot()
    {
        top.Reboot();
        return null;
    }
}