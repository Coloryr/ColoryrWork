﻿using DiffPlex.DiffBuilder.Model;
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
            Write = true;
            obj1.Code = CodeSave.Load(Local + "main.cs");
            Dispatcher.Invoke(() =>
            {
                Model = App.StartContrast(obj1, old);
                textEditor.Text = obj1.Code;
            });
            Write = false;
        }

        public async void GetCode()
        {
            Write = true;
            if (type != CodeType.App)
            {
                var data = await App.HttpUtils.GetCode(type, obj.UUID);
                if (data == null)
                {
                    App.LogShow("获取代码", $"代码{obj1.Type}[{obj1.UUID}]获取错误");
                    Write = false;
                    return;
                }
                else
                    obj1 = data;

                textEditor.Text = obj1.Code;
                old = obj1.Code;
                Text.Text = obj1.Text;
                if (File.Exists(Local + "main.cs"))
                {
                    string time = string.Format("{0:s}", DateTime.Now).Replace(":", "_");
                    File.Move(Local + "main.cs", Local + time + "-main.cs");
                }
                CodeSave.Save(Local + "main.cs", obj1.Code);
            }
            else
            {
                var data = await App.HttpUtils.GetAppCode(obj.UUID);
                if (data == null)
                {
                    App.LogShow("获取代码", $"代码{obj1.Type}[{obj1.UUID}]获取错误");
                    Write = false;
                    return;
                }
                else
                    obj2 = data;

            }
            App.LogShow("获取代码", $"代码{obj1.Type}[{obj1.UUID}]获取成功");
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

        private async void BuildOther()
        {
            old = obj1.Code;
            obj1.Text = Text.Text;
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
                EditFun type = EditFun.Edit;
                switch (item.Type)
                {
                    case ChangeType.Deleted:
                        type = EditFun.Remove;
                        break;
                    case ChangeType.Inserted:
                        type = EditFun.Add;
                        break;
                    case ChangeType.Modified:
                    case ChangeType.Imaginary:
                        type = EditFun.Edit;
                        break;
                }
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
            App.MainWindow_.Re(type);
            CodeSave.Save(Local + "main.cs", obj1.Code);
            App.ClearContrast();
        }

        private async void BuildApp()
        { 
            
        }

        private void Build_Click(object sender, RoutedEventArgs e)
        {
            if (Write)
                return;
            Write = true;
            if (type != CodeType.App)
            {
                BuildOther();
            }
            else
            {
                BuildApp();
            }
            Write = false;
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
