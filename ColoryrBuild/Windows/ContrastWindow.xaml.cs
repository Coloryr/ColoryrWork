using DiffPlex.DiffBuilder.Model;
using Lib.Build.Object;
using System.ComponentModel;
using System.Windows;

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
            Title = $"代码对比{obj.Type}[{obj.UUID}]";
            DiffView.OldText = oldText;
            DiffView.NewText = obj.Code;
            DiffView.Refresh();
            DiffView.ShowSideBySide();
            Activate();
            return DiffView.GetInlineDiffModel();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            App.ContrastWindow_ = null;
        }

        public void Clear()
        {
            Title = "代码对比";
            DiffView.OldText = "";
            DiffView.NewText = "";
            DiffView.Refresh();
        }
    }
}
