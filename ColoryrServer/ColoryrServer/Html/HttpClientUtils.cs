using System.Threading;

namespace ColoryrServer.Html
{
    class HttpClientUtils
    {
        private static ExClient[] clients;
        private static readonly object LockObj = new object();
        private static int LastIndex = 0;
        public static void Start()
        {
            clients = new ExClient[ServerMain.Config.HttpClientNumber];
            for (int a = 0; a < clients.Length; a++)
            {
                var item = new ExClient();
                clients[a] = item;
            }
        }
        public static void Stop()
        {
            foreach (var item in clients)
            {
                if (item == null)
                    continue;
                item.State = ClientState.Stop;
                item.Stop();
            }
        }
        public static ExClient Get()
        {
            return new ExClient();
            while (true)
            {
                ExClient item;
                lock (LockObj)
                {
                    LastIndex++;
                    if (LastIndex >= ServerMain.Config.HttpClientNumber)
                        LastIndex = 0;
                    item = clients[LastIndex];
                }
                if (item.State == ClientState.Ready)
                {
                    item.State = ClientState.Using;
                    return item;
                }
                Thread.Sleep(1);
            }
        }

        public static void Close(ExClient client)
        {
            if (client == null)
                return;
            client.State = ClientState.Closing;
            client.Clear();
            client.State = ClientState.Ready;
        }
    }
}
