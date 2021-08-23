using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Pipe;
using ColoryrServer.SDK;
using ColoryrServer.Utils;
using HttpMultipartParser;
using Lib.App;
using Lib.Build;
using Lib.Build.Object;
using Lib.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ColoryrServer.Http
{
    class HttpServer
    {
        private static Thread[] Workers;                             // 工作线程组
        public static ConcurrentBag<HttpListenerContext> Queue;   // 请求队列
        public static List<HttpListener> Listeners;
        public static bool IsActive { get; set; }//是否在运行

        private static void Init()
        {
            ServerMain.LogOut($"Http服务器正在启动");

            Workers = new Thread[ServerMain.Config.HttpThreadNumber];
            Listeners = new();
            Queue = new();
            IsActive = false;

            foreach (var item in ServerMain.Config.Http)
            {
                var Listener = new HttpListener();
                Listener.Prefixes.Add("http://" + item.IP + ":" +
                item.Port + "/");
                ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Listener.TimeoutManager.EntityBody = TimeSpan.FromSeconds(30);
                    Listener.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(30);
                }
                Listeners.Add(Listener);
            }
        }

        public static void Start()
        {
            try
            {
                Init();
                // 启动工作线程
                for (int i = 0; i < Workers.Length; i++)
                {
                    Workers[i] = new Thread(HttpWorker.Worker);
                    Workers[i].Start();
                }
                IsActive = true;
                for (int a = 0; a < Listeners.Count; a++)
                {
                    var item = Listeners[a];
                    item.Start();
                    item.BeginGetContext(ContextReady, a);
                }
                ServerMain.LogOut("Http服务器已启动");
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        public static void StartPipe()
        {
            try
            {
                Init();
                // 启动工作线程
                for (int i = 0; i < Workers.Length; i++)
                {
                    Workers[i] = new Thread(HttpWorker.PipeWorker);
                    Workers[i].Start();
                }
                IsActive = true;
                for (int a = 0; a < Listeners.Count; a++)
                {
                    var item = Listeners[a];
                    item.Start();
                    item.BeginGetContext(ContextReady, a);
                }
                ServerMain.LogOut("Http服务器已启动");
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
            }
        }

        private static void ContextReady(IAsyncResult ar)
        {
            if (IsActive)
            {
                var http = Listeners[(int)ar.AsyncState];
                http.BeginGetContext(ContextReady, ar.AsyncState);
                Queue.Add(http.EndGetContext(ar));
            }
        }

        public static void Stop()
        {
            if (IsActive)
            {
                ServerMain.LogOut("Http服务器正在关闭");
                IsActive = false;
                foreach (var item in Listeners)
                {
                    item.Stop();
                }
                ServerMain.LogOut("Http服务器已关闭");
            }
        }
    }
}
