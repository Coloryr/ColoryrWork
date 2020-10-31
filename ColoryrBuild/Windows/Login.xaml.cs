using Lib.Build.Object;
using System;
using System.Windows;

namespace ColoryrBuild
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            DataContext = App.Config;
        }
        private string password;
        public async void DoLogin()
        {
            try
            {
                var data = new BuildOBJ
                {
                    Code = password,
                    User = App.Config.User,
                    Mode = ReType.Login
                };
                var ReObj = await Http.GetAsync(data);
                if (ReObj == null)
                {
                    App.Show("登录失败-服务器返回错误");
                    return;
                }
                try
                {
                    ReMessage ReMessage = ReObj.ToObject<ReMessage>();
                    if (ReMessage.Build == true)
                    {
                        App.UserAdmin = ReMessage.UseTime == "1";
                        App.Config.Token = ReObj["Message"].ToString();
                        Config.Save();
                        App.Show("登录成功");
                        App.isLogin = true;
                    }
                    else
                        App.Show("登录失败:" + ReMessage.Message);
                }
                catch
                {
                    App.Show("编译服务器返回错误");
                }
            }
            catch (Exception e)
            {
                App.logs.LogError(e);
            }
            showMain();
        }
        private void showMain()
        {
            if (App.isLogin)
            {
                if (App.MainWindow_ == null)
                {
                    App.MainWindow_ = new MainWindow();
                    App.MainWindow_.Show();
                }
                else
                {
                    App.MainWindow_.Activate();
                }
                Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            password = Password.Password;
            Config.Save();
            Lock(true);
            DoLogin();
            Lock(false);
        }

        private void Lock(bool lock_)
        {
            lock_ = !lock_;
            User.IsEnabled =
            Password.IsEnabled =
            Button.IsEnabled =
            lock_;
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            var data = new BuildOBJ
            {
                Mode = ReType.GetApi,
                User = App.Config.User,
                Token = App.Config.Token
            };
            var ReObj = await Http.GetAsync(data);
            if (ReObj == null)
                return;
            try
            {
                foreach (var item in ReObj.ToObject<APIFileObj>().list)
                {
                    CodeSave.Save(item.Key, item.Value);
                }
            }
            catch
            {
                App.Show("SDK获取失败");
            }
            App.Login_ = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            showMain();
        }
    }
}
