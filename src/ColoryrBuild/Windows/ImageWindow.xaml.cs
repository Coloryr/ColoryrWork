using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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
