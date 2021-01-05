using System.Windows;

namespace ColoryrBuild.Windows
{
    /// <summary>
    /// ChoseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChoseWindow : Window
    {
        public bool ASet;
        public ChoseWindow(string title, string text)
        {
            InitializeComponent();
            Title = title;
            Text.Text = text;
        }

        public bool Set()
        {
            ShowDialog();
            return ASet;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ASet = true;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ASet = false;
            Close();
        }
    }
}
