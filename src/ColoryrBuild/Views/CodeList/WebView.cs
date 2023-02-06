using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Windows;

namespace ColoryrBuild.Views.CodeList;

public class WebView : CodeListView
{
    public static Action FRefresh;
    public WebView()
    {
        type = CodeType.Web;
        MainWindow.CallRefresh += Refresh;
        FRefresh = Refresh;
    }

    public override async void AddClick(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("UUID设置", "Web/").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;
        data = data.Replace('\\', '/');
        if (data.EndsWith('/'))
        {
            data = data[..^1];
        }
        var res = new ChoseWindow("选择类型", "是否是Vue项目").Set();

        var list = await App.HttpUtils.AddWeb(data, res);
        if (list == null)
        {
            InfoWindow.Show("创建", "服务器返回错误");
            return;
        }
        App.LogShow("创建", list.Message);
        if (list.Build)
        {
            Refresh();
        }
        else
        {
            InfoWindow.Show("创建", list.Message);
        }
    }
}
