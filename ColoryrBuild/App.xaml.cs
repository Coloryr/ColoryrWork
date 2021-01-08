using ColoryrBuild.Windows;
using DiffPlex.DiffBuilder.Model;
using Lib.Build;
using Lib.Build.Object;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ColoryrBuild
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 运行路径
        /// </summary>
        public static string RunLocal { get; private set; }

        public static System.Windows.Forms.NotifyIcon notifyIcon;
        public static HttpUtils HttpUtils = new();
        public static bool IsLogin { get; private set; }
        public static ConfigObj Config { get; private set; }
        public static MainWindow MainWindow_;
        public static ContrastWindow ContrastWindow_;
        public static LogWindow LogWindow_;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RunLocal = AppDomain.CurrentDomain.BaseDirectory;
            CodeSave.Start();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Config = ConfigSave.Config(new ConfigObj
            {
                Name = "",
                Token = "",
                Http = "https://"
            }, RunLocal + "Config.json");

            notifyIcon = new();
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "ColoryrWork编辑器";
            notifyIcon.Click += NotifyIcon_Click;

            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Log("启动");
            ShowA("登录", "账户未登录");
            while (!IsLogin)
            {
                Login();
            }
            ContrastWindow_ = new();
            ContrastWindow_.Show();
        }

        public static void Login()
        {
            IsLogin = false;
            new Login().ShowDialog();
        }

        public static void Log(string data)
        {
            if (LogWindow_ == null)
            {
                LogWindow_ = new();
                LogWindow_.Show();
            }
            LogWindow_.Log(data);
        }

        public static DiffPaneModel StartContrast(CSFileCode obj, string old)
        {
            if (ContrastWindow_ == null)
            {
                ContrastWindow_ = new();
                ContrastWindow_.Show();
            }

            return ContrastWindow_.Start(obj, old);
        }

        public static void ShowB(string v1, string v2)
        {
            notifyIcon.ShowBalloonTip(100, v1, v2, System.Windows.Forms.ToolTipIcon.Error);
            Log(v1 + "|" + v2);
        }

        public static void ShowA(string v1, string v2)
        {
            notifyIcon.ShowBalloonTip(100, v1, v2, System.Windows.Forms.ToolTipIcon.Info);
            Log(v1 + "|" + v2);
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            MainWindow_?.Activate();
        }

        public static async Task<bool> StartLogin()
        {
            var res = await HttpUtils.Login();
            ConfigSave.Save(Config, RunLocal + "Config.json");
            return res;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                MessageBox.Show("捕获未处理异常:" + e.Exception.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生错误" + ex.ToString());
            }

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder sbEx = new StringBuilder();
            if (e.IsTerminating)
            {
                sbEx.Append("发生错误，将关闭\n");
            }
            sbEx.Append("捕获未处理异常：");
            if (e.ExceptionObject is Exception)
            {
                sbEx.Append(((Exception)e.ExceptionObject).ToString());
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }
            MessageBox.Show(sbEx.ToString());
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show("捕获线程内未处理异常：" + e.Exception.ToString());
            e.SetObserved();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            notifyIcon.Dispose();
        }
    }
}
