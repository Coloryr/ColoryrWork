using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Windows;

namespace ColoryrBuild.Views.CodeList;

public class WebSocketView : CodeListView
{
    public static Action FRefresh;
    public WebSocketView()
    {
        type = CodeType.WebSocket;
        MainWindow.CallRefresh += Refresh;
        FRefresh = Refresh;
    }

    public override void AddClick(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("UUID设置").Set();
        Add(type, data);
    }
}
