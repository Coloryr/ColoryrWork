using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit.Folding;
using Lib.Build;
using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ColoryrBuild.Windows
{
    /// <summary>
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        private string old;
        private readonly CSFileObj obj;
        private readonly CodeType type;
        private CSFileCode obj1;
        private AppFileObj obj2;
        private DiffPaneModel Model;
        private readonly FileSystemWatcher FileSystemWatcher;
        private readonly string Local;
        private bool Write;

        public EditWindow(CSFileObj obj, CodeType type)
        {
            InitializeComponent();
            this.obj = obj;
            var foldingManager = FoldingManager.Install(textEditor.TextArea);
            var foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            textEditor.Options.ShowSpaces = true;
            textEditor.Options.ShowTabs = true;
            this.type = type;
            Title = $"编辑窗口{type}[{obj.UUID}]";
            if (type != CodeType.App)
                CodeA.IsEnabled = false;
            Local = CodeSave.FilePath + "\\" + type.ToString() + "\\" + obj.UUID + "\\";
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            GetCode();
            FileSystemWatcher = new FileSystemWatcher();
            try
            {
                FileSystemWatcher.Path = Local;
                FileSystemWatcher.Filter = "main.cs";
                FileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
                FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                FileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (Write)
                return;
            CodeSave.Load(Local + "main.cs");
        }

        public async void GetCode()
        {
            Write = true;
            if (type != CodeType.App)
            {
                var data = await App.HttpUtils.GetCode(type, obj.UUID);
                if (data == null)
                {
                    App.LogShow("获取错误", "代码获取错误");
                    Write = false;
                    return;
                }
                else
                    obj1 = data;

                textEditor.Text = obj1.Code;
                old = obj1.Code;
                Text.Text = obj1.Text;

                CodeSave.Save(Local + "main.cs", obj1.Code);
            }
            else
            {
                var data = await App.HttpUtils.GetAppCode(obj.UUID);
                if (data == null)
                {
                    App.LogShow("获取错误", "代码获取错误");
                    Write = false;
                    return;
                }
                else
                    obj2 = data;

            }
            Write = false;
        }

        private void Updata_Click(object sender, RoutedEventArgs e)
        {
            obj.Text = obj1.Text = Text.Text;
            obj1.Code = textEditor.Text;
            Model = App.StartContrast(obj1, old);
        }

        private void ReCode_Click(object sender, RoutedEventArgs e)
        {
            GetCode();
        }

        private async void Build_Click(object sender, RoutedEventArgs e)
        {
            old = obj1.Code;
            obj.Text = Text.Text;
            List<CodeEditObj> list = new();
            if (Model == null)
            {
                Updata_Click(null, null);
            }
            for (int pos = 0; pos < Model.Lines.Count; pos++)
            {
                var item = Model.Lines[pos];
                if (item.Type == ChangeType.Unchanged)
                    continue;
                EditFun type = item.Type switch
                {
                    ChangeType.Deleted => EditFun.Remove,
                    ChangeType.Inserted => EditFun.Add,
                    ChangeType.Modified => EditFun.Edit,
                    ChangeType.Imaginary => EditFun.Edit
                };
                list.Add(new()
                {
                    Code = item.Text,
                    Fun = type,
                    Line = pos
                });
            }
            var data = await App.HttpUtils.Build(obj1, type, list);
            if (data == null)
            {
                App.LogShow("编译", "服务器返回错误");
                return;
            }
            App.LogShow("编译", data.Message);
            obj1.Version++;
            CodeSave.Save(Local + "main.cs", obj1.Code);
            App.ClearContrast();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.CloseEdit(obj, type);
        }
    }
}
