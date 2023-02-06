using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using System;
using System.Windows;

namespace ColoryrBuild.Views.CodeList;

public class ClassView : CodeListView
{
    public static Action FRefresh;
    public ClassView()
    {
        type = CodeType.Class;
        MainWindow.CallRefresh += Refresh;
        FRefresh = Refresh;
    }

    public override void AddClick(object sender, RoutedEventArgs e)
    {
        var data = new InputWindow("UUID设置").Set();
        Add(type, data);
    }
}