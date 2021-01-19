using System.Windows;

namespace ColoryrBuild.Windows
{
    /// <summary>
    /// LogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        public void Log(string data)
        {
            Dispatcher.Invoke(() =>
            {
                Text.AppendText(data + "\n");
                Text.ScrollToEnd();
                Activate();
            });
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            App.LogWindow_ = null;
        }
    }
}
