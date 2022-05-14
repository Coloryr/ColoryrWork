using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Views;

/// <summary>
/// TableView1.xaml 的交互逻辑
/// </summary>
public partial class TableView1 : UserControl
{
    public static Action Action;
    public Dictionary<string, CSFileObj> DllList;
    public TableView1()
    {
        InitializeComponent();
        MainWindow.CallRefresh += Refresh;
        Action = Refresh;
    }

    private async void Refresh()
    {
        var list = await App.HttpUtils.GetList(CodeType.Dll);
        if (list == null)
        {
            App.LogShow("刷新", "DLL刷新失败");
            return;
        }
        DllList = list.List;
        ListDll.Items.Clear();
        foreach (var item in DllList)
        {
            ListDll.Items.Add(item.Value);
        }
        App.LogShow("刷新", "DLL刷新成功");
    }
    private async void AddDllClick(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("UUID设置").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;
        var list = await App.HttpUtils.Add(CodeType.Dll, data);
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
    private void ChangeDllClick(object sender, RoutedEventArgs e)
    {
        if (ListDll.SelectedItem == null)
            return;
        var item = ListDll.SelectedItem as CSFileObj;
        App.AddEdit(item, CodeType.Dll);
    }
    private async void DeleteDllClick(object sender, RoutedEventArgs e)
    {
        if (ListDll.SelectedItem == null)
            return;
        var item = ListDll.SelectedItem as CSFileObj;
        var res = new ChoseWindow("删除确认", "是否要删除").Set();
        if (res)
        {
            var data = await App.HttpUtils.Remove(CodeType.Dll, item);
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

    private void RefreshDllClick(object sender, RoutedEventArgs e)
    {
        Refresh();
    }
    private void ClearDllClick(object sender, RoutedEventArgs e)
    {
        InputDll.Text = "";
    }
    private void InputDllTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(InputDll.Text))
        {
            ListDll.Items.Clear();
            foreach (var item in DllList)
            {
                ListDll.Items.Add(item.Value);
            }
        }
        else
        {
            ListDll.Items.Clear();
            foreach (var item in DllList)
            {
                if (item.Value.UUID.Contains(InputDll.Text))
                {
                    ListDll.Items.Add(item.Value);
                }
            }
        }
    }
}
