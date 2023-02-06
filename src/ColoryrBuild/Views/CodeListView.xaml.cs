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
public abstract partial class CodeListView : UserControl
{
    public Dictionary<string, CSFileObj> CodeList;
    protected CodeType type;

    public CodeListView()
    {
        InitializeComponent();
    }

    public abstract void AddClick(object sender, RoutedEventArgs e);
    public void ChangeClick(object sender, RoutedEventArgs e)
    {
        if (List1.SelectedItem is not CSFileObj item)
            return;

        App.AddEdit(item, type);
    }
    public async void DeleteClick(object sender, RoutedEventArgs e)
    {
        if (List1.SelectedItem is not CSFileObj item)
            return;

        if (new ChoseWindow("删除确认", "是否要删除").Set())
        {
            var res = await App.HttpUtils.RemoveObj(type, item);
            if (res == null)
            {
                App.LogShow("删除", "服务器返回错误");
                return;
            }
            App.LogShow("删除", res.Message);
            if (res.Build)
            {
                Refresh();
            }
            else
            {
                InfoWindow.Show("删除", res.Message);
            }
        }
    }

    private void RefreshClick(object sender, RoutedEventArgs e)
    {
        Refresh();
    }
    private void ClearClick(object sender, RoutedEventArgs e)
    {
        Input.Text = "";
    }
    private void InputTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Input.Text))
        {
            List1.Items.Clear();
            foreach (var item in CodeList)
            {
                List1.Items.Add(item.Value);
            }
        }
        else
        {
            List1.Items.Clear();
            foreach (var item in CodeList)
            {
                if (item.Value.UUID.Contains(Input.Text))
                {
                    List1.Items.Add(item.Value);
                }
            }
        }
    }

    /// <summary>
    /// 刷新工程
    /// </summary>
    public async void Refresh()
    {
        var list = await App.HttpUtils.GetList(type);
        if (list == null)
        {
            App.LogShow("刷新", $"{type}刷新失败");
            return;
        }
        CodeList = list.List;
        List1.Items.Clear();
        foreach (var item in CodeList)
        {
            List1.Items.Add(item.Value);
        }
        App.LogShow("刷新", $"{type}刷新成功");
    }

    /// <summary>
    /// 新建工程
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">名字</param>
    public async void Add(CodeType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;
        var res = await App.HttpUtils.AddObj(type, name);
        if (res == null)
        {
            App.LogShow("创建", "服务器返回错误");
            return;
        }
        App.LogShow("创建", res.Message);
        if (res.Build)
        {
            Refresh();
        }
        else
        {
            InfoWindow.Show("创建", res.Message);
        }
    }
}
