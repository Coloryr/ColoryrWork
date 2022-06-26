﻿using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Windows;

namespace ColoryrBuild.Views.CodeList;

public class WebApiView : CodeListView
{
    private const CodeType Type = CodeType.Dll;
    public static Action Refresh;
    public WebApiView()
    {
        MainWindow.CallRefresh += FRefresh;
        RefreshAction = Refresh = FRefresh;
    }

    private async void FRefresh()
    {
        var list = await App.HttpUtils.GetList(Type);
        if (list == null)
        {
            new InfoWindow("刷新", $"{Type}刷新失败");
            return;
        }
        CodeList = list.List;
        List1.Items.Clear();
        foreach (var item in CodeList)
        {
            List1.Items.Add(item.Value);
        }
        App.LogShow("刷新", $"{Type}刷新成功");
    }

    public override async void AddClick(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("UUID设置", "WebApi/").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;
        data = data.Replace('\\', '/');
        if (data.EndsWith("/"))
        {
            data = data[..^1];
        }
        var list = await App.HttpUtils.AddObj(Type, data);
        if (list == null)
        {
            new InfoWindow("添加", "服务器返回错误");
            return;
        }
        else if (!list.Build)
        {
            new InfoWindow("创建失败", list.Message);
            return;
        }
        App.LogShow("创建", list.Message);
        if (list.Build)
        {
            FRefresh();
        }
    }

    public override void ChangeClick(object sender, RoutedEventArgs e)
    {
        if (List1.SelectedItem == null)
            return;
        var item = List1.SelectedItem as CSFileObj;
        App.AddEdit(item, Type);
    }

    public override async void DeleteClick(object sender, RoutedEventArgs e)
    {
        if (List1.SelectedItem == null)
            return;
        var item = List1.SelectedItem as CSFileObj;
        var res = new ChoseWindow("删除确认", "是否要删除").Set();
        if (res)
        {
            var data = await App.HttpUtils.RemoveObj(Type, item);
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
}
