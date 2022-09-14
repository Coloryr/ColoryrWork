using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Windows;

namespace ColoryrBuild.Views.CodeList;

internal class SocketView : CodeListView
{
    private const CodeType Type = CodeType.Socket;
    public static Action Refresh;
    public SocketView()
    {
        MainWindow.CallRefresh += FRefresh;
        RefreshAction = Refresh = FRefresh;
    }

    private async void FRefresh()
    {
        var list = await App.HttpUtils.GetList(Type);
        if (list == null)
        {
            App.LogShow("刷新", $"{Type}刷新失败");
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
        var data = new InputWindow("UUID设置").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;
        var res = new ChoseWindow("选择类型", "是否是Netty项目").Set();

        var list = await App.HttpUtils.AddObj(Type, data, res.ToString());
        if (list == null)
        {
            App.LogShow("创建", "服务器返回错误");
            return;
        }
        App.LogShow("创建", list.Message);
        if (list.Build)
        {
            FRefresh();
        }
        else
        {
            InfoWindow.Show("创建", list.Message);
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
        if (new ChoseWindow("删除确认", "是否要删除").Set())
        {
            var res = await App.HttpUtils.RemoveObj(Type, item);
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
}