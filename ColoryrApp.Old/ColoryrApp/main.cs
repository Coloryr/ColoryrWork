using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ColoryrApp
{
    class test
    {
        public ContentPage Page { get; private set; }
        private string Local;

        private Button Button;
        private Label Label;
        private Entry Entry;
        public bool Init(string local)
        {
            Local = local;
            string pageXAML = "";
            var XamlFile = Path.Combine(local, "main.xaml");
            if (File.Exists(XamlFile))
            {
                pageXAML = File.ReadAllText(XamlFile);
            }
            else
            {
                return false;
            }
            Page = new ContentPage().LoadFromXaml(pageXAML);
            Button = Page.FindByName<Button>("Button");
            Label = Page.FindByName<Label>("Label");
            Entry = Page.FindByName<Entry>("Entry");

            Button.Clicked += ButtonClick;
            return true;
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
