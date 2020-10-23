using ColoryrServer.DataBase;
using ColoryrServer.DllManager;
using ColoryrServer.Http;
using ColoryrServer.IoT;
using ColoryrServer.Robot;
using ColoryrServer.WebSocket;
using HtmlAgilityPack;
using Lib.Build;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer
{
    public class ServerMain
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
            Task.Factory.StartNew(() =>
            {
                string a = "[错误]" + e.ToString();
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
            Task.Factory.StartNew(() => Logs.LogWrite(a));
            Console.WriteLine(a);
        }
        /// <summary>
        /// 写信息到日志中
        /// </summary>
        /// <param name="a">信息</param>
        public static void LogOut(string a)
        {
            a = "[信息]" + a;
            Task.Factory.StartNew(() => Logs.LogWrite(a));
            Console.WriteLine(a);
        }

        public static void Start()
        {
            try
            {
                isGo = true;
                //初始化运行路径
                RunLocal = AppDomain.CurrentDomain.BaseDirectory;
                //设置编码
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //创建日志文件
                Logs = new Logs(RunLocal);
                //配置文件
                new Config();
                new CSFile();

                new APIFile();
                new FileTemp();

                //给编译用的，防DLL找不到
                new HtmlDocument();
                new HttpResponseMessage();
                Parallel.ForEach(new List<string>(), (i, b) => { });
                Bitmap bitmap = new Bitmap(1, 1);
                bitmap.Dispose();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                //Mysql链接
                var state = MysqlCon.Start();
                if (state)
                {
                    LogOut("Mysql服务器已连接");
                }
                else
                {
                    LogError("Mysql连接失败");
                }
                //MS链接
                state = MSCon.Start();
                if (state)
                {
                    LogOut("MSsql服务器已连接");
                }
                else
                {
                    LogError("MSsql连接失败");
                }
                //Redis链接
                state = RedisCon.Start();
                if (state)
                {
                    LogOut("Redis服务器已连接");
                }
                else
                {
                    LogError("Redis连接失败");
                }

                //服务器启动
                HttpControl.Start();
                IoTSocket.Start();
                ServerWebSocket.Start();
                RobotSocket.Start();

                //初始化动态编译
                new GenTask();
                new DllStonge().DynamicInit();

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
            HttpControl.Stop();
            MysqlCon.Stop();
            MSCon.Stop();
            IoTSocket.Stop();
            ServerWebSocket.Stop();
            RobotSocket.Stop();
            RedisCon.Stop();
            LogOut("已关闭");
        }

        static void Main()
        {
            try
            {
                ServiceBase.Run(new ColorServer());
                Thread.Sleep(2000);
            }
            catch
            {

            }
            if (isGo == false)
            {
                using (Process myPro = new Process())
                {
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
}
