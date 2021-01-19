using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ColoryrServer.Pipe
{
    class PipeClient
    {
        private static ConcurrentDictionary<string, HttpListenerResponse> Requests;
        private static Socket ClientSocket;
        private static Thread ClientThread;
        private static bool IsRun;

        public static void Start(SocketConfig Config)
        {
            Requests = new();
            IsRun = true;
            ClientThread = new Thread(() =>
            {
                while (IsRun)
                {
                    if (ClientSocket?.Connected == true)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        try
                        {
                            ClientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                            ClientSocket.Connect(Config.IP, Config.Port);
                            SocketAsyncEventArgs call = new();
                            call.Completed += new EventHandler<SocketAsyncEventArgs>(Call);
                            ClientSocket.ReceiveAsync(call);
                        }
                        catch (Exception e)
                        {
                            ServerMain.LogError(e);
                        }
                    }
                }
            });
            ClientThread.Start();
        }

        private static void Call(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    var data = JsonConvert.DeserializeObject<HttpReturn>(Encoding.UTF8.GetString(e.Buffer));
                    if (Requests.TryRemove(data.UID, out var item))
                    {
                        item.ContentType = data.ContentType;
                        item.ContentEncoding = data.Encoding;
                        if (data.Head != null)
                            foreach (var Item in data.Head)
                            {
                                item.AddHeader(Item.Key, Item.Value);
                            }
                        if (data.Cookie != null)
                            item.AppendCookie(new Cookie("cs", data.Cookie));
                        if (data.Data1 == null)
                            item.OutputStream.Write(data.Data);
                        item.OutputStream.Flush();
                        item.StatusCode = data.ReCode;
                        item.Close();
                    }
                }
            }
        }

        public static void Http(PipeHttpData data, HttpListenerResponse request)
        {
            if (ClientSocket?.Connected == true)
            {
                Requests.TryAdd(data.UID, request);
                var temp = JsonConvert.SerializeObject(data);
                var temp1 = Encoding.UTF8.GetBytes(temp);
                ClientSocket.Send(temp1);
            }
            else
            {
                request.OutputStream.Write(Encoding.UTF8.GetBytes($"服务器错误"));
                request.OutputStream.Flush();
                request.StatusCode = 400;
                request.Close();
            }
        }
        public static void WebSocket(int Port)
        {

        }
        public static void IoT(int Port)
        {

        }
    }
}
