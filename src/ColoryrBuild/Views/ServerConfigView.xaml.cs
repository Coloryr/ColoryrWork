using ColoryrBuild.Windows;
using ColoryrWork.Lib.Build.Object;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ColoryrBuild.PostBuild;

namespace ColoryrBuild.Views;

/// <summary>
/// ServerConfigView.xaml 的交互逻辑
/// </summary>
public partial class ServerConfigView : UserControl
{
    
    public ServerConfigView()
    {
        InitializeComponent();
    }

    public void GetHttpList() 
    {
        
    }

    private void Add_Click(object sender, RoutedEventArgs e) 
    {
        string ip;
        int port;
        var res = new HttpEditWindow("127.0.0.1", 80).Set(out ip, out port);
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var item = HttpList.SelectedItem as HttpObj;
        if (item == null)
            return;


    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {

    }
}