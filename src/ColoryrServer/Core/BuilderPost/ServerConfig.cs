using ColoryrServer.Core.FileSystem;
using ColoryrWork.Lib.Build.Object;
using System.Net;

namespace ColoryrServer.Core.BuilderPost;

public interface ITopAPI
{
    public HttpListObj GetHttpConfigList(BuildOBJ json);
    public ReMessage AddHttpConfig(BuildOBJ json);
    public ReMessage RemoveHttpConfig(BuildOBJ json);
    public ReMessage AddHttpRouteConfig(BuildOBJ json);
    public ReMessage RemoveHttpRouteConfig(BuildOBJ json);
    public ReMessage AddHttpUrlRouteConfig(BuildOBJ json);
    public ReMessage RemoveHttpUrlRouteConfig(BuildOBJ json);
    public ReMessage SetServerEnable(BuildOBJ json);
    public void Reboot();
}

public static class ServerConfig
{
    private static ITopAPI top;
    public static void Init(ITopAPI api)
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
    public static ReMessage SetServerEnable(BuildOBJ json)
    {
        if (json.Text == "FixMode")
        {
            bool enable = json.Code.ToLower() is "true";
            ServerMain.Config.FixMode = enable;
            ConfigUtils.Save();
            return new()
            {
                Build = true,
                Message = "设置完成"
            };
        }
        else
            return top.SetServerEnable(json);
    }
    public static SocketObj GetSocketConfig()
    {
        return new()
        {
            Socket = ServerMain.Config.Socket,
            Mqtt = ServerMain.Config.MqttConfig.Socket,
            WebSocket = ServerMain.Config.WebSocket.Socket,
            Robot = ServerMain.Config.Robot.Socket
        };
    }
    public static ReMessage SetSocketConfig(BuildOBJ json)
    {
        string ip = json.Code;
        int port = json.Version;
        if (IPAddress.TryParse(ip, out _) == false)
        {
            return new()
            {
                Build = false,
                Message = "参数非法"
            };
        }
        if (port > 0xFFFF || port < 0)
        {
            return new()
            {
                Build = false,
                Message = "参数非法"
            };
        }
        if (json.Text is "Socket")
        {
            ServerMain.Config.Socket.IP = ip;
            ServerMain.Config.Socket.Port = port;
            ConfigUtils.Save();
            return new()
            {
                Build = true,
                Message = "设置Socket配置完成"
            };
        }
        else if (json.Text is "Mqtt")
        {
            ServerMain.Config.MqttConfig.Socket.IP = ip;
            ServerMain.Config.MqttConfig.Socket.Port = port;
            ConfigUtils.Save();
            return new()
            {
                Build = true,
                Message = "设置Mqtt配置完成"
            };
        }
        else if (json.Text is "Robot")
        {
            ServerMain.Config.Robot.Socket.IP = ip;
            ServerMain.Config.Robot.Socket.Port = port;
            ConfigUtils.Save();
            return new()
            {
                Build = true,
                Message = "设置Robot配置完成"
            };
        }
        else if (json.Text is "WebSocket")
        {
            ServerMain.Config.WebSocket.Socket.IP = ip;
            ServerMain.Config.WebSocket.Socket.Port = port;
            ConfigUtils.Save();
            return new()
            {
                Build = true,
                Message = "设置WebSocket配置完成"
            };
        }
        else
        {
            return new()
            {
                Build = false,
                Message = "参数错误"
            };
        }
    }
    public static ReMessage Reboot()
    {
        top.Reboot();
        return null;
    }
}