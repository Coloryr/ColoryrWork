using ColoryrBuild.View;
using ColoryrBuild.Views;
using ColoryrBuild.Views.CodeList;
using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using DiffPlex.DiffBuilder.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void Refresh();
        public static event Refresh CallRefresh;
        public static Action<CodeEditView> SwitchTo;
        public static Action<CodeEditView> AddCodeEdit;
        public static Action<CodeEditView> CloseCodeEdit;

        private Dictionary<CodeEditView, TabItem> Views = new();

        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow_ = this;
            SwitchTo = FSwitchTo;
            AddCodeEdit = FAddCodeEdit;
            CloseCodeEdit = FCloseCodeEdit;
            GetApi();
            CallRefresh.Invoke();
        }

        private async void GetApi()
        {
            string dir = App.RunLocal + "CodeTEMP/SDK/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            foreach (var item in Directory.GetFiles(dir))
            {
                File.Delete(item);
            }
            var list = await App.HttpUtils.GetApi();
            foreach (var item in list.List)
            {
                File.WriteAllText(dir + item.Key + ".cs", item.Value);
            }
        }

        public void RefreshCode(CodeType type)
        {
            switch (type)
            {
                case CodeType.Dll:
                    WebApiView.Action();
                    break;
                case CodeType.Class:
                    ClassView.Action();
                    break;
                case CodeType.Socket:
                    SocketView.Action();
                    break;
                case CodeType.Robot:
                    RobotView.Action();
                    break;
                case CodeType.WebSocket:
                    WebSocketView.Action();
                    break;
                case CodeType.Mqtt:
                    MqttView.Action();
                    break;
                case CodeType.Task:
                    TaskView.Action();
                    break;
                case CodeType.Web:
                    WebView.Action();
                    break;
            }
        }
        
        private void Window_Closed(object sender, CancelEventArgs e)
        {
            if (new ChoseWindow("关闭", "你确定要关闭编辑器吗").Set())
            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void FSwitchTo(CodeEditView view)
        {
            if (Views.TryGetValue(view, out var temp))
            {
                Tabs.SelectedItem = temp;
            }
        }

        private void FAddCodeEdit(CodeEditView view)
        {
            TabItem item = new()
            {
                Content = view,
                Header = $"代码编辑{view.type}[{view.obj.UUID}]"
            };
            item.SetValue(StyleProperty, Application.Current.Resources["TabItem"]);
            Tabs.Items.Add(item);
            Views.Add(view, item);
            Tabs.SelectedItem = item;
        }

        private void FCloseCodeEdit(CodeEditView view) 
        {
            if (Views.TryGetValue(view, out var temp))
            {
                Tabs.Items.Remove(temp);
            }
            Views.Remove(view);
            view.Close();
            GC.Collect();
        }

        public DiffPaneModel Start(CSFileCode obj, string oldText)
        {
            Title = $"代码对比{obj.Type}[{obj.UUID}]";
            DiffView.OldText = oldText;
            DiffView.NewText = obj.Code;
            DiffView.Refresh();
            Tabs.SelectedIndex = 2;
            return DiffView.GetInlineDiffModel();
        }

        public DiffPaneModel Start(CodeType type, string uuid, string new_, string oldText)
        {
            Title = $"代码对比{type}[{uuid}]";
            DiffView.OldText = oldText;
            DiffView.NewText = new_;
            DiffView.Refresh();
            Tabs.SelectedIndex = 2;
            return DiffView.GetInlineDiffModel();
        }

        public void Clear()
        {
            Title = "代码对比";
            DiffView.OldText = " ";
            DiffView.NewText = " ";
            DiffView.Refresh();
        }
    }
}
