using System.Windows;

namespace ColoryrBuild.Windows;

/// <summary>
/// InfoWindow.xaml 的交互逻辑
/// </summary>
public partial class InfoWindow : Window
{
    public static void Show(string title, string? text = null)
    {
        App.ThisApp.Dispatcher.Invoke(() =>
        {
            _ = new InfoWindow(title, text);
        });
    }
    private InfoWindow(string title, string? text)
    {
        InitializeComponent();
        Title = title;
        Text.Text = text;
        ShowDialog();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
