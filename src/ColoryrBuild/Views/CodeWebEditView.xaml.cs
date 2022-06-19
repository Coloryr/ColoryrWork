using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Views;

/// <summary>
/// CodeWebEditView.xaml 的交互逻辑
/// </summary>
public partial class CodeWebEditView : UserControl, IEditView
{
    public CSFileObj Obj { get; }
    public CodeType Type { get; } = CodeType.Web;

    private string OldCode;
    private string FileName;
    private WebObj WebObj;
    private DiffPaneModel Model;
    private readonly FileSystemWatcher FileSystemWatcher;
    private ObservableCollection<string> Files = new();
    private readonly string Local;
    private bool IsWrite;
    private bool IsBuild = false;

    public CodeWebEditView(CSFileObj obj)
    {
        Obj = obj;

        InitializeComponent();

        var foldingManager = FoldingManager.Install(TextEditor.TextArea);
        var foldingStrategy = new XmlFoldingStrategy();
        foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
        TextEditor.Options.ShowSpaces = true;
        TextEditor.Options.ShowTabs = true;

        Local = $"{CodeSave.FilePath}/{Type}/{obj.UUID}/";
        if (!Directory.Exists(Local))
        {
            Directory.CreateDirectory(Local);
        }
        GetCode();
        FileList.ItemsSource = Files;
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
            _ = new InfoWindow("初始化错误", e.ToString());
        }
    }

    public void Close()
    {
        FileSystemWatcher.Dispose();
    }

    private async void OnChanged(object source, FileSystemEventArgs e)
    {
        if (IsWrite)
            return;
        IsWrite = true;
        WebObj.Codes[e.Name] = CodeSave.Load(Local + e.Name).Replace("\r", "");
        await Dispatcher.InvokeAsync(() =>
        {
            Model = App.StartContrast(Type, WebObj.UUID, WebObj.Codes[e.Name], OldCode);
            TextEditor.Text = WebObj.Codes[e.Name];
        });
        IsWrite = false;
    }

    private void AddLog(string data)
    {
        Logs.AppendText($"[{DateTime.Now}]{data}");
    }

    public async void GetCode()
    {
        if (IsWrite)
            return;
        IsWrite = true;
        Logs.Text = "";
        var data = await App.HttpUtils.GetWebCode(Obj.UUID);
        if (data == null)
        {
            AddLog($"代码Web[{Obj.UUID}]获取错误");
            IsWrite = false;
            return;
        }
        else
            WebObj = data;

        Files.Clear();
        string time = string.Format("{0:s}", DateTime.Now).Replace(":", "_");

        var newLocal = Local + $"backup/";
        if (Directory.Exists(newLocal))
        {
            Directory.Delete(newLocal, true);
        }
        Directory.CreateDirectory(newLocal);

        foreach (var item in WebObj.Codes)
        {
            string local = newLocal + item.Key;
            FileInfo info = new(local);
            if (!Directory.Exists(info.DirectoryName))
            {
                Directory.CreateDirectory(info.DirectoryName);
            }
            if (File.Exists(Local + item.Key))
            {
                File.Move(Local + item.Key, local);
            }
            CodeSave.Save(Local + item.Key, item.Value);
            Files.Add(item.Key);
        }

        try
        {
            ZIPUtils.Pack1(Local, newLocal, $"backup_{time}");
        }
        catch (Exception e)
        {
            _ = new InfoWindow("代码备份失败", e.ToString());
        }
        Directory.Delete(newLocal, true);

        Build_Button.IsEnabled = WebObj.IsVue;

        foreach (var item in WebObj.Files)
        {
            Files.Add(item.Key);
        }
        FileList.SelectedItem = Files.First();
        IsVue.IsChecked = WebObj.IsVue;
        AddLog($"代码Web[{Obj.UUID}]获取成功");
        IsWrite = false;
    }

    private async Task UpdateTask()
    {
        Updata_Button.IsEnabled = false;
        Logs.Text = "";
        WebObj.Text = Text.Text;
        if (FileName.EndsWith(".html") || FileName.EndsWith(".css")
        || FileName.EndsWith(".js") || FileName.EndsWith(".json")
        || FileName.EndsWith(".txt"))
        {
            WebObj.Codes[FileName] = TextEditor.Text.Replace("\r", "");
            Model = App.StartContrast(Type, WebObj.UUID, WebObj.Codes[FileName], OldCode);
            if (Model == null)
            {
                AddLog("代码对比错误");
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
            var res = await App.HttpUtils.WebFileEdit(WebObj, FileName, list);
            if (res == null)
            {
                AddLog("文件更新错误");
            }
            else
            {
                AddLog("文件更新完成");
                App.MainWindow_.RefreshCode(Type);
                WebObj.Up();
            }
            App.ClearContrast();
            Dispatcher.Invoke(() => MainWindow.SwitchTo(this));
        }
        else
        {
            AddLog("不支持修改该文件");
        }
        Updata_Button.IsEnabled = true;
    }

    private async void Updata_Click(object sender, RoutedEventArgs e)
    {
        await UpdateTask();
    }

    private async void Image_Click(object sender, RoutedEventArgs e) 
    {
        if (FileList.SelectedItem == null)
            return;

        Logs.Text = "";
        string data = FileList.SelectedItem as string;
        var data1 = await App.HttpUtils.WebDownloadFile(WebObj, data);
        if (data1 == null)
        {
            AddLog("服务器返回错误");
            return;
        }

        if (!data1.Build)
        {
            AddLog(data1.Message);
            return;
        }

        byte[] temp = Convert.FromBase64String(data1.Message);

        _ = new ImageWindow(temp);
    }

    private void ReCode_Click(object sender, RoutedEventArgs e)
    {
        ReCode_Button.IsEnabled = false;
        GetCode();
        ReCode_Button.IsEnabled = true;
    }

    private async void BuildWeb()
    {
        Logs.Text = "";
        string temp = FileName;
        WebObj.Text = Text.Text;
        var data = await App.HttpUtils.BuildWeb(WebObj, FileName);
        if (data == null)
        {
            AddLog("服务器返回错误");
            return;
        }
        AddLog(data.Message);
        WebObj.Next();
        OldCode = TextEditor.Text;
        App.MainWindow_.RefreshCode(Type);
        CodeSave.Save(Local + $"{FileName}", WebObj.Codes[temp]);
        App.ClearContrast();
        await Dispatcher.BeginInvoke(() => MainWindow.SwitchTo(this));
    }

    private void Build_Click(object sender, RoutedEventArgs e)
    {
        if (IsWrite)
            return;
        Build_Button.IsEnabled = false;
        IsWrite = true;
        BuildWeb();
        Build_Button.IsEnabled = true;
        IsWrite = false;
    }

    private async void Add_Click(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("文件名").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;

        Logs.Text = "";
        ReMessage res;
        if (data.EndsWith(".html") || data.EndsWith(".css")
            || data.EndsWith(".js") || data.EndsWith(".json")
            || data.EndsWith(".txt") || data.EndsWith(".vue")
            || data.EndsWith(".ts"))
        {
            res = await App.HttpUtils.AddWebCode(WebObj, data);
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
                res = await App.HttpUtils.AddWebFile(WebObj, data, Convert.ToBase64String(by));
            }
            else
            {
                return;
            }
        }
        if (res == null)
        {
            AddLog("服务器返回错误");
            return;
        }
        AddLog(res.Message);
        if (res.Build)
        {
            GetCode();
            FileName = data;
            Change_Click(null, null);
        }
    }
    private void Change_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null)
            return;
        string data = FileList.SelectedItem as string;
        if (FileName == data)
            return;

        if (WebObj.Codes.ContainsKey(data))
        {
            OldCode = WebObj.Codes[data];
            TextEditor.Text = OldCode;
            TextEditor.IsReadOnly = false;
        }
        else
        {
            TextEditor.Text = "无法编辑文件";
            TextEditor.IsReadOnly = true;
        }

        if (data.EndsWith(".js"))
        {

        }

        FileName = data;
    }
    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null)
            return;

        Logs.Text = "";
        string data = FileList.SelectedItem as string;
        var data1 = await App.HttpUtils.WebDownloadFile(WebObj, data);
        if (data1 == null)
        {
            AddLog("服务器返回错误");
            return;
        }

        if (!data1.Build)
        {
            AddLog(data1.Message);
            return;
        }

        byte[] temp = Convert.FromBase64String(data1.Message);
        System.Windows.Forms.OpenFileDialog openFileDialog = new()
        {
            CheckFileExists = false,
            FileName = data
        };

        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            string path = openFileDialog.FileName;
            if (File.Exists(path))
            {
                if (!new ChoseWindow("文件存在", "文件存在，是否要覆盖源文件").Set())
                {
                    return;
                }
            }

            File.WriteAllBytes(path, temp);
            AddLog("已下载文件");
        }
    }
    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null)
            return;

        Logs.Text = "";
        string data = FileList.SelectedItem as string;
        var data1 = await App.HttpUtils.WebRemoveFile(WebObj, data);
        if (data1 == null)
        {
            AddLog("服务器返回错误");
            return;
        }
        AddLog(data1.Message);
        GetCode();
    }

    private async void ZIP_Click(object sender, RoutedEventArgs e)
    {
        Logs.Text = "";
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
            var res = await App.HttpUtils.WebCodeZIP(WebObj, file);
            if (res == null)
            {
                AddLog($"Web{WebObj.UUID}代码压缩包上传失败");
            }
            else if (!res.Build)
            {
                AddLog($"Web{WebObj.UUID}代码压缩包上传失败：{res.Message}");
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
        Logs.Text = "";
        var set = !WebObj.IsVue;
        IsVue.IsEnabled = false;
        var res = await App.HttpUtils.SetIsVue(WebObj, set);
        if (res == null)
        {
            AddLog($"设置Web{WebObj.UUID}的Vue模式失败");
            IsVue.IsEnabled = true;
            return;
        }
        else if (!res.Build)
        {
            AddLog($"设置Web{WebObj.UUID}的Vue模式失败：{res.Message}");
            IsVue.IsEnabled = true;
            return;
        }
        WebObj.IsVue = set;
        Dispatcher.Invoke(() => IsVue.IsChecked = WebObj.IsVue);
        AddLog($"设置Web{WebObj.UUID}的Vue模式完成");
        IsVue.IsEnabled = true;
    }
}
