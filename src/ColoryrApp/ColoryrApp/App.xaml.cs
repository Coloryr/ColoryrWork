using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Xamarin.Forms;

namespace ColoryrApp
{
    public partial class App : Application
    {
        public const string Version = "1.0.0";
        public const string Url = "http://127.0.0.1:25555";
        public const string UUID = "";
        public const string Key = "";

        private string local;
        private dynamic AppDll;
        public App(string local)
        {
            InitializeComponent();
            HttpClient client = new HttpClient();
            try
            {
                this.local = local;
                var DllFile = Path.Combine(local, "out.dll");
                var PdbFile = Path.Combine(local, "out.pdb");

                using var DllFileStream = new FileStream(DllFile, FileMode.Open, FileAccess.Read);
                Assembly ass;
                var DllData = new byte[DllFileStream.Length];
                DllFileStream.Read(DllData, 0, DllData.Length);
                if (File.Exists(PdbFile))
                    using (var PdbFileStream = new FileStream(PdbFile, FileMode.Open, FileAccess.Read))
                    {
                        var PdbData = new byte[PdbFileStream.Length];
                        PdbFileStream.Read(PdbData, 0, PdbData.Length);
                        ass = Assembly.Load(DllData, PdbData);
                    }
                else
                    ass = Assembly.Load(DllData);
                Type type = ass.GetTypes()[0];
                AppDll = ass.CreateInstance(type.FullName, true);
                var res = AppDll.Init(local);
                if (!res)
                {
                    MainPage = new MainPage("No Xaml File");
                    return;
                }
                Page page = AppDll.Page;
                MainPage = page;
            }
            catch (Exception e)
            {
                MainPage = new MainPage(e.ToString());
            }
        }


        protected override void OnStart()
        {
            AppDll?.OnStart();
        }

        protected override void OnSleep()
        {
            AppDll?.OnSleep();
        }

        protected override void OnResume()
        {
            AppDll?.OnResume();
        }
    }
}
