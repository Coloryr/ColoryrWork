using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Lib.Build.Object;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace ColoryrBuild
{
    /// <summary>
    /// CodeWin.xaml 的交互逻辑
    /// </summary>
    public partial class CodeWin : Window
    {
        private TextEditor Editor;
        private bool IsBuild;
        private CSFileCode Obj;
        private bool IsLock;
        private bool IsNew;
        private bool Last;
        private FileSystemWatcher FileSystemWatcher;
        public CodeWin(CSFileCode obj)
        {
            InitializeComponent();
            Editor = new TextEditor();
            Editor.ShowLineNumbers = true;
            Editor.FontFamily = new FontFamily("Console");
            Editor.FontSize = 14;
            Editor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55DADADA"));
            Editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            Editor.Options.ShowSpaces = true;
            Editor.Options.ShowTabs = true;
            st.Children.Add(Editor);
            Obj = obj;
            DataContext = Obj;
            IsNew = true;
            if (!string.IsNullOrWhiteSpace(obj.UUID))
            {
                IsNew = false;
                ID.IsReadOnly = true;
                IsLock = true;
                Editor.Text = obj.Code;
                KeyB.IsEnabled = false;
            }
            CodeSave.Save(obj);
            FileSystemWatcher = new FileSystemWatcher();
            try
            {
                FileSystemWatcher.Path = CodeSave.FilePath;
                FileSystemWatcher.Filter = CodeSave.GetFileName(obj);
                FileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
                FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                FileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private async void Build(bool read)
        {
            if (read)
            {
                CodeSave.Load(Obj);
                Dispatcher.Invoke(() => Editor.Text = Obj.Code);
            }
            App.Show("正在编译", 500);
            Dispatcher.Invoke(() =>
            {
                ID.IsReadOnly =
                Text.IsReadOnly = true;
                Editor.IsEnabled = false;
                BuildB.IsEnabled = false;
                KeyB.IsEnabled = false;
                Obj.Code = Editor.Text;
            });
            ReType type = ReType.AddDll;
            switch (Obj.Type)
            {
                case CodeType.Dll:
                    type = ReType.AddDll;
                    break;
                case CodeType.Class:
                    type = ReType.AddClass;
                    break;
                case CodeType.IoT:
                    type = ReType.AddIoT;
                    break;
                case CodeType.WebSocket:
                    type = ReType.AddWebSocket;
                    break;
                case CodeType.Robot:
                    type = ReType.AddRobot;
                    break;
            }
            var SendData = new BuildOBJ
            {
                UUID = Obj.UUID,
                Code = Obj.Code,
                User = App.Config.User,
                Token = App.Config.Token,
                Text = Obj.Text,
                Version = Obj.Version,
                Mode = type
            };
            var ReObj = await Http.GetAsync(SendData);
            if (ReObj == null)
                App.Show("编译服务器返回错误");
            else
            {
                var temp = ReObj.ToObject<ReMessage>();
                if (temp.Build == true)
                {
                    Obj.Version++;
                    App.Show("编译成功，用时" + temp.UseTime + "ms");
                    Dispatcher.Invoke(() =>
                    {
                        ID.IsReadOnly = true;
                    });
                    IsLock = true;
                    switch (Obj.Type)
                    {
                        case CodeType.Dll:
                            App.MainWindow_.DllRe();
                            type = ReType.CodeDll;
                            break;
                        case CodeType.Class:
                            App.MainWindow_.ClassRe();
                            type = ReType.CodeClass;
                            break;
                        case CodeType.IoT:
                            App.MainWindow_.IoTRe();
                            type = ReType.CodeIoT;
                            break;
                        case CodeType.WebSocket:
                            App.MainWindow_.WSRe();
                            type = ReType.CodeWebSocket;
                            break;
                        case CodeType.Robot:
                            App.MainWindow_.RobotRe();
                            type = ReType.CodeRobot;
                            break;
                    }
                    if (IsNew)
                    {
                        SendData = new BuildOBJ
                        {
                            Mode = type,
                            User = App.Config.User,
                            Token = App.Config.Token,
                            UUID = Obj.UUID
                        };
                        ReObj = await Http.GetAsync(SendData);
                        if (ReObj == null)
                            return;

                        if (ReObj.ToObject<ReMessage>().Message == "233")
                            Dispatcher.Invoke(() => App.SatrtLogin());
                        else
                        {
                            App.OpenCodeWin(ReObj.ToObject<CSFileCode>());
                            Close();
                        }
                    }
                }
                else
                {
                    if (temp.Message == "233")
                    {
                        App.Show("登陆超时");
                        Dispatcher.Invoke(() => App.SatrtLogin());
                    }
                    else
                    {
                        App.Show("编译失败");
                        MessageBox.Show(temp.Message);
                    }
                }
            }
            Dispatcher.Invoke(() =>
            {
                Text.IsReadOnly = false;
                Editor.IsEnabled =
                BuildB.IsEnabled = true;
                if (!IsLock)
                    KeyB.IsEnabled = true;
            });
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Last = !Last;
            if (Last)
            {
                return;
            }
            if (IsBuild)
                return;
            IsBuild = true;
            Build(true);
            IsBuild = false;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsBuild)
                return;
            IsBuild = true;
            Build(false);
            IsBuild = false;
            CodeSave.Save(Obj);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            var data = Guid.NewGuid();
            ID.Text = data.ToString().Replace("-", "");
            Obj.UUID = ID.Text;
        }

        private void KeyC_Click(object sender, RoutedEventArgs e)
        {
            switch (Obj.Type)
            {
                case CodeType.Dll:
                    if (App.UserAdmin)
                        Editor.Text += CodeDemo.main_.Replace("{name}", ID.Text);
                    else
                        Editor.Text += CodeDemo.main;
                    break;
                case CodeType.Class:
                    if (App.UserAdmin)
                        Editor.Text += CodeDemo.class_.Replace("{name}", ID.Text);
                    break;
                case CodeType.IoT:
                    if (App.UserAdmin)
                        Editor.Text += CodeDemo.iot_.Replace("{name}", ID.Text);
                    break;
                case CodeType.WebSocket:
                    if (App.UserAdmin)
                        Editor.Text += CodeDemo.websocket_.Replace("{name}", ID.Text);
                    break;
                case CodeType.Robot:
                    if (App.UserAdmin)
                        Editor.Text += CodeDemo.robot_.Replace("{name}", ID.Text);
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            FileSystemWatcher.Dispose();
            App.CloseWin(Obj.UUID);
        }

        private async void KeyC_Click_1(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("这会覆盖你的代码，你确定要重新获取吗", "重新获取源码", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                ReType type = ReType.AddDll;
                switch (Obj.Type)
                {
                    case CodeType.Dll:
                        type = ReType.CodeDll;
                        break;
                    case CodeType.Class:
                        type = ReType.CodeClass;
                        break;
                    case CodeType.IoT:
                        type = ReType.CodeIoT;
                        break;
                    case CodeType.WebSocket:
                        type = ReType.CodeWebSocket;
                        break;
                    case CodeType.Robot:
                        type = ReType.CodeRobot;
                        break;
                }
                var SendData = new BuildOBJ
                {
                    Mode = type,
                    User = App.Config.User,
                    Token = App.Config.Token,
                    UUID = Obj.UUID
                };
                var ReObj = await Http.GetAsync(SendData);
                if (ReObj == null)
                    return;

                if (ReObj.ToObject<ReMessage>().Message == "233")
                    Dispatcher.Invoke(() => App.SatrtLogin());
                else
                {
                    App.OpenCodeWin(ReObj.ToObject<CSFileCode>());
                    Close();
                }
            }
        }
    }
}
