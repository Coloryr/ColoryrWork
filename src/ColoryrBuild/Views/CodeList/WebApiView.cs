using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Windows;

namespace ColoryrBuild.Views.CodeList;

public class WebApiView : CodeListView
{
    public static Action FRefresh;
    public WebApiView()
    {
        type = CodeType.Dll;
        MainWindow.CallRefresh += Refresh;
        FRefresh = Refresh;
    }

    public override void AddClick(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("UUID设置", "WebApi/").Set();
        if (string.IsNullOrWhiteSpace(data))
            return;
        data = data.Replace('\\', '/');
        if (data.EndsWith("/"))
        {
            data = data[..^1];
        }
        Add(type, data);
    }
}
