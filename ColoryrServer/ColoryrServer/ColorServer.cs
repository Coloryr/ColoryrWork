using System.ServiceProcess;

namespace ColoryrServer
{
    partial class ColorServer : ServiceBase
    {
        public ColorServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ServerMain.Start();
        }

        protected override void OnStop()
        {
            ServerMain.Stop();
        }
    }
}
