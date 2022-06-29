﻿using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace ColoryrServer.Core.Http.PostBuild;

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

public static class PostServerConfig
{
    public static ITopAPI top;
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
        return top.SetServerEnable(json);
    }
    public static SocketObj GetSocketConfig()
    {
        return new()
        {
            Socket = ServerMain.Config.Socket,
            Mqtt = ServerMain.Config.MqttConfig.Socket,
            WebSocket = ServerMain.Config.WebSocket.Socket
        };
    }
    public static ReMessage WebSetSocket(BuildOBJ json)
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
            ServerMain.ConfigUtil.Save();
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
            ServerMain.ConfigUtil.Save();
            return new()
            {
                Build = true,
                Message = "设置Mqtt配置完成"
            };
        }
        else if (json.Text is "WebSocket")
        {
            ServerMain.Config.WebSocket.Socket.IP = ip;
            ServerMain.Config.WebSocket.Socket.Port = port;
            ServerMain.ConfigUtil.Save();
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
    public static RobotObj GetRobotConfig()
    {
        return new()
        {
            IP = ServerMain.Config.Robot.Socket.IP,
            Port = ServerMain.Config.Robot.Socket.Port,
            Packs = ServerMain.Config.Robot.Packs
        };
    }
    public static ReMessage SetRobotConfig(BuildOBJ json)
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

        List<int> list = JsonConvert.DeserializeObject<List<int>>(json.Text);
        ServerMain.Config.Robot.Socket.IP = ip;
        ServerMain.Config.Robot.Socket.Port = port;
        ServerMain.Config.Robot.Packs = list;
        ServerMain.ConfigUtil.Save();

        return new()
        {
            Build = true,
            Message = "机器人配置设置完成"
        };
    }
    public static ReMessage Reboot()
    {
        top.Reboot();
        return null;
    }
}