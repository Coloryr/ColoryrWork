using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColoryrBuild.Windows;

/// <summary>
/// RouteWindow.xaml 的交互逻辑
/// </summary>
public partial class RouteWindow : Window
{
    private bool Res;
    public RouteWindow()
    {
        InitializeComponent();
    }

    private void AddClick(object sender, RoutedEventArgs e) 
    {
        string key, value;
        var res = new Input1Window("请求头设置", "键", "值").Set(out key, out value);
        if (!res)
            return;

        HeadList.Items.Add(new KVConfig()
        {
            Key = key,
            Value = value
        });
    }

    private void DeleteClick(object sender, RoutedEventArgs e)
    {
        var item = HeadList.SelectedItem as KVConfig;
        if (item == null)
            return;

        HeadList.SelectedItem = null;
        HeadList.Items.Remove(item);
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

    public bool Set(out string key, out RouteConfigObj obj) 
    {
        ShowDialog();
        obj = new()
        {
            Url = Url.Text,
            Heads = new()
        };
        key = Key.Text;
        foreach (KVConfig item in HeadList.Items)
        {
            obj.Heads.Add(item.Key, item.Value);
        }
        return Res;
    }
}
