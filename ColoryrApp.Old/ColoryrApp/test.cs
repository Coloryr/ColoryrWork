using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColoryrApp
{
    class test
    {
        public ContentPage Page { get; private set; }
        private Button Button;
        private Label Label;
        private Entry Entry;
        public void Init()
        {
            string pageXAML = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><ContentPage xmlns=\"http://xamarin.com/schemas/2014/forms\" xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\" x:Class=\"App2.MainPage\"><StackLayout HorizontalOptions=\"Center\"><Label Text=\"Hello World\" FontSize=\"Title\" Padding=\"30,10,30,10\" HorizontalOptions=\"Center\" HorizontalTextAlignment=\"Center\" x:Name=\"Label\" WidthRequest=\"500\"/><Button Text=\"Load\" x:Name=\"Button\"/><Entry Placeholder=\"Enter text\" x:Name=\"Entry\"/></StackLayout></ContentPage>";
            Page = new ContentPage().LoadFromXaml(pageXAML);
            Button = Page.FindByName<Button>("Button");
            Label = Page.FindByName<Label>("Label");
            Entry = Page.FindByName<Entry>("Entry");

            Button.Clicked += ButtonClick;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Label.Text = Entry.Text;
        }

        public void OnStart()
        {

        }

        public void OnSleep()
        {

        }

        public void OnResume()
        {

        }
    }
}
