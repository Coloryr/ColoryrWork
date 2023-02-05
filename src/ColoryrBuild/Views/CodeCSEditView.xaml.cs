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
    private string? FileName;
    private CSFileCode CsObj;
    private DiffPaneModel Model;
    private readonly Dictionary<string, string> FileMap = new();
    private readonly ObservableCollection<string> Files = new();
    private readonly FileSystemWatcher FileSystemWatcher;
    private readonly string Local;
    private bool IsWrite;
    private bool IsUpdate;
    private bool IsBuild;

    public CodeCSEditView(CSFileObj obj, CodeType type)
    {
        Obj = obj;
        Type = type;

        InitializeComponent();

        var foldingManager = FoldingManager.Install(TextEditor.TextArea);
        var foldingStrategy = new XmlFoldingStrategy();
        foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
        TextEditor.Options.ShowSpaces = true;
        TextEditor.Options.ShowTabs = true;

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
            FileSystemWatcher.Changed += OnChanged;
            FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            FileSystemWatcher.EnableRaisingEvents = true;
        }
        catch (Exception e)
        {
            InfoWindow.Show("初始化错误", e.ToString());
        }
    }

    public void Close()
    {
        FileSystemWatcher.Dispose();
    }

    public async void GetCode()
    {
        if (IsWrite)
            return;
        IsWrite = true;
        Logs.Text = "";
        string time = string.Format("{0:s}", DateTime.Now).Replace(":", "_");
        if (Type == CodeType.Class)
        {
            var res = await App.HttpUtils.GetClassCode(Obj);
            if (res == null)
            {
                AddLog("获取代码错误");
                IsWrite = false;
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
                Files.Add(item.name);
                FileMap.Add(item.name, item.code);

                string name = item.name + ".cs";

                if (File.Exists(Local + name))
                {
                    File.Move(Local + name, newLocal + name);
                }
                CodeSave.Save(Local + name, item.code);
            }

            try
            {
                ZIPUtils.Pack1(Local, newLocal, $"backup_{time}");
            }
            catch (Exception e)
            {
                InfoWindow.Show("代码备份失败", e.ToString());
            }
            Directory.Delete(newLocal, true);
            FileName = null;
            FileList.SelectedItem = Files.First();
        }
        else
        {
            var data = await App.HttpUtils.GetCode(Type, Obj.UUID);
            if (data == null)
            {
                AddLog("获取代码错误");
                IsWrite = false;
                return;
            }
            else
                CsObj = data;

            CsObj.Code = CsObj.Code;

            TextEditor.Text = CsObj.Code;
            OldCode = CsObj.Code;
            Text.Text = CsObj.Text;
            if (File.Exists(Local + "main.cs"))
            {
                try
                {
                    ZIPUtils.Pack(Local, Local + "main.cs", time + ".cs");
                }
                catch (Exception e)
                {
                    InfoWindow.Show("备份失败", e.ToString());
                }
            }
            CodeSave.Save(Local + "main.cs", CsObj.Code);
        }
        AddLog("获取代码成功");
        IsWrite = false;
    }

    private void AddLog(string data)
    {
        Logs.AppendText($"[{DateTime.Now}]{data}");
    }

    private async void OnChanged(object source, FileSystemEventArgs e)
    {
        if (IsWrite)
            return;
        IsWrite = true;
        if (Type == CodeType.Class)
        {
            var name = e.Name?.Replace(".cs", "");
            if (name == null)
            {
                IsWrite = false;
                return;
            }
            if (name.Contains('.'))
            {
                IsWrite = false;
                return;
            }
            if (!FileMap.ContainsKey(name))
            {
                IsWrite = false;
                return;
            }
            CsObj.Code = CodeSave.Load(Local + e.Name).Replace("\r", "");
            await Dispatcher.Invoke(async () =>
            {
                FileList.SelectedItem = name;
                Model = App.StartContrast(CsObj, OldCode);
                TextEditor.Text = CsObj.Code;
                await UpdateTask();
            });
        }
        else if (e.Name == "main.cs")
        {
            CsObj.Code = CodeSave.Load(Local + "main.cs").Replace("\r", "");
            await Dispatcher.InvokeAsync(() =>
            {
                Model = App.StartContrast(CsObj, OldCode);
                TextEditor.Text = CsObj.Code;
            });
        }
        IsWrite = false;
    }

    private async Task UpdateTask()
    {
        if (IsUpdate)
            return;
        IsUpdate = true;
        Logs.Text = "";
        Updata_Button.IsEnabled = false;
        CsObj.Text = Text.Text;
        CsObj.Code = TextEditor.Text.Replace("\r", "");
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
            if (FileName == null)
                return;

            var res = App.HttpUtils.ClassFileEdit(CsObj, FileName, list);
            if (res == null)
            {
                AddLog("代码上传错误");
                Updata_Button.IsEnabled = true;
                IsUpdate = false;
                return;
            }

            CsObj.Next();
            App.WindowMain.RefreshCode(Type);
            if (!IsWrite)
            {
                IsWrite = true;
                CodeSave.Save(Local + FileName + ".cs", CsObj.Code);
                IsWrite = false;
            }
            FileMap[FileName] = CsObj.Code;
            CsObj.Code = null;
            AddLog("代码上传成功");
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

    private async void Build()
    {
        if (IsBuild)
            return;
        Logs.Text = "";
        IsBuild = true;
        ReMessage? data;
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
            AddLog("服务器返回错误");
            return;
        }
        AddLog($"结果:{data.Message}\n" +
            $"用时:{data.UseTime}\n" +
            $"编译时间:{data.Time}");
        CsObj.Next();
        App.WindowMain.RefreshCode(Type);
        IsWrite = true;
        CodeSave.Save(Local + "main.cs", CsObj.Code);
        IsWrite = false;
        IsBuild = false;
    }

    private void Build_Click(object sender, RoutedEventArgs e)
    {
        if (IsWrite)
            return;
        Build_Button.IsEnabled = false;
        IsWrite = true;
        Build();
        Build_Button.IsEnabled = true;
        IsWrite = false;
    }

    private async void Add_Click(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("文件名").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;
        if (data.Contains('\\') || data.Contains('/'))
        {
            InfoWindow.Show("名字非法", "不支持子路径");
            return;
        }
        Logs.Text = "";
        var res = await App.HttpUtils.AddClassFile(CsObj, data);
        if (res == null)
        {
            AddLog("服务器返回错误");
            return;
        }
        AddLog(res.Message);
        if (res.Build)
        {
            GetCode();
        }
    }
    private void Change_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem is not string data)
            return;

        if (FileName == data)
            return;
        if (Type == CodeType.Class)
        {
            OldCode = FileMap[data];
            TextEditor.Text = OldCode;
        }
        FileName = data;
    }
    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem is not string data)
            return;

        Logs.Text = "";
        var res = await App.HttpUtils.RemoveClassFile(CsObj, data);
        if (res == null)
        {
            AddLog("服务器返回错误");
            return;
        }
        AddLog(res.Message);
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

    private async void TextEditor_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control) && e.Key == Key.S)
        {
            await UpdateTask();
        }
    }
}
