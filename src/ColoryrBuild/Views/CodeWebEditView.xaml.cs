using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Views;

/// <summary>
/// CodeWebEditView.xaml 的交互逻辑
/// </summary>
public partial class CodeWebEditView : UserControl, IEditView
{
    private string old;
    private string thisfile;
    public CSFileObj obj { get; }
    public CodeType type { get; }
    private WebObj obj1;
    private DiffPaneModel Model;
    private readonly FileSystemWatcher FileSystemWatcher;
    private readonly string Local;
    private bool Write;

    public CodeWebEditView(CSFileObj obj, CodeType type)
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
        obj1.Codes[e.Name] = CodeSave.Load(Local + e.Name).Replace("\r", "");
        await Dispatcher.InvokeAsync(() =>
        {
            Model = App.StartContrast(type, obj1.UUID, obj1.Codes[e.Name], old);
            textEditor.Text = obj1.Codes[e.Name];
        });
        Write = false;
    }

    public async void GetCode()
    {
        if (Write)
            return;
        Write = true;

        var data = await App.HttpUtils.GetWebCode(obj.UUID);
        if (data == null)
        {
            App.LogShow("获取代码", $"代码Web[{obj.UUID}]获取错误");
            Write = false;
            return;
        }
        else
            obj1 = data;

        FileList.Items.Clear();
        string time = string.Format("{0:s}", DateTime.Now).Replace(":", "_");

        var newLocal = Local + $"backup/";
        if (Directory.Exists(newLocal))
        {
            Directory.Delete(newLocal, true);
        }
        Directory.CreateDirectory(newLocal);

        foreach (var item in obj1.Codes)
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

        foreach (var item in obj1.Files)
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
        old = textEditor.Text = obj1.Codes["index.html"];
        App.LogShow("获取代码", $"代码Web[{obj.UUID}]获取成功");
        Write = false;
    }

    private async Task UpdateTask()
    {
        Updata_Button.IsEnabled = false;
        obj1.Text = Text.Text;
        if (thisfile.EndsWith(".html") || thisfile.EndsWith(".css")
        || thisfile.EndsWith(".js") || thisfile.EndsWith(".json")
        || thisfile.EndsWith(".txt"))
        {
            obj1.Codes[thisfile] = textEditor.Text.Replace("\r", "");
            Model = App.StartContrast(type, obj1.UUID, obj1.Codes[thisfile], old);

        }
        else
        {
            App.LogShow("更新错误", "不支持修改该文件");
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

    private async void BuildWeb()
    {
        string temp = thisfile;
        Updata_Click(null, null);
        if (Model == null)
        {
            Logs.AppendText("代码对比错误");
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
        obj1.Text = Text.Text;
        var data = await App.HttpUtils.BuildWeb(obj1, list, thisfile);
        if (data == null)
        {
            App.LogShow("编译", "服务器返回错误");
            return;
        }
        App.LogShow("编译", data.Message);
        obj1.Next();
        old = textEditor.Text;
        App.MainWindow_.RefreshCode(type);
        CodeSave.Save(Local + $"{thisfile}", obj1.Codes[temp]);
        App.ClearContrast();
        await Dispatcher.BeginInvoke(() => MainWindow.SwitchTo(this));
    }

    private void Build_Click(object sender, RoutedEventArgs e)
    {
        if (Write)
            return;
        Build_Button.IsEnabled = false;
        Write = true;
        BuildWeb();
        Build_Button.IsEnabled = true;
        Write = false;
    }

    private async void Add_Click(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("文件名").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;
        ReMessage res = null;
        if (data.EndsWith(".html") || data.EndsWith(".css")
            || data.EndsWith(".js") || data.EndsWith(".json")
            || data.EndsWith(".txt"))
        {
            res = await App.HttpUtils.AddWebCode(obj1, data);
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
                res = await App.HttpUtils.AddWebFile(obj1, data, BuildUtils.BytesToHexString(by));
            }
            else
            {
                return;
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
                old = obj1.Codes[data];
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
            var data1 = await App.HttpUtils.WebRemoveFile(obj1, data);
            if (data1 == null)
            {
                App.LogShow("删除", "服务器返回错误");
                return;
            }
            App.LogShow("删除", data1.Message);
            GetCode();
        }
    }

    private async void ZIP_Click(object sender, RoutedEventArgs e)
    {
        var select = new ChoseWindow("上传压缩包", "上传代码压缩包会覆盖之前所有的文件，你确定要继续吗");
        var openFileDialog1 = new System.Windows.Forms.OpenFileDialog
        {
            Title = "选择要上传的压缩包",
            Filter = "压缩包|zip.*",
            RestoreDirectory = true
        };
        if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            var by = File.ReadAllBytes(openFileDialog1.FileName);
            var file = Convert.ToBase64String(by);
            var res = await App.HttpUtils.WebCodeZIP(obj1, file);
            if (res == null)
            {
                App.LogShow("代码压缩包", $"Web{obj1.UUID}代码压缩包上传失败");
            }
            else if (!res.Build)
            {
                App.LogShow("代码压缩包", $"Web{obj1.UUID}代码压缩包上传失败：{res.Message}");
            }
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

    private async void IsVue_Click(object sender, RoutedEventArgs e)
    {
        var set = !obj1.IsVue;
        IsVue.IsEnabled = false;
        var res = await App.HttpUtils.SetIsVue(obj1, set);
        if (res == null)
        {
            App.LogShow("设置Vue模式", $"设置Web{obj1.UUID}的Vue模式失败");
            IsVue.IsEnabled = true;
            return;
        }
        else if (!res.Build)
        {
            App.LogShow("设置Vue模式", $"设置Web{obj1.UUID}的Vue模式失败：{res.Message}");
            IsVue.IsEnabled = true;
            return;
        }
        obj1.IsVue = set;
        Dispatcher.Invoke(() => IsVue.IsChecked = obj1.IsVue);
        App.LogShow("设置Vue模式", $"设置Web{obj1.UUID}的Vue模式完成");
        IsVue.IsEnabled = true;
    }
}
