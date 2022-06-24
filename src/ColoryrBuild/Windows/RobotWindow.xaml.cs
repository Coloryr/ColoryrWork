using System;
using System.Collections;
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
/// RobotWindow.xaml 的交互逻辑
/// </summary>
public partial class RobotWindow : Window
{
    private bool Res;
    public RobotWindow(List<int> list)
    {
        InitializeComponent();
        foreach (var item in RobotConfigSet.PackType)
        {
            if (!list.Contains(item.Key))
            {
                Selects.Items.Add(item);
            }
        }
    }

    public bool Set(out int index)
    {
        ShowDialog();
        if (Selects.SelectedItem != null)
        {
            index = ((KeyValuePair<int, string>)Selects.SelectedItem).Key;
        }
        else
        {
            Res = false;
            index = 0;
        }
        return Res;
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
}
