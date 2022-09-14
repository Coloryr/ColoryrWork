using System;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Views;

/// <summary>
/// LogView.xaml 的交互逻辑
/// </summary>
public partial class LogView : UserControl
{
    public static Action<string> AddLog;
    public LogView()
    {
        InitializeComponent();

        AddLog = FAddLog;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Text.Text = "";
    }

    public void FAddLog(string text)
    {
        Dispatcher.Invoke(() =>
        {
            Text.AppendText(text + Environment.NewLine);
        });
    }
}
