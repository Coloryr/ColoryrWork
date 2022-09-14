using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Windows;

/// <summary>
/// ChoseWindow.xaml 的交互逻辑
/// </summary>
public partial class Chose1Window : Window
{
    private int Type = 1;
    private bool Res;
    public Chose1Window()
    {
        InitializeComponent();
    }

    public bool Set(out int type)
    {
        ShowDialog();
        type = Type;
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

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton item)
        {
            if (item.Name == "Type1")
            {
                Type = 1;
            }
            else if (item.Name == "Type2")
            {
                Type = 2;
            }
            else if (item.Name == "Type3")
            {
                Type = 3;
            }
            else if (item.Name == "Type4")
            {
                Type = 4;
            }
        }
    }
}
