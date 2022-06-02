using ColoryrWork.Lib.Build.Object;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ColoryrBuild.Views;

/// <summary>
/// TableView1.xaml 的交互逻辑
/// </summary>
public abstract partial class CodeListView : UserControl
{
    public Dictionary<string, CSFileObj> CodeList;
    public Action RefreshAction;

    public CodeListView()
    {
        InitializeComponent();
    }

    public abstract void AddClick(object sender, RoutedEventArgs e);
    public abstract void ChangeClick(object sender, RoutedEventArgs e);
    public abstract void DeleteClick(object sender, RoutedEventArgs e);

    private void RefreshClick(object sender, RoutedEventArgs e)
    {
        RefreshAction();
    }
    private void ClearClick(object sender, RoutedEventArgs e)
    {
        Input.Text = "";
    }
    private void InputTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Input.Text))
        {
            List1.Items.Clear();
            foreach (var item in CodeList)
            {
                List1.Items.Add(item.Value);
            }
        }
        else
        {
            List1.Items.Clear();
            foreach (var item in CodeList)
            {
                if (item.Value.UUID.Contains(Input.Text))
                {
                    List1.Items.Add(item.Value);
                }
            }
        }
    }
}
