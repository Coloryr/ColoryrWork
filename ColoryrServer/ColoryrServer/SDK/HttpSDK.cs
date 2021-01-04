using System.Collections.Generic;
using System.Collections.Specialized;

namespace ColoryrServer.SDK
{
    public class HttpRequest
    {
        private Dictionary<string, dynamic> Parameter;
        public NameValueCollection RowRequest { get; private set; }//原始请求的字符串
        public string Cookie { get; private set; }
        /// <summary>
        /// <summary>
        /// 请求头结构
        /// </summary>
        /// <param name="Parameter">参数</param>
        /// <param name="RowRequest">原始请求头</param>
        /// <param name="IsValid">是否验证</param>
        /// <param name="Cookie">对话</param>
        public HttpRequest(Dictionary<string, dynamic> Parameter, NameValueCollection RowRequest, string Cookie)
        {
            this.Parameter = Parameter;
            this.RowRequest = RowRequest;
            this.Cookie = Cookie;
        }

        /// 获取参数
        /// </summary>
        /// <param name="arg">参数名</param>
        /// <returns>数据</returns>
        public string GetParameter(string arg)
        {
            if (Parameter.ContainsKey(arg))
                return Parameter[arg];
            return null;
        }
    }
    public class HttpResponse
    {
        public string Response { get; set; }
        public int ReCode { get; set; }
        public Dictionary<string, string> Head { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 返回头结构体
        /// </summary>
        /// <param name="ReCode">相应代码</param>
        public HttpResponse(int ReCode = 200)
        {
            this.ReCode = ReCode;
        }
        /// <summary>
        /// 往返回的字符串写数据
        /// </summary>
        /// <param name="str">要写的数据</param>
        public HttpResponse Write(string str)
        {
            Response += str;
            return this;
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
    public class HttpResponseSession : HttpResponse
    {
        public string Cookie { get; private set; }
        /// <summary>
        /// 带会话的返回头
        /// </summary>
        /// <param name="Cookie">会话</param>
        public HttpResponseSession(string Cookie) : base()
        {
            this.Cookie = Cookie;
        }
    }
    public class HttpResponseDictionary
    {
        public int ReCode { get; set; }
        public Dictionary<string, object> Response { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, string> Head { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 返回头结构体
        /// </summary>
        /// <param name="ReCode">相应代码</param>
        public HttpResponseDictionary(int ReCode = 200)
        {
            this.ReCode = ReCode;
        }
        /// <summary>
        /// 往返回的字符串写数据
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        public void AddResponse(string Key, object Value)
        {
            Response.Add(Key, Value);
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
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="res"></param>
        /// <param name="text"></param>
        /// <param name="data"></param>
        public HttpResponseDictionary Send(int res, string text, dynamic data = null)
        {
            Response.Add("res", res);
            Response.Add("text", text);
            Response.Add("data", data);
            return this;
        }
    }
    public class HttpResponseDictionarySession : HttpResponseDictionary
    {
        public string Cookie { get; private set; }
        /// <summary>
        /// 带会话的返回头
        /// </summary>
        /// <param name="Cookie">会话</param>
        public HttpResponseDictionarySession(string Cookie) : base()
        {
            this.Cookie = Cookie;
        }
    }
}
