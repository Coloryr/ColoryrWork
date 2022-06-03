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
using System.Windows.Input;

namespace ColoryrBuild.Views;

/// <summary>
/// CodeCSEditView.xaml 的交互逻辑
/// </summary>
public partial class CodeCSEditView : UserControl, IEditView
{
    public CSFileObj Obj { get; }
    public CodeType Type { get; }

    private string OldCode;
    private string FileName;
    private CSFileCode CsObj;
    private DiffPaneModel Model;
    private Dictionary<string, string> FileMap = new();
    private ObservableCollection<string> Files = new();
    private readonly FileSystemWatcher FileSystemWatcher;
    private readonly string Local;
    private bool Write;

    public CodeCSEditView(CSFileObj obj, CodeType type)
    {
        this.Obj = obj;
        this.Type = type;

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
            CsObj.Code = CodeSave.Load(Local + "main.cs").Replace("\r", "");
            await Dispatcher.InvokeAsync(() =>
            {
                Model = App.StartContrast(CsObj, OldCode);
                textEditor.Text = CsObj.Code;
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
        if (Type == CodeType.Class)
        {
            var res = await App.HttpUtils.GetClassCode(Obj);
            if (res == null)
            {
                Logs.AppendText("获取代码错误");
                Write = false;
                return;
            }
            CsObj = res.Obj;
            Files.Clear();
            FileMap.Clear();

            var newLocal = Local + $"backup/";
            if (Directory.Exists(newLocal))
            {
                Directory.Delete(newLocal, true);
            }
            Directory.CreateDirectory(newLocal);

            foreach (var item in res.List)
            {
                Files.Add(item.File);
                FileMap.Add(item.File, item.Code);

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

            FileName = Files.First();
            OldCode = textEditor.Text = FileMap[FileName];
        }
        else
        {
            var data = await App.HttpUtils.GetCode(Type, Obj.UUID);
            if (data == null)
            {
                Logs.AppendText("获取代码错误");
                Write = false;
                return;
            }
            else
                CsObj = data;

            CsObj.Code = CsObj.Code;

            textEditor.Text = CsObj.Code;
            OldCode = CsObj.Code;
            Text.Text = CsObj.Text;
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
            CodeSave.Save(Local + "main.cs", CsObj.Code);
        }
        Logs.AppendText("获取代码成功");
        Write = false;
    }

    private bool IsUpdate;

    private async Task UpdateTask()
    {
        if (IsUpdate)
            return;
        IsUpdate = true;
        Logs.Text = "";
        Updata_Button.IsEnabled = false;
        CsObj.Text = Text.Text;
        CsObj.Code = textEditor.Text.Replace("\r", "");
        await Dispatcher.InvokeAsync(() =>
        {
            Model = App.StartContrast(CsObj, OldCode);
        });
        if (Type == CodeType.Class)
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
            var res = App.HttpUtils.ClassFileEdit(CsObj, FileName, list);
            if (res == null)
            {
                Logs.AppendText("代码上传错误");
                Updata_Button.IsEnabled = true;
                IsUpdate = false;
                return;
            }

            CsObj.Next();
            App.MainWindow_.RefreshCode(Type);
            CodeSave.Save(Local + FileName + ".cs", CsObj.Code);
            CsObj.Code = null;
            App.ClearContrast();
            Logs.AppendText("代码上传成功");
            Dispatcher.Invoke(() => MainWindow.SwitchTo(this));
        }
        Updata_Button.IsEnabled = true;
        IsUpdate = false;
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
        if (Type != CodeType.Class)
        {
            await UpdateTask();
            OldCode = CsObj.Code;
            CsObj.Text = Text.Text;
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
            data = await App.HttpUtils.Build(CsObj, Type, list);
        }
        else
        {
            data = await App.HttpUtils.Build(CsObj, Type, null);
        }

        if (data == null)
        {
            Logs.AppendText("服务器返回错误");
            return;
        }
        Logs.AppendText($"结果:{data.Message}\n" +
            $"用时:{data.UseTime}\n" +
            $"编译时间:{data.Time}");
        CsObj.Next();
        App.MainWindow_.RefreshCode(Type);
        CodeSave.Save(Local + "main.cs", CsObj.Code);
        App.ClearContrast();
        IsBuild = false;
        Dispatcher.Invoke(() => MainWindow.SwitchTo(this));
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
        if (string.IsNullOrWhiteSpace(data))
            return;
        if (data.Contains("\\") || data.Contains("/"))
        {
            MessageBox.Show("名字非法", "不支持子路径");
            return;
        }
        Logs.Text = "";
        var res = await App.HttpUtils.AddClassFile(CsObj, data);
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
        if (FileName == data)
            return;
        if (Type == CodeType.Class)
        {
            OldCode = FileMap[data];
            textEditor.Text = OldCode;
        }
        FileName = data;
    }
    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem == null)
            return;
        Logs.Text = "";
        string data = FileList.SelectedItem as string;
        var res = await App.HttpUtils.RemoveClassFile(CsObj, data);
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

    private async void textEditor_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.S)
        {
            await UpdateTask();
        }
    }
}
