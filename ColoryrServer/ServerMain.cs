﻿using ColoryrServer.DataBase;
using ColoryrServer.DllManager;
using ColoryrServer.DllManager.StartGen.GenUtils;
using ColoryrServer.FileSystem;
using ColoryrServer.Html;
using ColoryrServer.Http;
using ColoryrServer.IoT;
using ColoryrServer.MQTT;
using ColoryrServer.Robot;
using ColoryrServer.TaskUtils;
using ColoryrServer.WebSocket;
using HtmlAgilityPack;
using ICSharpCode.SharpZipLib.Zip;
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

        public static bool isGo { get; private set; } = false;

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

        public static void Start()
        {
            try
            {
                isGo = true;
                //初始化运行路径
                RunLocal = AppDomain.CurrentDomain.BaseDirectory + "ColoryrServer/";
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
                dynamic test = new HttpResponseMessage();
                var test1 = test.IsSuccessStatusCode;
                Parallel.ForEach(new List<string>(), (i, b) => { });
                Bitmap bitmap = new Bitmap(1, 1);
                bitmap.Dispose();
                Stream zip = ZipOutputStream.Null;
                Stream zip1 = ZipInputStream.Null;


                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                MQTTServer.Start();
                RobotUtils.Start();
                MysqlCon.Start();
                MSCon.Start();
                RedisCon.Start();
                OracleCon.Start();
                RamDataBase.Start();
                GenCode.Start();
                DllStonge.Start();
                HtmlUtils.Start();
                FileHttpStream.Start();
                IoTSocketServer.Start();
                ServerWebSocket.Start();

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
            HtmlUtils.Stop();
            LogOut("已关闭");
        }

        static void Main()
        {
            Start();

            HttpServer.Start();
        }
    }
}
