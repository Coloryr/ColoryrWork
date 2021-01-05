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
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Config.Http = Addr.Text;
            App.Config.Name = User.Text;
            App.Config.SaveToken = Token.IsChecked == true;
            var res = await App.StartLogin();
            if (res)
            {
                Close();
            }
        }
    }
}
