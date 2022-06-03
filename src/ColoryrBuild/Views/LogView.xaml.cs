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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColoryrBuild.Views
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class LogView : UserControl
    {
        public static Action<string> AddLog;
        public LogView()
        {
            InitializeComponent();

            AddLog = FAddLog;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Text.Text = "";
        }

        public void FAddLog(string text)
        {
            Text.AppendText(text + Environment.NewLine);
        }
    }
}
