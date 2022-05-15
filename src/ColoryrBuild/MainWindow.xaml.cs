using ColoryrBuild.View;
using ColoryrBuild.Views;
using ColoryrBuild.Views.CodeList;
using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
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
        public static Action<CodeEdit> SwitchTo;
        public static Action<CodeEdit> AddCodeEdit;

        private Dictionary<CodeEdit, TabItem> Views = new();

        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow_ = this;
            SwitchTo = FSwitchTo;
            AddCodeEdit = FAddCodeEdit;
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

        private void FSwitchTo(CodeEdit view)
        {
            if (Views.TryGetValue(view, out var temp))
            {
                Tabs.SelectedItem = temp;
            }
        }

        private void FAddCodeEdit(CodeEdit view)
        {
            TabItem item = new()
            {
                Content = view
            };
            item.SetValue(StyleProperty, Application.Current.Resources["TabItem"]);
            Tabs.Items.Add(item);
            Views.Add(view, item);
            Tabs.SelectedItem = item;
        }

        private void FCloseCodeEdit(CodeEdit view) 
        {
            if (Views.TryGetValue(view, out var temp))
            {
                Tabs.Items.Remove(temp);
            }
        }
    }
}
