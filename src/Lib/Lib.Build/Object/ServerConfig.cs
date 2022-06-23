using System;
using System.Collections.Generic;
using System.Text;

namespace ColoryrWork.Lib.Build.Object;

public record KVConfig
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public record SocketConfig
{
    /// <summary>
    /// socket设置的IP
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }
}

public record RouteConfigObj
{
    public string Url { get; set; }
    public Dictionary<string, string> Heads { get; set; }
}

public record SocketObj
{
    public SocketConfig Socket { get; set; }
    public SocketConfig Mqtt { get; set; }
    public SocketConfig WebSocket { get; set; }
}

public record HttpListObj
{ 
    public List<SocketConfig> HttpList { get; set; }
    public Dictionary<string, RouteConfigObj> RouteList { get; set; }
    public Dictionary<string, RouteConfigObj> UrlRouteList { get; set; }
}