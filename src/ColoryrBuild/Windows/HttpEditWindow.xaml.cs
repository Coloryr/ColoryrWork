using System.Windows;

namespace ColoryrBuild.Windows;

/// <summary>
/// HttpEditWindow.xaml 的交互逻辑
/// </summary>
public partial class HttpEditWindow : Window
{
    public string IP { get; set; }
    public int Port { get; set; }

    private bool Res;
    public HttpEditWindow(string ip, int port)
    {
        IP = ip;
        Port = port;
        InitializeComponent();

        DataContext = this;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Res = true;
        Close();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        Res = false;
        Close();
    }

    public bool Set(out string ip, out int port)
    {
        ShowDialog();
        ip = IP;
        port = Port;
        return Res;
    }
}