using ColoryrWork.Lib.Build;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ColoryrBuild.Windows
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private Task LoginTask;
        private CancellationTokenSource cancel;
        private bool IsLogin = false;
        public Login()
        {
            InitializeComponent();
            cancel = new();
            Addr.Text = App.Config.Http;
            User.Text = App.Config.Name;
            Token.IsChecked = App.Config.SaveToken;
            LoginTask = Task.Run(async () =>
            {
                if (App.Config.SaveToken && !string.IsNullOrWhiteSpace(App.Config.Token))
                {
                    App.LogShow("登录", "检查登录");
                    var res = await App.AutoLogin();
                    if (res)
                    {
                        App.LogShow("登录", "已自动登录");
                        await App.HttpUtils.InitLog();
                        Dispatcher.Invoke(() => Close());
                    }
                }
            }, cancel.Token);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsLogin)
                return;
            IsLogin = true;
            Addr.IsEnabled = User.IsEnabled = Pass.IsEnabled = ButtonLogin.IsEnabled = false;
            App.Config.Http = Addr.Text;
            App.Config.Name = User.Text;
            string pass = Pass.Password;
            if (string.IsNullOrWhiteSpace(App.Config.Http)
                || string.IsNullOrWhiteSpace(App.Config.Name)
                || string.IsNullOrWhiteSpace(pass))
            {
                _ = new InfoWindow("登录错误", "请输入登录信息");
                Addr.IsEnabled = User.IsEnabled = Pass.IsEnabled = ButtonLogin.IsEnabled = true;
                IsLogin = false;
            }
            App.Config.SaveToken = Token.IsChecked == true;
            var res = await App.StartLogin(BuildUtils.GetSHA1(pass));
            if (res)
            {
                _ = App.HttpUtils.InitLog();
                if (LoginTask.Status == TaskStatus.Running)
                {
                    cancel.Cancel(false);
                }
                Close();
            }
            else
            {
                _ = new InfoWindow("登录错误", "服务器无响应");
            }
            Addr.IsEnabled = User.IsEnabled = Pass.IsEnabled = ButtonLogin.IsEnabled = true;
            IsLogin = false;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!App.IsLogin)
            {
                App.Close();
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            App.LoginWindow_ = null;
            MainWindow.LoginDone();
        }
    }
}
