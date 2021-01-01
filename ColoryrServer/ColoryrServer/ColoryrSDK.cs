using ColoryrServer.DataBase;
using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.IoT;
using ColoryrServer.Robot;
using ColoryrServer.Utils;
using ColoryrServer.WebSocket;
using Fleck;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    public class Mysql
    {
        private string Database;
        /// <summary>
        /// Mysql数据库
        /// </summary>
        /// <param name="Database">数据库名</param>
        public Mysql(string Database)
        {
            if (MysqlCon.State == false)
                throw new VarDump("Mysql未就绪");
            if (string.IsNullOrWhiteSpace(Database))
                throw new VarDump("没有选择数据库");
            this.Database = Database;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>返回的数据</returns>
        public List<List<dynamic>> MysqlSql(string sql, Dictionary<string, string> arg)
        {
            var com = GenCommand(sql, arg);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            return MysqlCon.MysqlSql(com, Database);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">Mysql参数</param>
        /// <returns>执行结果</returns>
        public List<List<dynamic>> MysqlSql(string sql, MySqlParameter[] arg)
        {
            var com = new MySqlCommand(sql);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            com.Parameters.AddRange(arg);
            return MysqlCon.MysqlSql(com, Database);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="arg">Mysql命令语句</param>
        /// <returns>执行结果</returns>
        public List<List<dynamic>> MysqlSql(MySqlCommand arg)
        {
            return MysqlCon.MysqlSql(arg, Database);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>Mysql命令语句</returns>
        public MySqlCommand MysqlCommand(string sql, Dictionary<string, string> arg)
        {
            var com = GenCommand(sql, arg);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            MysqlCon.MysqlSql(com, Database);
            return com;
        }
        private MySqlCommand GenCommand(string sql, Dictionary<string, string> arg)
        {
            var com = new MySqlCommand(sql);
            if (arg != null)
                foreach (var item in arg)
                {
                    com.Parameters.Add(new MySqlParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                }
            return com;
        }
    }

    public class MSsql
    {
        private string Database;
        /// <summary>
        /// MSsql数据库
        /// </summary>
        /// <param name="Database">数据库名</param>
        public MSsql(string Database)
        {
            if (MSCon.State == false)
                throw new VarDump("MS数据库没有链接");
            if (string.IsNullOrWhiteSpace(Database))
                throw new VarDump("没有选择数据库");
            this.Database = Database;
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>返回的数据</returns>
        public List<List<dynamic>> MSsqlSql(string sql, Dictionary<string, string> arg)
        {
            var a = GenCommand(sql, arg);
            if (a == null)
                throw new VarDump("SQL语句参数非法");
            return MSCon.MSsqlSql(a, Database);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">Mysql参数</param>
        /// <returns>执行结果</returns>
        public List<List<dynamic>> MSsqlSql(string sql, MySqlParameter[] arg)
        {
            var com = new SqlCommand(sql);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            com.Parameters.AddRange(arg);
            return MSCon.MSsqlSql(com, Database);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>Mysql命令语句</returns>
        public SqlCommand MysqlCommand(string sql, Dictionary<string, string> arg)
        {
            var com = GenCommand(sql, arg);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            MSCon.MSsqlSql(com, Database);
            return com;
        }
        private SqlCommand GenCommand(string sql, Dictionary<string, string> arg)
        {
            var com = new SqlCommand(sql);
            if (arg != null)
                foreach (var item in arg)
                {
                    com.Parameters.Add(new SqlParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                }
            return com;
        }
    }

    public class Oracle
    {
        private string Database;
        /// <summary>
        /// MSsql数据库
        /// </summary>
        /// <param name="Database">数据库名</param>
        public Oracle(string Database)
        {
            if (OracleCon.State == false)
                throw new VarDump("Oracle没有链接");
            if (string.IsNullOrWhiteSpace(Database))
                throw new VarDump("没有选择数据库");
            this.Database = Database;
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>返回的数据</returns>
        public List<List<dynamic>> OracleSql(string sql, Dictionary<string, string> arg)
        {
            var a = GenCommand(sql, arg);
            if (a == null)
                throw new VarDump("SQL语句参数非法");
            return OracleCon.OracleSql(a, Database);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">Mysql参数</param>
        /// <returns>执行结果</returns>
        public List<List<dynamic>> OracleSql(string sql, OracleParameter[] arg)
        {
            var com = new OracleCommand(sql);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            com.Parameters.AddRange(arg);
            return OracleCon.OracleSql(com, Database);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="arg">参数</param>
        /// <returns>Mysql命令语句</returns>
        public OracleCommand OracleCommand(string sql, Dictionary<string, string> arg)
        {
            var com = GenCommand(sql, arg);
            if (com == null)
                throw new VarDump("SQL语句参数非法");
            OracleCon.OracleSql(com, Database);
            return com;
        }
        private OracleCommand GenCommand(string sql, Dictionary<string, string> arg)
        {
            var com = new OracleCommand(sql);
            if (arg != null)
                foreach (var item in arg)
                {
                    com.Parameters.Add(new OracleParameter(item.Key, Tools.GBKtoUTF8(item.Value)));
                }
            return com;
        }
    }

    public class Redis
    {
        public Redis()
        {
            if (RedisCon.State == false)
                throw new VarDump("Redis没有链接");

        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public object Get(dynamic key)
        {
            return RedisCon.Get(key);
        }
        /// <summary>
        /// 设置键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="Time">存在秒</param>
        public bool Set(dynamic key, dynamic value, int Time = 0)
        {
            return RedisCon.Set(key, value, Time);
        }
        /// <summary>
        /// 是否存在键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public bool Exists(dynamic key)
        {
            return RedisCon.Exists(key);
        }
        /// <summary>
        /// 删除键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功</returns>
        public bool Remove(dynamic key)
        {
            return RedisCon.Remove(key);
        }
        /// <summary>
        /// 累加
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>累加后的数据</returns>
        public long Increment(string key, long val = 1)
        {
            return RedisCon.Increment(key, val);
        }
    }
    public class Session
    {
        public string Cookie { get; private set; }
        /// <summary>
        /// 启用会话
        /// </summary>
        /// <param name="Cookie">会话</param>
        public Session(string Cookie)
        {
            this.Cookie = Cookie;
        }
        /// <summary>
        /// 开始会话
        /// </summary>
        public void Start()
        {
            Cookie = SessionCheck.GetCookie(Cookie);
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回值</returns>
        public dynamic Get(string key)
        {
            return SessionCheck.GetCache(Cookie, key);
        }
        /// <summary>
        /// 设置键值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(string key, dynamic value)
        {
            SessionCheck.SetCache(Cookie, key, value);
        }
        /// <summary>
        /// 检查是否有Cookie
        /// </summary>
        /// <returns>是否存在</returns>
        public bool HaveCookie()
        {
            return SessionCheck.HaveCookie(Cookie);
        }
        /// <summary>
        /// 检查是否有键
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>是否存在</returns>
        public bool HaveKey(string key)
        {
            return SessionCheck.HaveKey(Cookie, key);
        }
        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {
            SessionCheck.SessionClose(Cookie);
        }
    }
    /// <summary>
    /// IoT设备请求头
    /// </summary>
    public class IoTRequest
    {
        public string Name { get; private set; }
        public byte[] Data { get; private set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="Name">IoT设备注册的名字</param>
        /// <param name="Data">IoT发送的数据</param>
        public IoTRequest(string Name, byte[] Data)
        {
            this.Name = Name;
            this.Data = Data;
        }
    }
    public class IoT
    {
        /// <summary>
        /// 获取IoT设备列表
        /// </summary>
        /// <returns>设备列表</returns>
        public static List<string> GetIoTList()
        {
            return IoTSocket.GetList();
        }
        /// <summary>
        /// 向IoT设备发送字符串
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">字符串</param>
        public static void Send(string Name, string Data)
        {
            IoTPackDo.SendPack(Name, Encoding.UTF8.GetBytes(Data));
        }
        /// <summary>
        /// 向IoT设备发送数据
        /// </summary>
        /// <param name="Name">设备名</param>
        /// <param name="Data">数据</param>
        public static void Send(string Name, byte[] Data)
        {
            IoTPackDo.SendPack(Name, Data);
        }
    }
    public class Html : IDisposable
    {
        private HttpClient Http;
        private CancellationToken Cancel;

        protected Html(TimeSpan timeOut, CookieContainer CookieContainer,
            CancellationToken CancellationToken = new CancellationToken(),
            Dictionary<string, string> Head = null)
        {
            if (CookieContainer == null)
            {
                throw new VarDump("cookie储存不能为空");
            }
            var Handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            };
            Http = new HttpClient(Handler);
            Http.Timeout = timeOut;
            if (Head != null)
            {
                foreach (var Item in Head)
                {
                    Http.DefaultRequestHeaders.Add(Item.Key, Item.Value);
                }
            }
            else
            {
                Http.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");
            }
            Cancel = CancellationToken;
        }

        /// <summary>
        /// http爬虫
        /// </summary>
        /// <param name="cookieContainer">cookie储存对象</param>
        public Html() : this(TimeSpan.FromSeconds(10), new CookieContainer())
        {
        }

        /// <summary>
        /// http爬虫
        /// </summary>
        /// <param name="cookieContainer">cookie储存对象</param>
        public Html(CookieContainer cookieContainer) : this(TimeSpan.FromSeconds(10), cookieContainer)
        {
        }
        ~Html()
        {
            Dispose();
        }
        /// <summary>
        /// Http爬虫
        /// </summary>
        /// <param name="timeOut">请求超时</param>
        /// <param name="cancel">取消请求</param>
        /// <param name="head">请求头</param>
        /// <param name="cookieContainer">cookie储存对象</param>
        public Html(TimeSpan timeOut, CancellationToken cancel,
            Dictionary<string, string> head, CookieContainer cookieContainer) : this(timeOut, cookieContainer, cancel, head)
        {
        }
        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>byte</returns>
        public byte[] GetByte(string url)
        {
            return Http.GetByteArrayAsync(url, Cancel).Result;
        }
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>字符串</returns>
        public string GetString(string url)
        {
            return Http.GetStringAsync(url, Cancel).Result;
        }
        /// <summary>
        /// 发送表单获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">参数</param>
        /// <returns>字符串</returns>
        public string PostString(string url, Dictionary<string, string> arg)
        {
            return Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel).Result.Content.ReadAsStringAsync().Result;
        }
        /// <summary>
        /// 获取解析后的html
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>解析后的html</returns>
        public HtmlDoc GetWebHtml(string url)
        {
            var data = new HtmlDoc(GetString(url));
            return data;
        }
        /// <summary>
        /// 发送表单获取解析后的html
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns></returns>
        public HtmlDoc PostWebHtml(string url, Dictionary<string, string> arg)
        {
            var data = new HtmlDoc(PostString(url, arg));
            return data;
        }
        /// <summary>
        /// 进行一次http请求
        /// </summary>
        /// <param name="httpRequest">请求结构</param>
        /// <returns>返回结构</returns>
        public HttpResponseMessage Do(HttpRequestMessage httpRequest)
        {
            return Http.SendAsync(httpRequest, Cancel).Result;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="data">数据</param>
        /// <returns>返回的字符串</returns>
        public string PutString(string url, byte[] data)
        {
            return Http.PutAsync(url, new ByteArrayContent(data), Cancel).Result.Content.ReadAsStringAsync().Result;
        }
        /// <summary>
        /// 发送表单数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns>返回结构</returns>
        public HttpResponseMessage PostData(string url, Dictionary<string, string> arg)
        {
            return Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel).Result;
        }
        /// <summary>
        /// Get获取数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>返回结构</returns>
        public HttpResponseMessage GetData(string url)
        {
            return Http.GetAsync(url, Cancel).Result;
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Http.Dispose();
        }
    }

    public class HtmlAsync : IDisposable
    {
        private HttpClient Http;
        private CancellationToken Cancel;
        protected HtmlAsync(TimeSpan timeOut, CookieContainer CookieContainer,
            CancellationToken CancellationToken = new CancellationToken(),
            Dictionary<string, string> Head = null)
        {
            if (CookieContainer == null)
            {
                throw new VarDump("cookie储存不能为空");
            }
            var Handler = new HttpClientHandler()
            {
                CookieContainer = CookieContainer
            };
            Http = new HttpClient(Handler);
            Http.Timeout = timeOut;
            if (Head != null)
            {
                foreach (var Item in Head)
                {
                    Http.DefaultRequestHeaders.Add(Item.Key, Item.Value);
                }
            }
            else
            {
                Http.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 Edg/81.0.416.77");
            }
            Cancel = CancellationToken;
        }
        /// <summary>
        /// http爬虫
        /// </summary>
        /// <param name="cookieContainer">cookie储存对象</param>
        public HtmlAsync() : this(TimeSpan.FromSeconds(10), new CookieContainer())
        {
        }

        /// <summary>
        /// http爬虫
        /// </summary>
        /// <param name="cookieContainer">cookie储存对象</param>
        public HtmlAsync(CookieContainer cookieContainer) : this(TimeSpan.FromSeconds(10), cookieContainer)
        {
        }
        ~HtmlAsync()
        {
            Dispose();
        }
        /// <summary>
        /// Http爬虫
        /// </summary>
        /// <param name="timeOut">请求超时</param>
        /// <param name="cancel">取消请求</param>
        /// <param name="head">请求头</param>
        /// <param name="cookieContainer">cookie储存对象</param>
        public HtmlAsync(TimeSpan timeOut, CancellationToken cancel,
            Dictionary<string, string> head, CookieContainer cookieContainer) : this(timeOut, cookieContainer, cancel, head)
        {
        }
        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>byte</returns>
        public Task<byte[]> GetByte(string url)
        {
            return Http.GetByteArrayAsync(url, Cancel);
        }
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>字符串</returns>
        public Task<string> GetString(string url)
        {
            return Http.GetStringAsync(url, Cancel);
        }
        /// <summary>
        /// 发送表单获取字符串
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">参数</param>
        /// <returns>字符串</returns>
        public async Task<string> PostString(string url, Dictionary<string, string> arg)
        {
            var temp = await Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel);
            return await temp.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// 获取解析后的html
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>解析后的html</returns>
        public async Task<HtmlDoc> GetWebHtml(string url)
        {
            var data = new HtmlDoc(await GetString(url));
            return data;
        }
        /// <summary>
        /// 发送表单获取解析后的html
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns></returns>
        public async Task<HtmlDoc> PostWebHtml(string url, Dictionary<string, string> arg)
        {
            var data = new HtmlDoc(await PostString(url, arg));
            return data;
        }
        /// <summary>
        /// 进行一次http请求
        /// </summary>
        /// <param name="httpRequest">请求结构</param>
        /// <returns>返回结构</returns>
        public async Task<HttpResponseMessage> DoString(HttpRequestMessage httpRequest)
        {
            return await Http.SendAsync(httpRequest, Cancel);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="data">数据</param>
        /// <returns>返回的字符串</returns>
        public async Task<string> PutString(string url, byte[] data)
        {
            var temp = await Http.PutAsync(url, new ByteArrayContent(data), Cancel);
            return await temp.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// 发送表单数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="arg">数据</param>
        /// <returns>返回结构</returns>
        public async Task<HttpResponseMessage> PostData(string url, Dictionary<string, string> arg)
        {
            return await Http.PostAsync(url, new FormUrlEncodedContent(arg), Cancel);
        }
        /// <summary>
        /// Get获取数据
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>返回结构</returns>
        public async Task<HttpResponseMessage> GetData(string url)
        {
            return await Http.GetAsync(url, Cancel);
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Http.Dispose();
        }
    }
    public class HtmlDoc
    {
        public HtmlDocument html;
        /// <summary>
        /// Html解析
        /// </summary>
        /// <param name="data">Html字符串</param>
        public HtmlDoc(string data)
        {
            html = new HtmlDocument();
            html.LoadHtml(data);
        }
        /// <summary>
        /// Html解析
        /// </summary>
        /// <param name="data">Html节点</param>
        public HtmlDoc(HtmlNode data)
        {
            html = new HtmlDocument();
            html.LoadHtml(data.InnerHtml);
        }
        /// <summary>
        /// 选择节点
        /// </summary>
        /// <param name="NodeName">标签名</param>
        /// <param name="ClassName">class名字</param>
        /// <returns></returns>
        public List<HtmlNode> Select(string NodeName, string ClassName)
        {
            try
            {
                return html.DocumentNode.Descendants(NodeName)
                        .Where(x => x.Attributes.Contains("class")
                        && x.Attributes["class"].Value == ClassName)
                        .ToList();
            }
            catch
            {
                throw new VarDump("选择" + NodeName + "_Class:" + ClassName + "出错");
            }
        }
        /// <summary>
        /// 选择节点
        /// </summary>
        /// <param name="NodeName">标签名</param>
        /// <param name="AttributesName">元素名字</param>
        /// <param name="Attributes">元素</param>
        /// <returns></returns>
        public List<HtmlNode> Select(string NodeName, string AttributesName, string Attributes)
        {
            try
            {
                return html.DocumentNode.Descendants(NodeName)
                        .Where(x => x.Attributes.Contains(AttributesName)
                        && x.Attributes[AttributesName].Value == Attributes)
                        .ToList();
            }
            catch (Exception e)
            {
                throw new VarDump("选择" + NodeName + "_" + NodeName + ":" + Attributes + "出错", e);
            }
        }
        /// <summary>
        /// 选择节点
        /// </summary>
        /// <param name="NodeName">标签名</param>
        /// <returns></returns>
        public List<HtmlNode> Select(string NodeName)
        {
            try
            {
                return html.DocumentNode.Descendants(NodeName).ToList();
            }
            catch (Exception e)
            {
                throw new VarDump("选择" + NodeName + "出错", e);
            }
        }
    }
    public class EnCode
    {
        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="data">源数据</param>
        /// <returns>编码后数据</returns>
        public static string BASE64(string data)
        {
            byte[] temp = Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(temp);
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5(string data)
        {
            return BitConverter.ToString(MD5_R(data)).ToLower().Replace("-", "");
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="data">加密</param>
        /// <returns>加密后的byte</returns>
        public static byte[] MD5_R(string data)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] buffer = Encoding.Default.GetBytes(data);
                return md5.ComputeHash(buffer);
            }
        }
        /// <summary>
        /// OPENSSL加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="data">数据</param>
        /// <param name="fOAEP"></param>
        /// <returns>加密后的数据</returns>
        public static string OpenSSL(string publicKey, string data, bool fOAEP = false)
        {
            RSACryptoServiceProvider _publicKeyRsaProvider;
            try
            {
                _publicKeyRsaProvider = Openssl.CreateRsaProviderFromPublicKey(publicKey);
                return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(data), fOAEP));
            }
            catch (Exception e)
            {
                throw new VarDump("密钥错误", e);
            }
        }
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="data">原数据</param>
        /// <returns>加密后的数据</returns>
        public static string SHA1(string data)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] str01 = Encoding.Default.GetBytes(data);
            byte[] str02 = sha1.ComputeHash(str01);
            return BitConverter.ToString(str02).Replace("-", "");
        }
        /// <summary>
        /// AES-128-CBC加密
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="key">密匙</param>
        /// <param name="iv">盐</param>
        /// <returns>加密后的数据</returns>
        public static byte[] AES128(string data, string key, byte[] iv)
        {
            try
            {
                byte[] keyArray = Encoding.Default.GetBytes(key);
                byte[] ivArray = iv;
                byte[] toEncryptArray = Encoding.Default.GetBytes(data);

                RijndaelManaged rDel = new RijndaelManaged
                {
                    Key = keyArray,
                    IV = ivArray,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return resultArray;
            }
            catch (Exception e)
            {
                throw new VarDump("加密失败", e);
            }
        }
    }
    public class DeCode
    {
        /// <summary>
        /// BASE64解码
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <returns>加密后的数据</returns>
        public static string BASE64(string data)
        {
            byte[] c = Convert.FromBase64String(data);
            return Encoding.UTF8.GetString(c);
        }
        /// <summary>
        /// AES128解码
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">盐</param>
        /// <returns>解密后的数据</returns>
        public static string AES128(byte[] data, string key, byte[] iv)
        {
            RijndaelManaged rijalg = new RijndaelManaged();
            rijalg.Padding = PaddingMode.None;
            rijalg.Mode = CipherMode.CBC;

            rijalg.Key = Encoding.Default.GetBytes(key);
            rijalg.IV = iv;

            ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

            try
            {
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new VarDump("解密失败", e);
            }
        }
        /// <summary>
        /// AES256解密
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">盐</param>
        /// <returns>解密后的数据</returns>
        public static string AES256(byte[] data, string key, string iv)
        {
            RijndaelManaged rijalg = new RijndaelManaged();
            rijalg.BlockSize = 128;
            rijalg.KeySize = 256;
            rijalg.FeedbackSize = 128;
            rijalg.Padding = PaddingMode.None;
            rijalg.Mode = CipherMode.CBC;

            rijalg.Key = Encoding.Default.GetBytes(key);
            rijalg.IV = Encoding.Default.GetBytes(iv);

            ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

            try
            {
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new VarDump("解密失败", e);
            }
        }
        /// <summary>
        /// OPENSSL解码
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="data">数据</param>
        /// <param name="fOAEP"></param>
        /// <returns>解密后的数据</returns>
        public static string OpenSSL(string privateKey, string data, bool fOAEP = false)
        {
            RSACryptoServiceProvider _privateKeyRsaProvider;
            try
            {
                _privateKeyRsaProvider = Openssl.CreateRsaProviderFromPrivateKey(privateKey);
                return Convert.ToBase64String(_privateKeyRsaProvider.Decrypt(Encoding.UTF8.GetBytes(data), fOAEP));
            }
            catch
            {
                throw new VarDump("密钥错误");
            }
        }
    }
    public class Tools
    {
        /// <summary>
        /// HEX字符串到HEX
        /// </summary>
        /// <param name="hexString">HEX字符串</param>
        /// <returns>HEX数组</returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="a">源字符串</param>
        /// <param name="b">从开始</param>
        /// <param name="c">到结束</param>
        /// <returns>分割后的</returns>
        public static string GetString(string a, string b, string c = null)
        {
            if (b == null)
            {
                return a;
            }
            int x = a.IndexOf(b) + b.Length;
            int y;
            if (c != null)
            {
                y = a.IndexOf(c, x);
                if (a[y - 1] == '"')
                {
                    y = a.IndexOf(c, y + 1);
                }
                if (y - x <= 0)
                    return a;
                else
                    return a.Substring(x, y - x);
            }
            else
                return a.Substring(x);
        }
        /// <summary>
        /// GBK到UTF-8
        /// </summary>
        /// <param name="msg">GBK字符串</param>
        /// <returns>UTF-8字符串</returns>
        public static string GBKtoUTF8(string msg)
        {
            try
            {
                byte[] srcBytes = Encoding.Default.GetBytes(msg);
                byte[] bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, srcBytes);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return msg;
            }
        }
        /// <summary>
        /// UTF-8到GBK
        /// </summary>
        /// <param name="msg">UTF-8字符串</param>
        /// <returns>GBK字符串</returns>
        public static string UTF8toGBK(string msg)
        {
            try
            {
                byte[] srcBytes = Encoding.UTF8.GetBytes(msg);
                byte[] bytes = Encoding.Convert(Encoding.UTF8, Encoding.Default, srcBytes);
                return Encoding.Default.GetString(bytes);
            }
            catch
            {
                return msg;
            }
        }
        /// <summary>
        /// 转成JSON字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>json字符串</returns>
        public static string ToJson(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception e)
            {
                throw new VarDump("转成Json失败", e);
            }
        }
        /// <summary>
        /// JSON转成对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">字符串</param>
        /// <returns>对象</returns>
        public static T ToObject<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                throw new VarDump("转成对象失败", e);
            }
        }
        /// <summary>
        /// 字符串转成JSON对象
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns>JSON对象</returns>
        public static JObject ToJObject(string json)
        {
            try
            {
                return JObject.Parse(json);
            }
            catch (Exception e)
            {
                throw new VarDump("转成JSON对象失败", e);
            }
        }
        /// <summary>
        /// 获取类
        /// </summary>
        /// <param name="classname">类名</param>
        /// <returns>类</returns>
        public static dynamic GetClass(string classname, params object[] obj)
        {
            var data = DllStonge.GetClass(classname);
            if (data == null)
                throw new VarDump("没有这个类:" + classname);
            try
            {
                return Activator.CreateInstance(data.Type, obj);
            }
            catch (Exception e)
            {
                throw new VarDump("创建类:" + classname + "失败", e);
            }
        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns>时间戳</returns>
        public static long GetTimeSpan()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        /// <summary>
        /// 转码
        /// </summary>
        /// <param name="InputType">输入格式</param>
        /// <param name="OutputType">输出格式</param>
        /// <param name="InputData">输入数据</param>
        /// <returns>转码后的数据</returns>
        public static byte[] Transcoding(TranscodeType InputType, TranscodeType OutputType, byte[] InputData)
        {
            try
            {
                return Transcode.Start(InputType, OutputType, InputData);
            }
            catch (Exception e)
            {
                throw new VarDump("转码失败", e);
            }
        }
        /// <summary>
        /// 判断WebSocket客户端是否在线
        /// </summary>
        /// <param name="id">客户端ID</param>
        /// <returns>是否在线</returns>
        public static bool WebSocketIsOnline(string id)
        {
            return ServerWebSocket.IsOnline(id);
        }
        /// <summary>
        /// 获取在线的机器人
        /// </summary>
        /// <returns>机器人QQ号列表</returns>
        public static List<long> GetBots()
        {
            return RobotSocket.QQs;
        }
        /// <summary>
        /// 压缩HTML
        /// </summary>
        /// <param name="html">原始数据</param>
        /// <returns>压缩后的数据</returns>
        public static string CompressHTML(string html)
        {
            return CodeCompress.HTML(html);
        }
        /// <summary>
        /// 压缩JS
        /// </summary>
        /// <param name="js">原始数据</param>
        /// <returns>压缩后的数据</returns>
        public static string CompressJS(string js)
        {
            return CodeCompress.JS(js);
        }
        /// <summary>
        /// 压缩CSS
        /// </summary>
        /// <param name="css">原始数据</param>
        /// <returns>压缩后的数据</returns>
        public static string CompressCSS(string css)
        {
            return CodeCompress.CSS(css);
        }
    }
    public class WebSocketMessage
    {
        public IWebSocketConnection Client { get; private set; }
        public string Data { get; private set; }
        /// <summary>
        /// WebSocket传来数据
        /// </summary>
        /// <param name="Client">WS客户端</param>
        /// <param name="Data">WS本次传来的数据</param>
        public WebSocketMessage(IWebSocketConnection Client, string Data)
        {
            this.Client = Client;
            this.Data = Data;
        }
    }
    public class WebSocketOpen
    {
        public IWebSocketConnection Client { get; private set; }
        /// <summary>
        /// WebSocket连接
        /// </summary>
        /// <param name="Client">WS客户端</param>
        public WebSocketOpen(IWebSocketConnection Client)
        {
            this.Client = Client;
        }
    }
    public class WebSocketClose
    {
        public IWebSocketConnection Client { get; private set; }
        /// <summary>
        /// WebSocket断开
        /// </summary>
        /// <param name="Client">WS客户端</param>
        public WebSocketClose(IWebSocketConnection Client)
        {
            this.Client = Client;
        }
    }
    public class RobotAfter
    {
        public enum MessageType
        {
            group, private_, friend
        }
        public long qq { get; private set; }
        public MessageType type { get; private set; }
        public long id { get; private set; }
        public long fid { get; private set; }
        public bool res { get; private set; }
        public string error { get; private set; }
        public string messageId { get; private set; }
        public List<string> message { get; private set; }
        /// <summary>
        /// 机器人发送消息后回调
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="qq">QQ机器人账户</param>
        /// <param name="id">群号</param>
        /// <param name="fid">QQ号</param>
        /// <param name="res">是否发送成功</param>
        /// <param name="message">消息</param>
        public RobotAfter(MessageType type, long qq, long id, long fid, bool res, string error, List<string> message)
        {
            this.qq = qq;
            this.type = type;
            this.id = id;
            this.fid = fid;
            this.res = res;
            this.error = error;
            this.message = message;
            messageId = Tools.GetString(message[0], "source:", ",");
        }
        /// <summary>
        /// 撤回消息
        /// </summary>
        public void ReCall()
        {
            RobotSocket.ReCall(messageId);
        }
    }
    public class RobotRequest
    {
        public enum MessageType
        {
            group, private_, friend
        }
        public long qq { get; private set; }
        public MessageType type { get; private set; }
        public long id { get; private set; }
        public long fid { get; private set; }
        public string name { get; private set; }
        public List<string> message { get; private set; }
        public string messageId { get; private set; }
        /// <summary>
        /// 机器人请求
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <param name="qq">QQ机器人账户</param>
        /// <param name="id">群号</param>
        /// <param name="fid">QQ号</param>
        /// <param name="name">名字</param>
        /// <param name="message">消息</param>
        public RobotRequest(MessageType type, long qq, long id, long fid, string name, List<string> message)
        {
            this.qq = qq;
            this.type = type;
            this.id = id;
            this.fid = fid;
            this.name = name;
            this.message = message;
            if (message != null && message.Count != 0)
                messageId = Tools.GetString(message[0], "source:", ",");
        }
        /// <summary>
        /// 撤回消息
        /// </summary>
        public void ReCall()
        {
            RobotSocket.ReCall(messageId);
        }
        /// <summary>
        /// 发送消息回应
        /// </summary>
        /// <param name="message">消息</param>
        public void SendMessage(List<string> message)
        {
            switch (type)
            {
                case MessageType.group:
                    RobotSocket.SendGroupMessage(qq, id, message);
                    break;
                case MessageType.private_:
                    RobotSocket.SendGroupPrivateMessage(qq, id, fid, message);
                    break;
                case MessageType.friend:
                    RobotSocket.SendFriendMessage(qq, fid, message);
                    break;
            }
        }
        /// <summary>
        /// 发送图片回应
        /// </summary>
        /// <param name="img">图片二进制</param>
        public void SendImage(byte[] img)
        {
            string data = Convert.ToBase64String(img);
            switch (type)
            {
                case MessageType.group:
                    RobotSocket.SendGroupImage(qq, id, data);
                    break;
                case MessageType.private_:
                    RobotSocket.SendGroupPrivateImage(qq, id, fid, data);
                    break;
                case MessageType.friend:
                    RobotSocket.SendFriendImage(qq, fid, data);
                    break;
            }
        }

        /// <summary>
        /// 发送本地文件回应
        /// </summary>
        /// <param name="file">文件名</param>
        public void SendImageFile(string file)
        {
            switch (type)
            {
                case MessageType.group:
                    RobotSocket.SendGroupImageFile(qq, id, file);
                    break;
                case MessageType.private_:
                    RobotSocket.SendGroupPrivateImageFile(qq, id, fid, file);
                    break;
                case MessageType.friend:
                    RobotSocket.SendFriendImageFile(qq, fid, file);
                    break;
            }
        }

        /// <summary>
        /// 发送声音回应
        /// </summary>
        /// <param name="sound">音频二进制</param>
        public void SendSound(byte[] sound)
        {
            string data = Convert.ToBase64String(sound);
            switch (type)
            {
                case MessageType.group:
                    RobotSocket.SendGroupSound(qq, id, data);
                    break;
            }
        }

        /// <summary>
        /// 发送本地声音文件回复
        /// </summary>
        /// <param name="sound">文件名</param>
        public void SendSoundFile(string file)
        {
            switch (type)
            {
                case MessageType.group:
                    RobotSocket.SendGroupSoundFile(qq, id, file);
                    break;
            }
        }
    }
    public class RobotEvent
    {
        public enum EventType
        {
            GroupMemberJoin, GroupMemberQuit, GroupMemberKick
        };
        public long qq { get; private set; }
        public long id { get; private set; }
        public long fid { get; private set; }
        public string name { get; private set; }
        public string oname { get; private set; }
        public long oid { get; private set; }
        public EventType type { get; private set; }
        private RobotRequest RobotRequest;
        /// <summary>
        /// 机器人事件
        /// </summary>
        /// <param name="qq">QQ机器人账户</param>
        /// <param name="id">群号</param>
        /// <param name="fid">用户QQ号</param>
        /// <param name="name">用户昵称</param>
        /// <param name="oname">管理者昵称</param>
        /// <param name="oid">管理者QQ号</param>
        /// <param name="type">事件类型</param>
        public RobotEvent(EventType type, long qq, long id, long fid, string name, string oname, long oid)
        {
            this.qq = qq;
            this.id = id;
            this.fid = fid;
            this.name = name;
            this.oname = oname;
            this.oid = oid;
            this.type = type;
            RobotRequest = new RobotRequest(RobotRequest.MessageType.group, qq, id, fid, null, null);
        }
        /// <summary>
        /// 发送消息回应
        /// </summary>
        /// <param name="message">消息</param>
        public void SendMessage(List<string> message)
        {
            RobotRequest.SendMessage(message);
        }
        /// <summary>
        /// 发送图片回应
        /// </summary>
        /// <param name="img">图片二进制</param>
        public void SendImage(byte[] img)
        {
            RobotRequest.SendImage(img);
        }

        /// <summary>
        /// 发送声音回应
        /// </summary>
        /// <param name="sound">音频二进制</param>
        public void SendSound(byte[] sound)
        {
            RobotRequest.SendSound(sound);
        }
    }
    public class FileLoad
    {
        /// <summary>
        /// 从文件加载字符串
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件里面的字符串</returns>
        public static string LoadString(string filename)
        {
            return FileTemp.LoadString(filename);
        }
        /// <summary>
        /// 读一个文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>文件二进制</returns>
        public static byte[] LoadBytes(string filename)
        {
            return FileTemp.LoadBytes(filename);
        }
    }
    public class VarDump : Exception
    {
        private object[] obj;
        /// <summary>
        /// 变量输出
        /// </summary>
        /// <param name="obj">变量</param>
        public VarDump(params object[] obj)
        {
            this.obj = obj;
        }

        public string Get()
        {
            try
            {
                if (obj.Length == 0)
                {
                    return "no obj";
                }
                var data = new Dictionary<string, object>(new RepeatDictionaryComparer());
                foreach (var item in obj)
                {
                    var name = item.GetType();
                    data.Add(name.Name, item);
                }
                return JsonConvert.SerializeObject(data, Formatting.Indented).Replace("\\\"", "\"");
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
                return "变量输出失败" + e.ToString();
            }
        }
    }
    internal class RepeatDictionaryComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x != y;
        }
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}