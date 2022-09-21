using ColoryrServer.SDK;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ColoryrWork.Lib.Debug.Object;

public record HttpRequestObj
{
    /// <summary>
    /// 请求参数
    /// </summary>
    public Dictionary<string, dynamic> Parameter { get; init; }
    /// <summary>
    /// 请求头
    /// </summary>
    public NameValueCollection RowRequest { get; init; }
    /// <summary>
    /// Cookie
    /// </summary>
    public Dictionary<string, List<string>> Cookie { get; init; }
    /// <summary>
    /// 请求体类型
    /// </summary>
    public MyContentType ContentType { get; init; }
    /// <summary>
    /// 请求体数据
    /// </summary>
    public string Data { get; init; }
    /// <summary>
    /// 请求体数据
    /// </summary>
    public byte[] Data1 { get; init; }
    /// <summary>
    /// 请求方法
    /// </summary>
    public string Method { get; init; }
}

public record HttpObj
{
    public HttpRequestObj requestObj;
    public string url;
    public string function;
    public long id;
}

public record HttpResopneObj
{
    /// <summary>
    /// 返回码
    /// </summary>
    public int ReCode { get; set; }
    /// <summary>
    /// Cookie
    /// </summary>
    public Dictionary<string, string> Cookie { get; set; }
    /// <summary>
    /// 返回头
    /// </summary>
    public Dictionary<string, string> Head { get; set; }
    /// <summary>
    /// 返回类型
    /// </summary>
    public string ContentType { get; init; }

    public byte[] Data { get; init; }

    public HttpResopneObj()
    {
        Cookie ??= new();
        Head ??= new();
        ContentType ??= ServerContentType.TXT;
    }
}

public record HttpResObj
{
    public HttpResopneObj resopneObj;
    public long id;
}