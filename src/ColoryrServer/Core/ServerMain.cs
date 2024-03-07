using ColoryrServer.Core.BuilderPost;
using ColoryrServer.Core.Database;
using ColoryrServer.Core.DBConnection;
using ColoryrServer.Core.Dll;
using ColoryrServer.Core.Dll.Gen;
using ColoryrServer.Core.Dll.Service;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.Core.Html;
using ColoryrServer.Core.Managers;
using ColoryrServer.Core.PortServer;
using ColoryrServer.Core.Robot;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using HtmlAgilityPack;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Core;

public static class ServerMain
{
    public const string Version = "3.0.0";
    /// <summary>
    /// 配置文件
    /// </summary>
    public static MainConfig Config { get; set; }
    /// <summary>
    /// 运行路径
    /// </summary>
    public static string RunLocal { get; private set; }

    /// <summary>
    /// 服务器停止时的回调
    /// </summary>
    public delegate void StopCall();
    /// <summary>
    /// 服务器停止时触发的事件
    /// </summary>
    public static event StopCall OnStop;

    /// <summary>
    /// 日志输出
    /// </summary>
    private static Logs Logs;
    /// <summary>
    /// 写错误到日志中
    /// </summary>
    /// <param name="e">Exception</param>
    public static void LogError(Exception e)
    {
        var time = DateTime.Now;
        string a = $"[{time}][Error]{e}";
        Task.Run(() =>
        {
            PostDo.AddLog(a);
            Logs.LogWrite(a);
            Console.WriteLine(a);
        });
    }
    public static void LogError(string item, Exception e)
    {
        var time = DateTime.Now;
        string a = $"[{time}][Error]{item}{Environment.NewLine}[{time}][Error]{e}";
        Task.Run(() =>
        {
            PostDo.AddLog(a);
            Logs.LogWrite(a);
            Console.WriteLine(a);
        });
    }
    /// <summary>
    /// 写错误到日志中
    /// </summary>
    /// <param name="a">信息</param>
    public static void LogError(string a)
    {
        var time = DateTime.Now;
        a = $"[{time}][Error]{a}";
        Task.Run(() =>
        {
            PostDo.AddLog(a);
            Logs.LogWrite(a);
            Console.WriteLine(a);
        });
    }
    /// <summary>
    /// 写信息到日志中
    /// </summary>
    /// <param name="a">信息</param>
    public static void LogOut(string a)
    {
        var time = DateTime.Now;
        a = $"[{time}][Info]{a}";
        Task.Run(() =>
        {
            PostDo.AddLog(a);
            Logs.LogWrite(a);
            Console.WriteLine(a);
        });
    }
    /// <summary>
    /// 写警告到日志中
    /// </summary>
    /// <param name="a">信息</param>
    public static void LogWarn(string a)
    {
        var time = DateTime.Now;
        a = $"[{time}][Warn]{a}";
        Task.Run(() =>
        {
            PostDo.AddLog(a);
            Logs.LogWrite(a);
            Console.WriteLine(a);
        });
    }

    public static void Init()
    {
        OnStop = null;
        //初始化运行路径
        RunLocal = AppDomain.CurrentDomain.BaseDirectory + "ColoryrServer/";
        if (!Directory.Exists(RunLocal))
        {
            Directory.CreateDirectory(RunLocal);
        }
        //设置编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //创建日志文件
        Logs = new Logs(RunLocal);
    }

    public static void Start()
    {
        try
        {
            Console.WriteLine($"ColoryrServer版本:{Version}");

            //配置文件
            ConfigUtils.Start();

            AssemblyList.Start();
            PostDo.Start();
            PortNettyManager.Start();
            CodeManager.Start();
            NoteFile.Start();
            APIFile.Start();
            FileRam.Start();
            HttpClientUtils.Start();
            PortMqttServer.Start();
            //RobotUtils.Start();
            LoginDatabase.Start();
            WebBinManager.Start();
            LogDatabsae.Start();
            MSCon.Start();
            RedisCon.Start();
            MysqlCon.Start();
            SqliteCon.Start();
            RamDatabase.Start();

            DllLoad();

            GenCode.Start();
            ServerPackage.Start();

            WebFileManager.Start();
            FileStreamManager.Start();
            DllFileManager.Start();
            ServiceManager.Start();
            PortSocketServer.Start();
            PortWebSocket.Start();

            //等待初始化完成
            Thread.Sleep(2000);
        }
        catch (Exception e)
        {
            LogError(e);
            Console.Read();
        }
    }

    public static void DllLoad()
    {
        //给编译用的，防DLL找不到
        var save = new RamData("ColoryrServer_Init");
        save.Set("123", 1234);
        save.Get("123");
        save.Close();
        dynamic test3 = 1234;
        test3 = "1234";
        var obj = new JObject();
        var arr = new JArray();
        var test2 = new HtmlDocument();
        using var test = new HttpResponseMessage();
        var test1 = test.IsSuccessStatusCode;
        Parallel.ForEach(new List<string>(), (i, b) => { });
        var entry = new ZipEntry("1");
        using var image = new SKBitmap();
        var nameValue = new NameValueCollection();
        var re = new Regex("");
        using Aes aes = Aes.Create();
        var proxy = new WebProxy();
    }

    public static void Stop()
    {
        OnStop.Invoke();
    }
}
