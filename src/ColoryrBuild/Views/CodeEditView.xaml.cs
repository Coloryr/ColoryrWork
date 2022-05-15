using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit.Folding;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ColoryrBuild.Windows;

namespace ColoryrBuild.View;

/// <summary>
/// CodeEditView.xaml 的交互逻辑
/// </summary>
public partial class CodeEditView : UserControl
{
    private string old;
    private string thisfile;
    public readonly CSFileObj obj;
    public readonly CodeType type;
    private CSFileCode obj1;
    private WebObj obj3;
    private DiffPaneModel Model;
    private readonly FileSystemWatcher FileSystemWatcher;
    private readonly string Local;
    private bool Write;

    public CodeEditView(CSFileObj obj, CodeType type)
    {
        this.obj = obj;
        this.type = type;

        InitializeComponent();

        var foldingManager = FoldingManager.Install(textEditor.TextArea);
        var foldingStrategy = new XmlFoldingStrategy();
        foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
        textEditor.Options.ShowSpaces = true;
        textEditor.Options.ShowTabs = true;

        if (type is not CodeType.Web)
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

    public void Close()
    {
        FileSystemWatcher.Dispose();
    }

    private async void OnChanged(object source, FileSystemEventArgs e)
    {
        if (Write)
            return;
        Write = true;
        if(type is CodeType.Web)
        {
            obj3.Codes[e.Name] = CodeSave.Load(Local + e.Name).Replace("\r", "");
            await Dispatcher.InvokeAsync(() =>
            {
                Model = App.StartContrast(type, obj3.UUID, obj3.Codes[e.Name], old);
                textEditor.Text = obj3.Codes[e.Name];
            });
        }
        else if (type is not CodeType.Web)
        {
            if (e.Name == "main.cs")
            {
                obj1.Code = CodeSave.Load(Local + "main.cs").Replace("\r", "");
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
        if (type is not CodeType.Web)
        {
            var data = await App.HttpUtils.GetCode(type, obj.UUID);
            if (data == null)
            {
                App.LogShow("获取代码", $"代码{type}[{obj.UUID}]获取错误");
                Write = false;
                return;
            }
            else
                obj1 = data;

            obj1.Code = obj1.Code;

            textEditor.Text = obj1.Code;
            old = obj1.Code;
            Text.Text = obj1.Text;
            if (File.Exists(Local + "main.cs"))
            {
                string time = string.Format("{0:s}", DateTime.Now).Replace(":", "_");
                try
                {
                    ZIPUtils.Pack(Local, Local + "main.cs", time + ".cs");
                }
                catch
                {
                    MessageBox.Show("备份失败");
                }
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

            var newLocal = Local + $"backup/";
            if (Directory.Exists(newLocal))
            {
                Directory.Delete(newLocal, true);
            }
            Directory.CreateDirectory(newLocal);

            foreach (var item in obj3.Codes)
            {
                if (File.Exists(Local + item.Key))
                {
                    File.Move(Local + item.Key, newLocal + item.Key);
                }
                CodeSave.Save(Local + item.Key, item.Value);
                FileList.Items.Add(item.Key);
            }

            try
            {
                ZIPUtils.Pack1(Local, newLocal, $"backup_{time}");
            }
            catch
            {
                MessageBox.Show("备份失败");
            }
            Directory.Delete(newLocal, true);

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
        Write = false;
    }

    private async Task UpdateTask() 
    {
        Updata_Button.IsEnabled = false;
        if (type == CodeType.Web)
        {
            obj3.Text = Text.Text;
            if (thisfile.EndsWith(".html") || thisfile.EndsWith(".css")
            || thisfile.EndsWith(".js") || thisfile.EndsWith(".json")
            || thisfile.EndsWith(".txt"))
            {
                obj3.Codes[thisfile] = textEditor.Text.Replace("\r", "");
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
            obj1.Code = textEditor.Text.Replace("\r", "");
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
        App.MainWindow_.RefreshCode(type);
        CodeSave.Save(Local + "main.cs", obj1.Code);
        App.ClearContrast();
        IsBuild = false;
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
        App.MainWindow_.RefreshCode(type);
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
        if (type == CodeType.Web)
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

    private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Change_Click(null, null);
    }

    private void Close_Button_Click(object sender, RoutedEventArgs e)
    {
        App.CloseEdit(this);
    }
}
