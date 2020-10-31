using Lib.Build;
using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ColoryrBuild
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 配置文件
        /// </summary>
        public static ConfigOBJ Config { get; set; }
        /// <summary>
        /// 运行路径
        /// </summary>
        public static string RunLocal { get; set; }
        /// <summary>
        /// 主窗口
        /// </summary>
        public static MainWindow MainWindow_ { get; set; }
        public static Login Login_ { get; set; }
        public static System.Windows.Forms.NotifyIcon notifyIcon;
        public static bool isLogin;
        public static bool UserAdmin = false;

        public static Logs logs { get; set; }
        public static Dictionary<string, Window> CodeList { get; private set; } = new Dictionary<string, Window>();
        public static void SatrtLogin()
        {
            isLogin = false;
            if (Login_ == null)
            {
                Login_ = new Login();
                Login_.ShowDialog();
            }
            Login_.Activate();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            RunLocal = AppDomain.CurrentDomain.BaseDirectory;
            logs = new Logs(RunLocal);
            new Config();
            new CodeSave();
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            notifyIcon.Visible = true;
            CheckLogin();
        }
        public static void Show(string data, int time = 200)
        {
            notifyIcon.BalloonTipText = data;
            notifyIcon.ShowBalloonTip(time);
        }
        public static void OpenCodeWin(CSFileCode code)
        {
            Window win;
            if (string.IsNullOrWhiteSpace(code.UUID))
            {
                win = new CodeWin(code);
            }
            else
            {
                if (CodeList.ContainsKey(code.UUID))
                {
                    CodeList[code.UUID].Activate();
                    return;
                }
                else
                {
                    win = new CodeWin(code);
                    CodeList.Add(code.UUID, win);
                }
            }
            win.Show();
        }
        public static void CloseWin(string uuid)
        {
            if (uuid != null)
                CodeList.Remove(uuid);
        }
        public static async void CheckLogin()
        {
            try
            {
                var data = new BuildOBJ
                {
                    Mode = ReType.CheckLogin,
                    User = Config.User,
                    Token = Config.Token
                };
                if (Config.Url != null && Config.User != null && Config.Token != null)
                {
                    var obj = await Http.GetAsync(data);
                    if (obj == null)
                    {
                        Show("登录检查失败");
                        return;
                    }
                    var re = obj.ToObject<ReMessage>();
                    if (re.Build == true)
                    {
                        Show("账户登录成功");
                        UserAdmin = re.UseTime == "1";
                        isLogin = true;
                    }
                }
            }
            catch (Exception e)
            {
                logs.LogError(e);
            }
        }

        public static void Quit()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Current.Shutdown();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true;
                MessageBox.Show("捕获未处理异常:" + e.Exception.Message);
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
                sbEx.Append(((Exception)e.ExceptionObject).Message);
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }
            MessageBox.Show(sbEx.ToString());
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show("捕获线程内未处理异常：" + e.Exception.Message);
            e.SetObserved();
        }
    }
}
