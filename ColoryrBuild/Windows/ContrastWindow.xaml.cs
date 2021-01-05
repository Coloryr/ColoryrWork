using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// ContrastWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ContrastWindow : Window
    {
        public ContrastWindow()
        {
            InitializeComponent();
        }

        public DiffPaneModel Start(CSFileCode obj, string oldText)
        {
            Title = "代码对比:" + obj.UUID;
            DiffView.OldText = oldText;
            DiffView.NewText = obj.Code;
            DiffView.Refresh();

            return DiffView.GetInlineDiffModel();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            App.ContrastWindow_ = null;
        }
    }
}
