using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Views;

/// <summary>
/// TableView2.xaml 的交互逻辑
/// </summary>
public partial class TableView2 : UserControl
{
    public static Action Action;
    public Dictionary<string, CSFileObj> ClassList;
    public TableView2()
    {
        InitializeComponent();
        MainWindow.CallRefresh += Refresh;
        Action = Refresh;
    }

    private async void Refresh()
    {
        var list = await App.HttpUtils.GetList(CodeType.Class);
        if (list == null)
        {
            App.LogShow("刷新", "Class刷新失败");
            return;
        }
        ClassList = list.List;
        ListClass.Items.Clear();
        foreach (var item in ClassList)
        {
            ListClass.Items.Add(item.Value);
        }
        App.LogShow("刷新", "Class刷新成功");
    }

    private async void AddClassClick(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("UUID设置").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;
        var list = await App.HttpUtils.Add(CodeType.Class, data);
        if (list == null)
        {
            App.LogShow("添加", "服务器返回错误");
            return;
        }
        App.LogShow("创建", list.Message);
        if (list.Build)
        {
            Refresh();
        }
    }
    private void ChangeClassClick(object sender, RoutedEventArgs e)
    {
        if (ListClass.SelectedItem == null)
            return;
        var item = ListClass.SelectedItem as CSFileObj;
        App.AddEdit(item, CodeType.Class);
    }
    private async void DeleteClassClick(object sender, RoutedEventArgs e)
    {
        if (ListClass.SelectedItem == null)
            return;
        var item = ListClass.SelectedItem as CSFileObj;
        var res = new ChoseWindow("删除确认", "是否要删除").Set();
        if (res)
        {
            var data = await App.HttpUtils.Remove(CodeType.Class, item);
            if (data == null)
            {
                App.LogShow("删除", "服务器返回错误");
                return;
            }
            App.LogShow("删除", data.Message);
            if (data.Build)
            {
                Refresh();
            }
        }
    }
    private void RefreshClassClick(object sender, RoutedEventArgs e)
    {
        Refresh();
    }
    private void ClearClassClick(object sender, RoutedEventArgs e)
    {
        InputClass.Text = "";
    }
    private void InputClassTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputClass.Text))
        {
            ListClass.Items.Clear();
            foreach (var item in ClassList)
            {
                ListClass.Items.Add(item.Value);
            }
        }
        else
        {
            ListClass.Items.Clear();
            foreach (var item in ClassList)
            {
                if (item.Value.UUID.Contains(InputClass.Text))
                {
                    ListClass.Items.Add(item.Value);
                }
            }
        }
    }
}
