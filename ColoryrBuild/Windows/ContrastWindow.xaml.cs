﻿using DiffPlex.DiffBuilder.Model;
using Lib.Build.Object;
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
            Activate();
            return DiffView.GetInlineDiffModel();
        }

        public DiffPaneModel Start(CodeType type, string uuid, string new_, string oldText)
        {
            Title = $"代码对比{type}[{uuid}]";
            DiffView.OldText = oldText;
            DiffView.NewText = new_;
            DiffView.Refresh();
            Activate();
            return DiffView.GetInlineDiffModel();
        }

        public void Clear()
        {
            Title = "代码对比";
            DiffView.OldText = " ";
            DiffView.NewText = " ";
            DiffView.Refresh();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            App.ContrastWindow_ = null;
        }
    }
}
