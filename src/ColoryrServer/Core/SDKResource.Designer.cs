﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ColoryrServer.Core {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class SDKResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SDKResource() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ColoryrServer.Core.SDKResource", typeof(SDKResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.DllManager;
        ///using ColoryrServer.FileSystem;
        ///using ColoryrServer.Robot;
        ///using ColoryrServer.Utils;
        ///using ColoryrServer.WebSocket;
        ///using Fleck;
        ///using Newtonsoft.Json;
        ///using Newtonsoft.Json.Linq;
        ///using System;
        ///using System.Collections.Generic;
        ///using System.IO;
        ///using System.Security.Cryptography;
        ///using System.Text;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class HttpMultipartFile
        ///{
        ///    public Stream Data { get; set; }
        ///    public string FileName { get; set; }
        ///    public string [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string ColoryrSDK {
            get {
                return ResourceManager.GetString("ColoryrSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.DataBase;
        ///using Dapper;
        ///using MySql.Data.MySqlClient;
        ///using Oracle.ManagedDataAccess.Client;
        ///using StackExchange.Redis;
        ///using System.Collections.Generic;
        ///using System.Data.SqlClient;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class Mysql
        ///{
        ///    private string Database;
        ///    private int ID;
        ///    /// &lt;summary&gt;
        ///    /// Mysql数据库
        ///    /// &lt;/summary&gt;
        ///    /// &lt;param name=&quot;Database&quot;&gt;数据库名&lt;/param&gt;
        ///    /// &lt;param name=&quot;ID&quot;&gt;数据库ID&lt;/param&gt;
        ///    public Mysql(string Database, int ID = 0)
        ///    { [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string DatabaseSDK {
            get {
                return ResourceManager.GetString("DatabaseSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.Html;
        ///using HtmlAgilityPack;
        ///using System;
        ///using System.Collections.Generic;
        ///using System.Linq;
        ///using System.Net;
        ///using System.Net.Http;
        ///using System.Threading;
        ///using System.Threading.Tasks;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class NewHttpHtml
        ///{
        ///    public CancellationTokenSource Cancel;
        ///    public CookieContainer Cookies { get; private set; }
        ///
        ///    private ExHttpClient Client;
        ///    private Dictionary&lt;string, string&gt; Head;
        ///
        ///    public NewHttpHtml(CookieContainer Cook [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string HtmlSDK {
            get {
                return ResourceManager.GetString("HtmlSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using System.Collections.Generic;
        ///using System.Collections.Specialized;
        ///using System.IO;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class HttpRequest
        ///{
        ///    public Dictionary&lt;string, dynamic&gt; Parameter { get; init; }
        ///    public NameValueCollection RowRequest { get; init; }//原始请求的字符串
        ///    public Dictionary&lt;string, List&lt;string&gt;&gt; Cookie { get; init; }
        ///    public MyContentType ContentType { get; init; }
        ///    public Stream Stream { get; init; }
        ///    public string Method { get; init; }
        ///
        ///    /// 获取参数
        ///    // [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string HttpSDK {
            get {
                return ResourceManager.GetString("HttpSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.MQTT;
        ///using MQTTnet.Protocol;
        ///using MQTTnet.Server;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class MqttConnectionValidator
        ///{
        ///    public MqttConnectionValidatorContext Context { get; init; }
        ///    /// &lt;summary&gt;
        ///    /// MQTT服务器验证
        ///    /// &lt;/summary&gt;
        ///    /// &lt;param name=&quot;Context&quot;&gt;数据&lt;/param&gt;
        ///    public MqttConnectionValidator(MqttConnectionValidatorContext Context)
        ///        =&gt; this.Context = Context;
        ///    /// &lt;summary&gt;
        ///    /// 发送消息
        ///    /// &lt;/summary&gt;
        ///    /// &lt;param name=&quot;Topic&quot;&gt;标题&lt; [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string MqttSDK {
            get {
                return ResourceManager.GetString("MqttSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using System;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        ///public class NotesSDK : Attribute
        ///{
        ///    public string Text;
        ///    public string[] Input;
        ///    public string[] Output;
        ///
        ///    public NotesSDK(string Text, string[] Input = null, string[] Output = null)
        ///    {
        ///        this.Text = Text;
        ///        this.Input = Input ?? new string[1];
        ///        this.Output = Output ?? new string[1];
        ///    }
        ///}
        /// 的本地化字符串。
        /// </summary>
        public static string NotesSDK {
            get {
                return ResourceManager.GetString("NotesSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using Newtonsoft.Json;
        ///using System;
        ///using System.Collections.Concurrent;
        ///using System.Collections.Generic;
        ///using System.Net.Sockets;
        ///using System.Text;
        ///using System.Threading;
        ///using ASocket = System.Net.Sockets.Socket;
        ///
        ///namespace ColoryrServer.Robot;
        ///
        /////机器人返回数据包
        ////// &lt;summary&gt;
        ////// 55 [插件]获取群列表
        ////// &lt;/summary&gt;
        ///public record ListGroupPack : PackBase
        ///{
        ///    /// &lt;summary&gt;
        ///    /// 群列表
        ///    /// &lt;/summary&gt;
        ///    public List&lt;GroupInfo&gt; groups { get; set; }
        ///}
        ////// &lt;summary&gt;
        ////// 56 [插件]获取好友列表
        ////// &lt;/ [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string RobotPacks {
            get {
                return ResourceManager.GetString("RobotPacks", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.Robot;
        ///using System.Collections.Generic;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class RobotAfter
        ///{
        ///    public enum MessageType
        ///    {
        ///        group, private_, friend, stranger
        ///    }
        ///    public long qq { get; private set; }
        ///    public MessageType type { get; private set; }
        ///    public long id { get; private set; }
        ///    public long fid { get; private set; }
        ///    public bool res { get; private set; }
        ///    public string error { get; private set; }
        ///    public string messageId { get [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string RobotSDK {
            get {
                return ResourceManager.GetString("RobotSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.IoTSocket;
        ///using System.Collections.Generic;
        ///using System.Text;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class TcpSocketRequest
        ///{
        ///    private int Server { get; init; }
        ///    public int Port { get; init; }
        ///    public byte[] Data { get; init; }
        ///    /// &lt;summary&gt;
        ///    /// 构造方法
        ///    /// &lt;/summary&gt;
        ///    /// &lt;param name=&quot;Port&quot;&gt;Socket端口&lt;/param&gt;
        ///    /// &lt;param name=&quot;Data&quot;&gt;Socket发送的数据&lt;/param&gt;
        ///    public TcpSocketRequest(int Port, byte[] Data, int Server)
        ///    {
        ///        this.Port = Port;        /// [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string SocketSDK {
            get {
                return ResourceManager.GetString("SocketSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.TaskUtils;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class TaskSDK
        ///{
        ///    /// &lt;summary&gt;
        ///    /// 是否存在任务
        ///    /// &lt;/summary&gt;
        ///    /// &lt;param name=&quot;name&quot;&gt;任务名字&lt;/param&gt;
        ///    /// &lt;returns&gt;是否存在&lt;/returns&gt;
        ///    public static bool HaveTask(string name)
        ///        =&gt; TaskThread.HaveTask(name);
        ///    /// &lt;summary&gt;
        ///    /// 添加一个任务
        ///    /// 如果存在则会覆盖
        ///    /// &lt;/summary&gt;
        ///    /// &lt;param name=&quot;arg&quot;&gt;任务参数&lt;/param&gt;
        ///    public static bool StartTask(TaskUserArg arg)
        ///        =&gt; TaskThread.StartTask(arg);
        ///     [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string TaskSDK {
            get {
                return ResourceManager.GetString("TaskSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.FileSystem;
        ///using System;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class WebHtml
        ///{
        ///    /// &lt;summary&gt;
        ///    /// 向前端添加Web资源
        ///    /// &lt;/summary&gt;
        ///    /// &lt;param name=&quot;uuid&quot;&gt;资源UUID&lt;/param&gt;
        ///    /// &lt;param name=&quot;name&quot;&gt;名字&lt;/param&gt;
        ///    /// &lt;param name=&quot;code&quot;&gt;内容&lt;/param&gt;
        ///    public static void AddHtml(string uuid, string name, string code)
        ///    {
        ///        var web = HtmlUtils.GetHtml(uuid);
        ///        if (web == null)
        ///        {
        ///            var time = string.Format(&quot;{0:s}&quot;, DateTime.Now);
        ///     [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string WebHtmlSDK {
            get {
                return ResourceManager.GetString("WebHtmlSDK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 using ColoryrServer.WebSocket;
        ///using Fleck;
        ///
        ///namespace ColoryrServer.SDK;
        ///
        ///public class WebSocketMessage
        ///{
        ///    public bool IsAvailable { get; init; }
        ///    public IWebSocketConnectionInfo Info { get; init; }
        ///    public string Data { get; init; }
        ///    /// &lt;summary&gt;
        ///    /// WebSocket传来数据
        ///    /// &lt;/summary&gt;
        ///    /// &lt;param name=&quot;Client&quot;&gt;WS客户端&lt;/param&gt;
        ///    /// &lt;param name=&quot;Data&quot;&gt;WS本次传来的数据&lt;/param&gt;
        ///    public WebSocketMessage(IWebSocketConnection Client, string Data)
        ///    {
        ///        IsAvailable = Client [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string WebSocketSDK {
            get {
                return ResourceManager.GetString("WebSocketSDK", resourceCulture);
            }
        }
    }
}
