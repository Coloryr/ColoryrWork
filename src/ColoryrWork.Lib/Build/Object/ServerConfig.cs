using System.Collections.Generic;

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
    public SocketConfig Robot { get; set; }
}

public record HttpListObj
{
    public bool EnableRoute { get; set; }
    public List<SocketConfig> HttpList { get; set; }
    public Dictionary<string, RouteConfigObj> RouteList { get; set; }
    public Dictionary<string, RouteConfigObj> UrlRouteList { get; set; }
}

public record RobotObj
{
    public string IP { get; set; }
    public int Port { get; set; }
    public List<int> Packs { get; set; }
}

public record UserList
{
    public List<UserObj> List { get; set; }
}

public record UserObj
{
    public string User { get; set; }
    public string Password { get; set; }
    public string Time { get; set; }
}