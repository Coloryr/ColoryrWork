﻿using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit.Folding;
using Lib.Build;
using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        private string old;
        private CSFileObj obj;
        private CodeType type;
        private CSFileCode obj1;
        private DiffPaneModel Model;
        private FileSystemWatcher FileSystemWatcher;
        private string Local;
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
            Title = "代码编辑" + obj.UUID;
            if (type != CodeType.App)
                CodeA.IsEnabled = false;
            Local = CodeSave.FilePath + "\\" + type.ToString() + "\\" + obj.UUID + "\\";
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
                    App.ShowB("获取错误", "代码获取错误");
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
                var data = App.HttpUtils.GetAppCode(obj.UUID);

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
            foreach (var item in Model.Lines)
            {
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
                      Fun = type
                });
            }
            var data = await App.HttpUtils.Build(obj1, list);
            if (data.Build)
            {
                App.ShowA("编译", data.Message);
                CodeSave.Save(Local + "main.cs", obj1.Code);
            }
            else
            {
                App.ShowB("编译错误", data.Message);
            }
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
    }
}
