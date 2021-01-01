using System.Windows;

namespace ColoryrBuild
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 运行路径
        /// </summary>
        public static string RunLocal { get; set; }

        public static System.Windows.Forms.NotifyIcon notifyIcon;
    }
}
