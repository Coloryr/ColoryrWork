using System.Windows;
using System.Windows.Input;

namespace ColoryrBuild.Windows
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        public string Data { get; set; }
        public InputWindow(string title, string data = "")
        {
            InitializeComponent();
            Data = data;
            Title = title;
            DataContext = this;
        }

        public string Set()
        {
            ShowDialog();
            return Data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Data = Text.Text;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Data = "";
            Close();
        }

        private void Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Data = Text.Text;
                Close();
            }
        }
    }
}
