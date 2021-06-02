using ColoryrServer.DataBase;
using ColoryrServer.DllManager;
using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrServer.Html;
using ColoryrServer.Http;
using ColoryrServer.IoT;
using ColoryrServer.MQTT;
using ColoryrServer.Pipe;
using ColoryrServer.Robot;
using ColoryrServer.TaskUtils;
using ColoryrServer.WebSocket;
using HtmlAgilityPack;
using Lib.Build;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer
{
    public class TestRun
    {
        public static void Start()
        {
            StackTrace st = new(true);
            StackFrame[] sfs = st.GetFrames();
            for (int i = 1; i < sfs.Length; ++i)
            {
                StackFrame item = sfs[i];
                var item1 = item.GetMethod();
                if (item1.DeclaringType.FullName == "ColoryrServer.RunTest.Program" && item1.Module.Name == "ColoryrServer.RunTest.dll" && item1.Name == "Main")
                {
                    ServerMain.Start();
                }
            }
        }
        public static void Stop()
        {
            StackTrace st = new(true);
            StackFrame[] sfs = st.GetFrames();
            for (int i = 1; i < sfs.Length; ++i)
            {
                StackFrame item = sfs[i];
                var item1 = item.GetMethod();
                if (item1.DeclaringType.FullName == "ColoryrServer.RunTest.Program" && item1.Module.Name == "ColoryrServer.RunTest.dll" && item1.Name == "Main")
                {
                    ServerMain.Stop();
                }
            }
        }
        public static void Robot()
        {
            StackTrace st = new(true);
            StackFrame[] sfs = st.GetFrames();
            for (int i = 1; i < sfs.Length; ++i)
            {
                StackFrame item = sfs[i];
                var item1 = item.GetMethod();
                if (item1.DeclaringType.FullName == "ColoryrServer.RunTest.Program" && item1.Module.Name == "ColoryrServer.RunTest.dll" && item1.Name == "Main")
                {
                    RobotUtils.Start();
                }
            }
        }
    }
    internal class ServerMain
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        public static MainConfig Config { get; set; }
        /// <summary>
        /// 运行路径
        /// </summary>
        public static string RunLocal { get; set; }
        /// <summary>
        /// 日志输出
        /// </summary>
        public static Logs Logs;

        public static bool isGo = false;

        /// <summary>
        /// 写错误到日志中
        /// </summary>
        /// <param name="e">Exception</param>
        public static void LogError(Exception e)
        {
            string a = "[错误]" + e.ToString();
            Task.Run(() => Logs.LogWrite(a));
            Console.WriteLine(a);
        }
        /// <summary>
        /// 写错误到日志中
        /// </summary>
        /// <param name="a">信息</param>
        public static void LogError(string a)
        {
            a = "[错误]" + a;
            Task.Run(() => Logs.LogWrite(a));
            Console.WriteLine(a);
        }
        /// <summary>
        /// 写信息到日志中
        /// </summary>
        /// <param name="a">信息</param>
        public static void LogOut(string a)
        {
            a = "[信息]" + a;
            Task.Run(() => Logs.LogWrite(a));
            Console.WriteLine(a);
        }

        private static void DatabaseRun()
        {
            //Mysql链接
            MysqlCon.Start();
            //MS链接
            MSCon.Start();
            //Redis链接
            RedisCon.Start();
            //Oracle链接
            OracleCon.Start();
            //内存数据库
            RamDataBase.Start();
        }

        public static void Start()
        {
            try
            {
                isGo = true;
                //初始化运行路径
                RunLocal = AppDomain.CurrentDomain.BaseDirectory + "ColoryrServer\\";
                if (!Directory.Exists(RunLocal))
                {
                    Directory.CreateDirectory(RunLocal);
                }
                //设置编码
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //创建日志文件
                Logs = new Logs(RunLocal);
                //配置文件
                ConfigUtil.Start();
                CSFile.Start();
                NoteFile.Start();
                APIFile.Start();
                FileTemp.Start();
                FileRam.Start();
                TaskThread.Start();
                HttpClientUtils.Start();

                //给编译用的，防DLL找不到
                new HtmlDocument();
                new HttpResponseMessage();
                Parallel.ForEach(new List<string>(), (i, b) => { });
                Bitmap bitmap = new Bitmap(1, 1);
                bitmap.Dispose();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                if (Config.Pipe.Enable)
                {
                    if (!Config.Pipe.ServerCore)
                    {
                        if (Config.Pipe.HttpServer)
                        {
                            HttpServer.StartPipe();
                        }
                        if (Config.Pipe.WebSocketServer)
                        {
                            ServerWebSocket.StartPipe();
                        }
                        if (Config.Pipe.IotServer)
                        {
                            IoTSocketServer.StartPipe();
                        }
                        if (Config.Pipe.MqttServer)
                        {
                            MQTTServer.StartPipe();
                        }
                    }
                    else
                    {
                        RobotUtils.Start();
                        DatabaseRun();
                        //初始化动态编译
                        GenCode.Start();
                        DllStonge.Start();
                        HtmlUtils.Start();
                    }
                }
                else
                {
                    MQTTServer.Start();
                    RobotUtils.Start();
                    DatabaseRun();
                    //服务器启动
                    HttpServer.Start();
                    IoTSocketServer.Start();
                    ServerWebSocket.Start();
                    //初始化动态编译
                    GenCode.Start();
                    DllStonge.Start();
                }

                //等待初始化完成
                Thread.Sleep(2000);
            }
            catch (Exception e)
            {
                LogError(e);
                Console.Read();
            }
        }

        public static void Stop()
        {
            LogOut("正在关闭");
            if (Config.Pipe.Enable)
            {
                PipeServer.Stop();
            }
            HttpServer.Stop();
            MysqlCon.Stop();
            MSCon.Stop();
            IoTSocketServer.Stop();
            ServerWebSocket.Stop();
            RobotUtils.Stop();
            RedisCon.Stop();
            OracleCon.Stop();
            RamDataBase.Stop();
            MQTTServer.Stop();
            TaskThread.Stop();
            HttpClientUtils.Stop();
            LogOut("已关闭");
        }

        static void Main()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    ServiceBase.Run(new ColoryrServer());
                }
                else
                {
                    Start();
                }
                Thread.Sleep(2000);
            }
            catch
            {

            }
            if (isGo == false)
            {
                using Process myPro = new();
                myPro.StartInfo.FileName = "cmd.exe";
                myPro.StartInfo.UseShellExecute = false;
                myPro.StartInfo.RedirectStandardInput = true;
                myPro.StartInfo.RedirectStandardOutput = true;
                myPro.StartInfo.RedirectStandardError = true;
                myPro.StartInfo.CreateNoWindow = true;
                myPro.Start();
                string str = "sc create BuildServer binPath= \"" + AppDomain.CurrentDomain.BaseDirectory + "BuildServer.exe\"";

                myPro.StandardInput.WriteLine(str);
                myPro.StandardInput.WriteLine("exit");
                myPro.StandardInput.AutoFlush = true;
                string output = myPro.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
                myPro.WaitForExit();
                if (output.Contains("失败"))
                {
                    if (output.Contains("拒绝访问"))
                    {
                        Console.WriteLine("请用管理员启动");
                        Console.Read();
                        return;
                    }
                    else
                    {
                        if (output.Contains("指定的服务已存在"))
                        {
                            myPro.Start();
                            myPro.StandardInput.WriteLine("sc stop BuildServer");
                            myPro.StandardInput.WriteLine("sc delete BuildServer");
                            myPro.StandardInput.WriteLine(str);
                            myPro.StandardInput.WriteLine("exit");
                            output = myPro.StandardOutput.ReadToEnd();
                            Console.WriteLine(output);
                            if (output.Contains("失败") && !output.Contains("成功"))
                            {
                                Console.WriteLine("服务安装失败");
                                return;
                            }
                        }
                    }
                }
                Console.WriteLine("服务已安装");
                myPro.Start();
                myPro.StandardInput.WriteLine("sc start BuildServer");
                myPro.StandardInput.WriteLine("exit");
                myPro.WaitForExit();
                Console.WriteLine(myPro.StandardOutput.ReadToEnd());
                Console.WriteLine("服务已启动");
                Console.WriteLine("按任意键退出");
                Console.Read();
            }
        }
    }
}
