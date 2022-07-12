using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace ColoryrServer.SDK;

public class HttpDllRequest
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
    /// 请求体流
    /// </summary>
    public Stream Stream { get; init; }
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

    /// <summary>
    /// 获取请求数据
    /// </summary>
    /// <param name="arg">参数名</param>
    /// <returns>返回</returns>
    public dynamic Get(string arg)
        => Parameter.ContainsKey(arg) ? Parameter[arg] : null;
}
public abstract class HttpDllResponse
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
    public HttpDllResponse()
    {
        if (Head == null)
            Head = new();
        ReCode = 200;
        Cookie = new();
        ContentType = ServerContentType.JSON;
    }
    /// <summary>
    /// 往返回头写数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public HttpDllResponse AddHead(string key, string value)
    {
        Head.Add(key, value);
        return this;
    }
}

public class HttpResponseString : HttpDllResponse
{
    /// <summary>
    /// 返回数据
    /// </summary>
    public string Data { get; set; }

    public HttpResponseString() : base() { }
    /// <summary>
    /// 写数据
    /// </summary>
    /// <param name="data">内容</param>
    public void Write(string data)
        => Data += data;
    /// <summary>
    /// 写数据后换行
    /// </summary>
    /// <param name="data">内容</param>
    public void WriteNewLine(string data)
       => Data += data + "\n";
}

public class HttpResponseDictionary : HttpDllResponse
{
    public Dictionary<string, object> Data { get; set; }
    /// <summary>
    /// 返回头结构体
    /// </summary>
    /// <param name="ReCode">相应代码</param>
    public HttpResponseDictionary() : base()
    {
        if (Data == null)
            Data = new();
    }
    /// <summary>
    /// 往返回的字符串写数据
    /// </summary>
    /// <param name="Key">键</param>
    /// <param name="Value">值</param>
    public void AddResponse(string Key, object Value)
        => Data.Add(Key, Value);
    /// <summary>
    /// 写数据
    /// </summary>
    /// <param name="res"></param>
    /// <param name="text"></param>
    /// <param name="data"></param>
    public HttpResponseDictionary Send(int res, string text, dynamic data = null)
    {
        Data.Add("res", res);
        Data.Add("text", text);
        Data.Add("data", data);
        return this;
    }
}
public class HttpResponseStream : HttpDllResponse
{
    /// <summary>
    /// 流
    /// </summary>
    public Stream Data { get; set; }
    /// <summary>
    /// 流的位置
    /// </summary>
    public int Pos { get; set; }
    public HttpResponseStream() : base()
    {
        if (Data == null)
            Data = new MemoryStream();
    }
}
public class HttpResponseBytes : HttpDllResponse
{
    /// <summary>
    /// 二进制
    /// </summary>
    public byte[] Data { get; set; }
    public HttpResponseBytes() : base()
    {
        if (Data == null)
            Data = Array.Empty<byte>();
    }
}
