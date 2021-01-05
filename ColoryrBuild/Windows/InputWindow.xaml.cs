using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColoryrBuild.Windows
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        public string Data { get; set; }
        public InputWindow(string title)
        {
            InitializeComponent();
            Title = title;
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
    }
}
