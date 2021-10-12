namespace ColoryrServer.NoASP
{
    internal class HttpServer
    {
        internal static List<HttpWorker> Workers = new();
        internal static List<HttpListenerWorker> Listeners = new();
        public static bool IsActive { get; set; }//是否在运行

        public static void Start()
        {
            try
            {
                ServerMain.LogOut($"Http服务器正在启动");
                IsActive = false;

                // 启动工作线程
                for (int i = 0; i < NoASP.Config.HttpThreadNumber; i++)
                {
                    HttpWorker worker = new("HttpWork" + i);
                    Workers.Add(worker);
                    worker.Start();
                }

                foreach (var item in ServerMain.Config.Http)
                {
                    var listener = new HttpListenerWorker("http://" + item.IP + ":" +
                    item.Port + "/");
                    ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
                    Listeners.Add(listener);
                }

                IsActive = true;
                ServerMain.LogOut("Http服务器已启动");
            }
            catch (Exception e)
            {
                ServerMain.LogError(e);
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
