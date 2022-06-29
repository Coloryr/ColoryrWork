using ColoryrWork.Lib.Build.Object;
using System.Collections.Generic;

namespace ColoryrServer.Core.FileSystem;

public abstract record MainConfig
{
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
    /// MQTT配置
    /// </summary>
    public MqttConfigObj MqttConfig { get; set; }
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
    /// <summary>
    /// 代码设置选项
    /// </summary>
    public CodeConfigObj CodeSetting { get; set; }
}

public record MqttConfigObj
{
    /// <summary>
    /// 使用SSL证书
    /// </summary>
    public bool UseSsl { get; set; }
    /// <summary>
    /// SSL证书存放位置
    /// </summary>
    public string Ssl { get; set; }
    /// <summary>
    /// SSL证书密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 绑定端口
    /// </summary>
    public SocketConfig Socket { get; set; }
}

public record CodeConfigObj
{
    /// <summary>
    /// 不参与动态编译的.dll
    /// </summary>
    public List<string> NotInclude { get; set; }
    /// <summary>
    /// 代码增量储存
    /// </summary>
    public bool CodeLog { get; set; }
    /// <summary>
    /// Html代码压缩
    /// </summary>
    public bool MinifyHtml { get; set; }
    /// <summary>
    /// JS代码压缩
    /// </summary>
    public bool MinifyJS { get; set; }
    /// <summary>
    /// CSS代码压缩
    /// </summary>
    public bool MinifyCSS { get; set; }
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
    /// <summary>
    /// 加密KEY
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// 加密盐
    /// </summary>
    public string IV { get; set; }
}
public record RequsetChoose
{
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
public record TaskUtilConfig
{
    /// <summary>
    /// 最大运行时间
    /// </summary>
    public int MaxTime { get; set; }
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
    /// <summary>
    /// 保存配置文件
    /// </summary>
    public abstract void Save();
}
