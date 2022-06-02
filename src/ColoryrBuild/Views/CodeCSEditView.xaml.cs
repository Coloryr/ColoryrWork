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
/// CodeCSEditView.xaml 的交互逻辑
/// </summary>
public partial class CodeCSEditView : UserControl, IEditView
{
    private string old;
    private string thisfile;
    public CSFileObj obj { get; }
    public CodeType type { get; }
    private CSFileCode obj1;
    private DiffPaneModel Model;
    private Dictionary<string, string> Files = new();
    private ObservableCollection<string> file = new();
    private readonly FileSystemWatcher FileSystemWatcher;
    private readonly string Local;
    private bool Write;

    public CodeCSEditView(CSFileObj obj, CodeType type)
    {
        this.obj = obj;
        this.type = type;

        InitializeComponent();

        var foldingManager = FoldingManager.Install(textEditor.TextArea);
        var foldingStrategy = new XmlFoldingStrategy();
        foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
        textEditor.Options.ShowSpaces = true;
        textEditor.Options.ShowTabs = true;

        if (type is not CodeType.Class)
            CodeA.IsEnabled = false;
        Local = $"{CodeSave.FilePath}/{type}/{obj.UUID}/";
        if (!Directory.Exists(Local))
        {
            Directory.CreateDirectory(Local);
        }
        GetCode();
        FileList.ItemsSource = file;
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
        if (e.Name == "main.cs")
        {
            obj1.Code = CodeSave.Load(Local + "main.cs").Replace("\r", "");
            await Dispatcher.InvokeAsync(() =>
            {
                Model = App.StartContrast(obj1, old);
                textEditor.Text = obj1.Code;
            });
        }
        Write = false;
    }

    public async void GetCode()
    {
        if (Write)
            return;
        Write = true;
        Logs.Text = "";
        string time = string.Format("{0:s}", DateTime.Now).Replace(":", "_");
        if (type == CodeType.Class)
        {
            var res = await App.HttpUtils.GetClassCode(obj);
            if (res == null)
            {
                Logs.AppendText("获取代码错误");
                Write = false;
                return;
            }
            obj1 = res.Obj;
            file.Clear();
            Files.Clear();

            var newLocal = Local + $"backup/";
            if (Directory.Exists(newLocal))
            {
                Directory.Delete(newLocal, true);
            }
            Directory.CreateDirectory(newLocal);

            foreach (var item in res.List)
            {
                file.Add(item.File);
                Files.Add(item.File, item.Code);

                string name = item.File + ".cs";

                if (File.Exists(Local + name))
                {
                    File.Move(Local + name, newLocal + name);
                }
                CodeSave.Save(Local + name, item.Code);
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

            thisfile = file.First();
            old = textEditor.Text = Files[thisfile];

        }
        else
        {
            var data = await App.HttpUtils.GetCode(type, obj.UUID);
            if (data == null)
            {
                Logs.AppendText("获取代码错误");
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
        }
        Logs.AppendText("获取代码成功");
        Write = false;
    }

    private async Task UpdateTask()
    {
        Logs.Text = "";
        Updata_Button.IsEnabled = false;
        obj1.Text = Text.Text;
        obj1.Code = textEditor.Text.Replace("\r", "");
        await Dispatcher.InvokeAsync(() =>
        {
            Model = App.StartContrast(obj1, old);
        });
        if (type == CodeType.Class)
        {
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
            var res = App.HttpUtils.ClassFileEdit(obj1, thisfile, list);
            if (res == null)
            {
                Logs.AppendText("代码上传错误");
                Updata_Button.IsEnabled = true;
                return;
            }

            obj1.Next();
            App.MainWindow_.RefreshCode(type);
            CodeSave.Save(Local + thisfile + ".cs", obj1.Code);
            obj1.Code = null;
            App.ClearContrast();
            Dispatcher.Invoke(() => MainWindow.SwitchTo(this));
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
        Logs.Text = "";
        IsBuild = true;
        ReMessage data;
        if (type != CodeType.Class)
        {
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
            data = await App.HttpUtils.Build(obj1, type, list);
        }
        else
        {
            data = await App.HttpUtils.Build(obj1, type, null);
        }

        if (data == null)
        {
            Logs.AppendText("服务器返回错误");
            return;
        }
        Logs.AppendText($"结果:{data.Message}\n" +
            $"用时:{data.UseTime}\n" +
            $"最后更新时间:{data.Time}");
        obj1.Next();
        App.MainWindow_.RefreshCode(type);
        CodeSave.Save(Local + "main.cs", obj1.Code);
        App.ClearContrast();
        IsBuild = false;
        await Dispatcher.BeginInvoke(() => MainWindow.SwitchTo(this));
    }

    private void Build_Click(object sender, RoutedEventArgs e)
    {
        if (Write)
            return;
        Build_Button.IsEnabled = false;
        Write = true;
        BuildOther();
        Build_Button.IsEnabled = true;
        Write = false;
    }

    private async void Add_Click(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("文件名").Set();
        Logs.Text = "";
        if (string.IsNullOrWhiteSpace(data))
            return;
        var res = await App.HttpUtils.AddClassFile(obj1, data);
        if (res == null)
        {
            Logs.AppendText("服务器返回错误");
            return;
        }
        Logs.AppendText(res.Message);
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
        if (type == CodeType.Class)
        {
            old = Files[data];
            textEditor.Text = old;
        }
        thisfile = data;
    }
    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null)
            return;
        Logs.Text = "";
        string data = FileList.SelectedItem as string;
        var res = await App.HttpUtils.RemoveClassFile(obj1, data);
        if (res == null)
        {
            Logs.AppendText("服务器返回错误");
            return;
        }
        Logs.AppendText(res.Message);
        if (res.Build)
        {
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
