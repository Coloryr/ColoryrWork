using ColoryrServer.Core.DataBase;
using ColoryrServer.Core.DllManager.Gen;
using ColoryrServer.Core.DllManager.PostBuild;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Code;
using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.Core.Html;
using ColoryrServer.Core.IoT;
using ColoryrServer.Core.MQTT;
using ColoryrServer.Core.Netty;
using ColoryrServer.Core.Robot;
using ColoryrServer.Core.TaskUtils;
using ColoryrServer.Core.WebSocket;
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
    public const string Version = "2.0.0";
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
    public static ConfigUtil ConfigUtil { get; set; }

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
        string a = "[错误]" + e.ToString();
        Task.Run(() =>
        {
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
        a = "[错误]" + a;
        Task.Run(() =>
        {
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
        a = "[信息]" + a;
        Task.Run(() =>
        {
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
            ConfigUtil.Start();

            DllLoad();

            CodeFileManager.Start();
            NoteFile.Start();
            APIFile.Start();
            FileTemp.Start();
            FileRam.Start();
            HttpClientUtils.Start();
            MQTTServer.Start();
            RobotUtils.Start();
            PostBuild.Start();
            WebBinManager.Start();
            DllRunError.Start();
            MSCon.Start();
            RedisCon.Start();
            OracleCon.Start();
            MysqlCon.Start();
            SqliteCon.Start();
            RamDataBase.Start();
            GenCode.Start();
            TaskManager.Start();
            DllStongeManager.Start();
            WebFileManager.Start();
            FileHttpStream.Start();
            SocketServer.Start();
            ServerWebSocket.Start();
            NettyManager.Start();

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
        var test2 = new HtmlDocument();
        dynamic test = new HttpResponseMessage();
        var test1 = test.IsSuccessStatusCode;
        test.Dispose();
        Parallel.ForEach(new List<string>(), (i, b) => { });
        Image<Rgba32> bitmap = new(1, 1);
        SystemFonts.Families.GetEnumerator();
        bitmap.Mutate(a => { a.BackgroundColor(Color.AliceBlue); });
        bitmap.Dispose();
        Stream zip = Stream.Null;
        Stream zip1 = Stream.Null;
        ZipEntry entry = new("1");
        NameValueCollection nameValue = new();
        Regex re = new("");
        Aes aes = Aes.Create();
        aes.Dispose();
    }

    public static void Stop()
    {
        OnStop.Invoke();
    }
}
