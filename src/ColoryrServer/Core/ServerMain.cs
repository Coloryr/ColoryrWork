using ColoryrServer.Core.BuilderPost;
using ColoryrServer.Core.DataBase;
using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.DllManager.Service;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.Core.FileSystem.Web;
using ColoryrServer.Core.Html;
using ColoryrServer.Core.PortServer;
using ColoryrServer.Core.Robot;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using HtmlAgilityPack;
using ICSharpCode.SharpZipLib.Zip;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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

public class ServerMain
{
    public const string Version = "2.4.0";
    /// <summary>
    /// 配置文件
    /// </summary>
    public static MainConfig Config { get; set; }
    /// <summary>
    /// 运行路径
    /// </summary>
    public static string RunLocal { get; private set; }
    /// <summary>
    /// 配置文件操作
    /// </summary>
    public static ConfigUtils ConfigUtils { get; set; }

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

    public static void Start()
    {
        try
        {
            Console.WriteLine($"ColoryrServer版本:{Version}");
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
            //配置文件
            ConfigUtils.Start();

            PostDo.Start();
            PortNettyManager.Start();
            CodeFileManager.Start();
            NoteFile.Start();
            APIFile.Start();
            FileRam.Start();
            HttpClientUtils.Start();
            PortMqttServer.Start();
            RobotUtils.Start();
            LoginSave.Start();
            WebBinManager.Start();
            DllRunLog.Start();
            MSCon.Start();
            RedisCon.Start();
            OracleCon.Start();
            MysqlCon.Start();
            SqliteCon.Start();
            RamDataBase.Start();

            DllLoad();

            GenCode.Start();

            WebFileManager.Start();
            FileHttpStream.Start();

            DllStongeManager.Start();
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
        var test2 = new HtmlDocument();
        using HttpResponseMessage test = new();
        var test1 = test.IsSuccessStatusCode;
        Parallel.ForEach(new List<string>(), (i, b) => { });
        using Image<Rgba32> bitmap = new(1, 1);
        SystemFonts.Families.GetEnumerator();
        bitmap.Mutate(a => { a.BackgroundColor(Color.AliceBlue); });
        ZipEntry entry = new("1");
        NameValueCollection nameValue = new();
        Regex re = new("");
        using Aes aes = Aes.Create();
        WebProxy proxy = new();
    }

    public static void Stop()
    {
        OnStop.Invoke();
    }
}
