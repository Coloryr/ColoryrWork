using System;
using System.Collections.Generic;
using System.IO;
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
/// ImageWindow.xaml 的交互逻辑
/// </summary>
public partial class ImageWindow : Window
{
    public ImageWindow(byte[] data)
    {
        InitializeComponent();
        using MemoryStream stream = new(data);
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = stream;
        bitmapImage.EndInit();
        Img.Source = bitmapImage;
        Width = bitmapImage.Width;
        Height = bitmapImage.Height;
        Show();
        Activate();
    }
}
