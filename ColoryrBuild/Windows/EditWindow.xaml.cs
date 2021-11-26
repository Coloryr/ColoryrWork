using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit.Folding;
using Lib.Build;
using Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Windows
{
    /// <summary>
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        private string old;
        private string thisfile;
        private readonly CSFileObj obj;
        private readonly CodeType type;
        private CSFileCode obj1;
        private AppFileObj obj2;
        private WebObj obj3;
        private DiffPaneModel Model;
        private readonly FileSystemWatcher FileSystemWatcher;
        private readonly string Local;
        private bool Write;

        public EditWindow(CSFileObj obj, CodeType type)
        {
            this.obj = obj;
            this.type = type;

            InitializeComponent();

            var foldingManager = FoldingManager.Install(textEditor.TextArea);
            var foldingStrategy = new XmlFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            textEditor.Options.ShowSpaces = true;
            textEditor.Options.ShowTabs = true;

            Title = $"编辑窗口{type}[{obj.UUID}]";
            if (type is not CodeType.App and not CodeType.Web)
                CodeA.IsEnabled = false;
            Local = $"{CodeSave.FilePath}/{type}/{obj.UUID}/";
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            GetCode();
            FileSystemWatcher = new FileSystemWatcher();
            try
            {
                FileSystemWatcher.Path = Local;
                FileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
                FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                FileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private async void OnChanged(object source, FileSystemEventArgs e)
        {
            if (Write)
                return;
            Write = true;
            if(type is CodeType.Web)
            {
                obj1.Code = CodeSave.Load(Local + e.Name);
                await Dispatcher.InvokeAsync(() =>
                {
                    Model = App.StartContrast(obj1, old);
                    textEditor.Text = obj1.Code;
                });
            }
            else if (type is not CodeType.App and not CodeType.Web)
            {
                if (e.Name == "main.cs")
                {
                    obj1.Code = CodeSave.Load(Local + "main.cs");
                    await Dispatcher.InvokeAsync(() =>
                    {
                        Model = App.StartContrast(obj1, old);
                        textEditor.Text = obj1.Code;
                    });
                }
            }
            else
            {
                
            }
            Write = false;
        }

        public async void GetCode()
        {
            if (Write)
                return;
            Write = true;
            if (type is not CodeType.App and not CodeType.Web)
            {
                var data = await App.HttpUtils.GetCode(type, obj.UUID);
                if (data == null)
                {
                    App.LogShow("获取代码", $"代码{type}[{obj1.UUID}]获取错误");
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
                    var newLocal = Local + "backup/";
                    if (!Directory.Exists(newLocal))
                    {
                        Directory.CreateDirectory(newLocal);
                    }
                    File.Move(Local + "main.cs", newLocal + time + ".cs");
                }
                CodeSave.Save(Local + "main.cs", obj1.Code);
                App.LogShow("获取代码", $"代码{obj1.Type}[{obj1.UUID}]获取成功");
            }
            else if (type == CodeType.Web)
            {
                var data = await App.HttpUtils.GetWebCode(obj.UUID);
                if (data == null)
                {
                    App.LogShow("获取代码", $"代码Web[{obj.UUID}]获取错误");
                    Write = false;
                    return;
                }
                else
                    obj3 = data;

                FileList.Items.Clear();

                string time = string.Format("{0:s}", DateTime.Now).Replace(":", "_");
                var newLocal = Local + $"backup_{time}/";
                if (!Directory.Exists(newLocal))
                {
                    Directory.CreateDirectory(newLocal);
                }

                foreach (var item in obj3.Codes)
                {
                    if (File.Exists(Local + item.Key))
                    {
                        File.Move(Local + item.Key, newLocal + item.Key);
                    }
                    CodeSave.Save(Local + item.Key, item.Value);
                    FileList.Items.Add(item.Key);
                }
                foreach (var item in obj3.Files)
                {
                    FileList.Items.Add(item.Key);
                }
                foreach (var item in FileList.Items)
                {
                    if (item as string == "index.html")
                    {
                        FileList.SelectedItem = item;
                        break;
                    }
                }
                thisfile = "index.html";
                old = textEditor.Text = obj3.Codes["index.html"];
                App.LogShow("获取代码", $"代码Web[{obj.UUID}]获取成功");
            }
            else
            {
                var data = await App.HttpUtils.GetAppCode(obj.UUID);
                if (data == null)
                {
                    App.LogShow("获取代码", $"代码APP[{obj.UUID}]获取错误");
                    Write = false;
                    return;
                }
                else
                    obj2 = data;

                FileList.Items.Clear();

                string time = string.Format("{0:s}", DateTime.Now).Replace(":", "_");
                var newLocal = Local + $"backup_{time}/";
                if (!Directory.Exists(newLocal))
                {
                    Directory.CreateDirectory(newLocal);
                }

                foreach (var item in obj2.Codes)
                {
                    if (File.Exists(Local + $"{item.Key}.cs"))
                    {
                        File.Move(Local + $"{item.Key}.cs", newLocal + $"{item.Key}.cs");
                    }
                    CodeSave.Save(Local + $"{item.Key}.cs", item.Value);
                    FileList.Items.Add($"{item.Key}.cs");
                }

                foreach (var item in obj2.Xamls)
                {
                    if (File.Exists(Local + $"{item.Key}.xaml"))
                    {
                        File.Move(Local + $"{item.Key}.xaml", newLocal + $"{item.Key}.xaml");
                    }
                    CodeSave.Save(Local + $"{item.Key}.xaml", item.Value);
                    FileList.Items.Add($"{item.Key}.xaml");
                }
                foreach (var item in obj2.Files)
                {
                    FileList.Items.Add(item.Key);
                }
                foreach (var item in FileList.Items)
                {
                    if ((item as string) == "main.cs")
                    {
                        FileList.SelectedItem = item;
                        break;
                    }
                }
                thisfile = "main.cs";
                old = textEditor.Text = obj2.Codes["main"];
                App.LogShow("获取代码", $"代码App[{obj2.UUID}]获取成功");
            }
            Write = false;
        }

        private async Task UpdateTask() 
        {
            Updata_Button.IsEnabled = false;
            if (type == CodeType.App)
            {
                obj2.Text = Text.Text;
                if (thisfile.EndsWith(".cs"))
                {
                    string temp = thisfile.Replace(".cs", "");
                    obj2.Codes[temp] = textEditor.Text;
                    Model = App.StartContrast(type, obj2.UUID, obj2.Codes[temp], old);
                }
                else if (thisfile.EndsWith(".xaml"))
                {
                    string temp = thisfile.Replace(".xaml", "");
                    obj2.Codes[temp] = textEditor.Text;
                    Model = App.StartContrast(type, obj2.UUID, obj2.Xamls[temp], old);
                }
                else
                {
                    App.LogShow("更新错误", "不支持修改该文件");
                }
            }
            else if (type == CodeType.Web)
            {
                obj3.Text = Text.Text;
                if (thisfile.EndsWith(".html") || thisfile.EndsWith(".css")
                || thisfile.EndsWith(".js") || thisfile.EndsWith(".json")
                || thisfile.EndsWith(".txt"))
                {
                    obj3.Codes[thisfile] = textEditor.Text;
                    Model = App.StartContrast(type, obj3.UUID, obj3.Codes[thisfile], old);
                }
                else
                {
                    App.LogShow("更新错误", "不支持修改该文件");
                }
            }
            else
            {
                obj1.Text = Text.Text;
                obj1.Code = textEditor.Text;
                await Dispatcher.InvokeAsync(() =>
                {
                    Model = App.StartContrast(obj1, old);
                });
            }
            Updata_Button.IsEnabled = true;
        }

        private async void Updata_Click(object sender, RoutedEventArgs e)
        {
            await UpdateTask();
        }

        private void ReCode_Click(object sender, RoutedEventArgs e)
        {
            ReCode_Button.IsEnabled = false;
            GetCode();
            ReCode_Button.IsEnabled = true;
        }

        private bool IsBuild = false;

        private async void BuildOther()
        {
            if (IsBuild)
                return;
            IsBuild = true;
            await UpdateTask();
            old = obj1.Code;
            obj1.Text = Text.Text;
            List<CodeEditObj> list = new();
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
            App.LogShow("编译", "编译后\n" +
                $"结果:{data.Message}\n" +
                $"用时:{data.UseTime}\n" +
                $"最后更新时间:{data.Time}");
            obj1.Next();
            App.MainWindow_.Re(type);
            CodeSave.Save(Local + "main.cs", obj1.Code);
            App.ClearContrast();
            IsBuild = false;
        }

        private async void BuildApp()
        {
            ReType temp1 = ReType.AppUpdata;
            string temp = "";
            Updata_Click(null, null);
            if (thisfile.EndsWith(".cs"))
            {
                temp = thisfile.Replace(".cs", "");
                old = obj2.Codes[temp];
                temp1 = ReType.AppCsUpdata;
            }
            else if (thisfile.EndsWith(".xaml"))
            {
                temp = thisfile.Replace(".xaml", "");
                old = obj2.Xamls[temp];
                temp1 = ReType.AppXamlUpdata;
            }
            List<CodeEditObj> list = new();
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
            obj2.Text = Text.Text;
            var data = await App.HttpUtils.BuildApp(obj2, temp1, temp, list);
            if (data == null)
            {
                App.LogShow("编译", "服务器返回错误");
                return;
            }
            App.LogShow("编译", data.Message);
            obj2.Next();
            App.MainWindow_.Re(type);
            if (temp1 == ReType.AppCsUpdata)
            {
                CodeSave.Save(Local + $"{thisfile}.cs", obj2.Codes[temp]);
            }
            else if (temp1 == ReType.AppXamlUpdata)
            {
                CodeSave.Save(Local + $"{thisfile}.xaml", obj2.Xamls[temp]);
            }
            App.ClearContrast();
        }

        private async void BuildWeb()
        {
            string temp = thisfile;
            Updata_Click(null, null);
            if (Model == null)
            {
                App.LogShow("编译", "代码对比错误");
                return;
            }
            List<CodeEditObj> list = new();
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
            obj3.Text = Text.Text;
            var data = await App.HttpUtils.BuildWeb(obj3, list, thisfile);
            if (data == null)
            {
                App.LogShow("编译", "服务器返回错误");
                return;
            }
            App.LogShow("编译", data.Message);
            obj3.Next();
            old = textEditor.Text;
            App.MainWindow_.Re(type);
            CodeSave.Save(Local + $"{thisfile}", obj3.Codes[temp]);
            App.ClearContrast();
        }

        private void Build_Click(object sender, RoutedEventArgs e)
        {
            if (Write)
                return;
            Build_Button.IsEnabled = false;
            Write = true;
            if (type is CodeType.Web)
            {
                BuildWeb();
            }
            else if (type is CodeType.App)
            {
                BuildApp();
            }
            else
            {
                BuildOther();
            }
            Build_Button.IsEnabled = true;
            Write = false;
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var data = new InputWindow("文件名").Set();
            if (string.IsNullOrWhiteSpace(data))
                return;
            ReMessage res = null;
            if (type == CodeType.App)
            {
                ReType fileType = ReType.Check;
                if (data.EndsWith(".cs"))
                {
                    data = data.Replace(".cs", "");
                    fileType = ReType.AppAddCS;
                }
                else if (data.EndsWith(".xaml"))
                {
                    data = data.Replace(".xaml", "");
                    fileType = ReType.AppAddXaml;
                }
                res = await App.HttpUtils.AddAppFile(obj2, fileType, data);
            }
            else if (type == CodeType.Web)
            {
                if (data.EndsWith(".html") || data.EndsWith(".css")
                || data.EndsWith(".js") || data.EndsWith(".json")
                || data.EndsWith(".txt"))
                {
                    res = await App.HttpUtils.AddWebCode(obj3, data);
                }
                else
                {
                    var openFileDialog1 = new System.Windows.Forms.OpenFileDialog
                    {
                        Title = "选择要添加的",
                        Filter = "文件|*.*",
                        FilterIndex = 2,
                        RestoreDirectory = true
                    };
                    if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var by = File.ReadAllBytes(openFileDialog1.FileName);
                        res = await App.HttpUtils.AddWebFile(obj3, data, BuildUtils.BytesToHexString(by));
                    }
                    else
                    {
                        return;
                    }
                }
            }
            if (res == null)
            {
                App.LogShow("添加", "服务器返回错误");
                return;
            }
            App.LogShow("添加", res.Message);
            if (res.Build)
            {
                GetCode();
            }
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (FileList.SelectedItem == null)
                return;
            string data = FileList.SelectedItem as string;
            if (thisfile == data)
                return;
            if (type == CodeType.Web)
            {
                if (data.EndsWith(".html") || data.EndsWith(".css")
                   || data.EndsWith(".js") || data.EndsWith(".json")
                   || data.EndsWith(".txt"))
                {
                    old = obj3.Codes[data];
                    textEditor.Text = old;
                }
                else
                {
                    old = textEditor.Text = "";
                }
            }
            else
            {
                if (data.EndsWith(".cs"))
                {
                    old = obj2.Codes[data.Replace(".cs", "")];
                    textEditor.Text = old;
                }
                else if (data.EndsWith(".xaml"))
                {
                    old = obj2.Xamls[data.Replace(".xaml", "")];
                    textEditor.Text = old;
                }
                else
                {
                    old = textEditor.Text = "";
                }
            }
            thisfile = data;
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FileList.SelectedItem == null)
                return;
            string data = FileList.SelectedItem as string;
            if (type == CodeType.Web)
            {
                var data1 = await App.HttpUtils.WebRemoveFile(obj3, data);
                if (data1 == null)
                {
                    App.LogShow("删除", "服务器返回错误");
                    return;
                }
                App.LogShow("删除", data1.Message);
                GetCode();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.CloseEdit(obj, type);
        }

        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Change_Click(null, null);
        }
    }
}
