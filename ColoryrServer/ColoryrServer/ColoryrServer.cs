using System.ServiceProcess;

namespace ColoryrServer
{
    partial class ColoryrServer : ServiceBase
    {
        public ColoryrServer()
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
