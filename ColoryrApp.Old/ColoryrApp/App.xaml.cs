using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Xamarin.Forms;

namespace ColoryrApp
{
    public partial class App : Application
    {
        private string local;
        private dynamic Assembly1;
        public App(string local)
        {
            InitializeComponent();
            HttpClient client = new HttpClient();

            try
            {
                var DllFile = Path.Combine(local, "out.dll");
                var PdbFile = Path.Combine(local, "out.pdb");

                using (var DllFileStream = new FileStream(DllFile, FileMode.Open, FileAccess.Read))
                {
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
                    Assembly1 = ass.CreateInstance(type.FullName, true);
                }

                Assembly1 = new test();
                Assembly1.Init();
                Page page = Assembly1.Page;
                MainPage = page;
            }
            catch (Exception e)
            {
                MainPage = new MainPage(e.ToString());
            }
        }


        protected override void OnStart()
        {
            Assembly1.OnStart();
        }

        protected override void OnSleep()
        {
            Assembly1.OnSleep();
        }

        protected override void OnResume()
        {
            Assembly1.OnResume();
        }
    }
}
