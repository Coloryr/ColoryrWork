using ColoryrServer.DataBase;
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
    internal class ASP 
    {
        /// <summary>
        /// �����ļ�
        /// </summary>
        public static MainConfig Config { get; set; }
        /// <summary>
        /// ����·��
        /// </summary>
        public static string RunLocal { get; set; }
        /// <summary>
        /// ��־���
        /// </summary>
        private static Logs Logs;

        public static bool isGo { get; private set; } = false;

        private static WebApplication Web;

        /// <summary>
        /// д������־��
        /// </summary>
        /// <param name="e">Exception</param>
        public static void LogError(Exception e)
        {
            string a = "[����]" + e.ToString();
            Task.Run(() => Logs.LogWrite(a));
            Console.WriteLine(a);
        }
        /// <summary>
        /// д������־��
        /// </summary>
        /// <param name="a">��Ϣ</param>
        public static void LogError(string a)
        {
            a = "[����]" + a;
            Task.Run(() => Logs.LogWrite(a));
            Console.WriteLine(a);
        }
        /// <summary>
        /// д��Ϣ����־��
        /// </summary>
        /// <param name="a">��Ϣ</param>
        public static void LogOut(string a)
        {
            a = "[��Ϣ]" + a;
            Task.Run(() => Logs.LogWrite(a));
            Console.WriteLine(a);
        }

        private static void DatabaseRun()
        {
            //Mysql����
            MysqlCon.Start();
            //MS����
            MSCon.Start();
            //Redis����
            RedisCon.Start();
            //Oracle����
            OracleCon.Start();
            //�ڴ����ݿ�
            RamDataBase.Start();
        }

        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();
            Web = builder.Build();

            try
            {
                isGo = true;
                //��ʼ������·��
                RunLocal = AppDomain.CurrentDomain.BaseDirectory + "ColoryrServer/";
                if (!Directory.Exists(RunLocal))
                {
                    Directory.CreateDirectory(RunLocal);
                }
                //���ñ���
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //������־�ļ�
                Logs = new Logs(RunLocal);
                //�����ļ�
                ConfigUtil.Start();
                CSFile.Start();
                NoteFile.Start();
                APIFile.Start();
                FileTemp.Start();
                FileRam.Start();
                TaskThread.Start();
                HttpClientUtils.Start();

                //�������õģ���DLL�Ҳ���
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
                DatabaseRun();
                //��ʼ����̬����
                GenCode.Start();
                DllStonge.Start();
                HtmlUtils.Start();
                FileHttpStream.Start();
                //����������
                //HttpServer.Start();
                IoTSocketServer.Start();
                ServerWebSocket.Start();

                //�ȴ���ʼ�����
                Thread.Sleep(2000);
            }
            catch (Exception e)
            {
                LogError(e);
                Console.Read();
            }

            Web.UseHttpsRedirection();
            Web.UseRouting();

            Web.UseAuthorization();
            Web.MapGet("/", Get);

            Web.Run();

        }

        private static void Get(HttpContext context) { 

        }


        public static async void Stop()
        {
            LogOut("���ڹر�");
            await Web.StopAsync();
            //HttpServer.Stop();
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
            LogOut("�ѹر�");
        }
    }
}

    