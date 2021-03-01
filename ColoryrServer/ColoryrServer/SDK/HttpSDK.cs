﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace ColoryrServer.SDK
{
    public class HttpRequest
    {
        public Dictionary<string, dynamic> Parameter { get; init; }
        public NameValueCollection RowRequest { get; init; }//原始请求的字符串
        public string Cookie { get; init; }
        public MyContentType ContentType { get; init; }
        public Stream Stream { get; init; }

        /// 获取参数
        /// </summary>
        /// <param name="arg">参数名</param>
        /// <returns>数据</returns>
        public string GetParameter(string arg)
            => Parameter.ContainsKey(arg) ? Parameter[arg] : null;
    }
    public abstract class HttpResponse
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public int ReCode { get; set; }
        /// <summary>
        /// Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        public bool SetCookie { get; set; }
        /// <summary>
        /// 返回头
        /// </summary>
        public Dictionary<string, string> Head { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public EncodeType EncodeType { get; set; }

        public HttpResponse()
        {
            if (Head == null)
                Head = new();
            ReCode = 200;
            SetCookie = false;
            Cookie = "";
        }
        /// <summary>
        /// 往返回头写数据
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        public void AddHead(string Key, string Value)
        {
            Head.Add(Key, Value);
        }
    }

    public class HttpResponseString : HttpResponse
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

    public class HttpResponseDictionary : HttpResponse
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
    public class HttpResponseStream : HttpResponse
    {
        /// <summary>
        /// 流
        /// </summary>
        public Stream Data { get; set; }
        public HttpResponseStream() : base()
        {
            if (Data == null)
                Data = new MemoryStream();
        }
    }
}
