using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
