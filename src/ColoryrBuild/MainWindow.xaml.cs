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
        public static Action<UserControl> SwitchTo;
        public static Action<UserControl> AddCodeEdit;
        public static Action<UserControl> CloseCodeEdit;
        public static Action LoginDone;

        private Dictionary<UserControl, TabItem> Views = new();

        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow_ = this;
            SwitchTo = FSwitchTo;
            AddCodeEdit = FAddCodeEdit;
            CloseCodeEdit = FCloseCodeEdit;
            LoginDone = FLoginDone;
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
            if (list.List != null)
                foreach (var item in list.List)
                {
                    File.WriteAllText(dir + item.Key + ".cs", item.Value);
                }
            dir = App.RunLocal + "CodeTEMP/Common/";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (list.Other != null)
                foreach (var item in list.Other)
                {
                    File.WriteAllText(dir + item.Key + ".cs", item.Value);
                }
        }

        public void RefreshCode(CodeType type)
        {
            switch (type)
            {
                case CodeType.Dll:
                    WebApiView.Refresh();
                    break;
                case CodeType.Class:
                    ClassView.Refresh();
                    break;
                case CodeType.Socket:
                    SocketView.Refresh();
                    break;
                case CodeType.Robot:
                    RobotView.Refresh();
                    break;
                case CodeType.WebSocket:
                    WebSocketView.Refresh();
                    break;
                case CodeType.Mqtt:
                    MqttView.Refresh();
                    break;
                case CodeType.Task:
                    TaskView.Refresh();
                    break;
                case CodeType.Web:
                    WebView.Refresh();
                    break;
            }
        }

        public void FLoginDone() 
        {
            GetApi();
            CallRefresh.Invoke();
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

        private void FSwitchTo(UserControl view)
        {
            if (Views.TryGetValue(view, out var temp))
            {
                Tabs.SelectedItem = temp;
            }
        }

        private void FAddCodeEdit(UserControl view)
        {
            var view1 = view as IEditView;
            TabItem item = new()
            {
                Content = view,
                Header = $"代码编辑{view1.Type}[{view1.Obj.UUID}]"
            };
            item.SetValue(StyleProperty, Application.Current.Resources["TabItem"]);
            Tabs.Items.Add(item);
            Views.Add(view, item);
            Tabs.SelectedItem = item;
        }

        private void FCloseCodeEdit(UserControl view)
        {
            if (Views.TryGetValue(view, out var temp))
            {
                Tabs.Items.Remove(temp);
            }
            Views.Remove(view);
            var view1 = view as IEditView;
            view1.Close();
            GC.Collect();
        }

        public DiffPaneModel Start(CSFileCode obj, string oldText)
        {
            Title = $"代码对比{obj.Type}[{obj.UUID}]";
            DiffView.OldText = oldText;
            DiffView.NewText = obj.Code;
            DiffView.Refresh();
            Tabs.SelectedIndex = 3;
            return DiffView.GetInlineDiffModel();
        }

        public DiffPaneModel Start(CodeType type, string uuid, string new_, string oldText)
        {
            Title = $"代码对比{type}[{uuid}]";
            DiffView.OldText = oldText;
            DiffView.NewText = new_;
            DiffView.Refresh();
            Tabs.SelectedIndex = 3;
            return DiffView.GetInlineDiffModel();
        }

        public void Clear()
        {
            Title = (Tabs.SelectedItem as TabItem).Header as string;
            DiffView.OldText = " ";
            DiffView.NewText = " ";
            DiffView.Refresh();
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedItem == null)
                return;
            Title = (Tabs.SelectedItem as TabItem).Header as string;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            App.LogShow("启动", "初始化");
            App.Login();
        }
    }
}
