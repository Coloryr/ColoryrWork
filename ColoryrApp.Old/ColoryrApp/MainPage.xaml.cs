using Xamarin.Forms;

namespace ColoryrApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage(string message)
        {
            InitializeComponent();
            Label.Text = message;
        }
    }
}
