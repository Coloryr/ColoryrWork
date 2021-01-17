using System.Threading.Tasks;
using System.Windows;

namespace ColoryrBuild.Windows
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            Addr.Text = App.Config.Http;
            User.Text = App.Config.Name;
            Token.IsChecked = App.Config.SaveToken;
            Task.Run(async () =>
            {
                if (App.Config.SaveToken)
                {
                    Dispatcher.Invoke(() => App.ShowA("登录", "检查登录"));
                    var res = await App.AutoLogin();
                    if (res)
                    {
                        App.ShowA("登录", "已自动登录");
                        Dispatcher.Invoke(() => Close());
                    }
                }
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Config.Http = Addr.Text;
            App.Config.Name = User.Text;
            App.Config.SaveToken = Token.IsChecked == true;
            var res = await App.StartLogin(Utils.GetSHA1(Pass.Password));
            if (res)
            {
                Close();
            }
        }
    }
}
