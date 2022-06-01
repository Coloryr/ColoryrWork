using System.Collections.Generic;

namespace ColoryrServer.Core.FileSystem;

public abstract record MainConfig
{
    /// <summary>
    /// Http配置
    /// </summary>
    public List<HttpConfig> Http { get; set; }
    /// <summary>
    /// Socket配置
    /// </summary>
    public SocketConfig Socket { get; set; }
    /// <summary>
    /// WebSocket配置
    /// </summary>
    public SocketConfig WebSocket { get; set; }
    /// <summary>
    /// Reboot配置
    /// </summary>
    public RebotConfigObj Robot { get; set; }
    /// <summary>
    /// Mysql配置
    /// </summary>
    public List<SQLConfig> Mysql { get; set; }
    /// <summary>
    /// MS sql设置
    /// </summary>
    public List<SQLConfig> MSsql { get; set; }
    /// <summary>
    /// Oracle配置
    /// </summary>
    public List<SQLConfig> Oracle { get; set; }
    /// <summary>
    /// SQLite配置
    /// </summary>
    public List<SQLConfig> SQLite { get; set; }
    /// <summary>
    /// Redis设置
    /// </summary>
    public List<RedisConfig> Redis { get; set; }
    /// <summary>
    /// 编辑用户设置
    /// </summary>
    public List<UserConfig> User { get; set; }
    /// <summary>
    /// 不参与动态编译的dll
    /// </summary>
    public List<string> NotInclude { get; set; }
    /// <summary>
    /// MQTT配置
    /// </summary>
    public MQTTConfig MQTTConfig { get; set; }
    /// <summary>
    /// 任务配置
    /// </summary>
    public TaskUtilConfig TaskConfig { get; set; }
    /// <summary>
    /// ffmpeg
    /// </summary>
    public string MPGE { get; set; }
    /// <summary>
    /// HttpClient数量
    /// </summary>
    public int HttpClientNumber { get; set; }
    /// <summary>
    /// 请求选项
    /// </summary>
    public RequsetChoose Requset { get; set; }
    /// <summary>
    /// 编辑器AES加密
    /// </summary>
    public AESConfig AES { get; set; }
}

public record RebotConfigObj
{
    /// <summary>
    /// 连接端口
    /// </summary>
    public SocketConfig Socket { get; set; }
    /// <summary>
    /// 订阅的包
    /// </summary>
    public List<int> Packs { get; set; }
}

public record AESConfig
{
    public string Key { get; set; }
    public string IV { get; set; }
}
public record RequsetChoose
{
    /// <summary>
    /// 前端
    /// </summary>
    public string Web { get; set; }
    /// <summary>
    /// 后端
    /// </summary>
    public string WebAPI { get; set; }
    /// <summary>
    /// 放进缓存的文件类型
    /// </summary>
    public List<string> Temp { get; set; }
    /// <summary>
    /// 缓存保留时间
    /// </summary>
    public int TempTime { get; set; }
    /// <summary>
    /// 启用文件流
    /// </summary>
    public bool Stream { get; set; }
    /// <summary>
    /// 文件流的类型
    /// </summary>
    public List<string> StreamType { get; set; }
}
public record MQTTConfig
{
    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }
}
public record TaskUtilConfig
{
    /// <summary>
    /// 线程数量
    /// </summary>
    public int ThreadNumber { get; set; }
    /// <summary>
    /// 最大运行时间
    /// </summary>
    public int MaxTime { get; set; }
}
public record UserConfig
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// 密码SHA1
    /// </summary>
    public string Password { get; set; }
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
public record HttpConfig : SocketConfig
{
}
public record SQLConfig
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// IP地址
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public string User { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 连接超时
    /// </summary>
    public int TimeOut { get; set; }
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string Conn { get; set; }
}
public record RedisConfig
{
    /// <summary>
    /// 启用
    /// </summary>
    public bool Enable { get; set; }
    /// <summary>
    /// IP地址
    /// </summary>
    public string IP { get; set; }
    /// <summary>
    /// 端口
    /// </summary>
    public short Port { get; set; }
    /// <summary>
    /// 连接字符串
    /// </summary>
    public string Conn { get; set; }
}
public abstract class ConfigUtil
{
    public static string FilePath = ServerMain.RunLocal + @"MainConfig.json";
    /// <summary>
    /// 读配置文件
    /// </summary>
    public abstract void Start();
}
