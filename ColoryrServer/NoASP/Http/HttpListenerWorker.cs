using System.Net;

namespace ColoryrServer.NoASP
{
    internal class HttpListenerWorker
    {
        private HttpListener listener;

        private int now = 0;

        public HttpListenerWorker(string IP)
        {
            listener = new();
            listener.Prefixes.Add(IP);
            listener.Start();
            listener.BeginGetContext(ContextReady, null);
        }

        private void ContextReady(IAsyncResult ar)
        {
            if (HttpServer.IsActive)
            {
                listener.BeginGetContext(ContextReady, ar.AsyncState);
                HttpServer.Workers[now++].Add(listener.EndGetContext(ar));
                if (now >= NoASP.Config.HttpThreadNumber)
                    now = 0;
            }
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
