//using ColoryrServer.FileSystem;
//using ColoryrServer.Http;
//using ColoryrServer.IoT;
//using ColoryrServer.WebSocket;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Concurrent;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;

//namespace ColoryrServer.Pipe
//{
//    class PipeClient
//    {
//        private static ConcurrentDictionary<string, HttpListenerResponse> HttpRequests;
//        private static Socket ClientSocket;
//        private static Thread ClientThread;
//        private static int ClientPort;
//        private static bool IsRun;

//        public static void Start(SocketConfig Config)
//        {
//            HttpRequests = new();
//            IsRun = true;
//            ClientThread = new Thread(() =>
//            {
//                while (IsRun)
//                {
//                    if (ClientSocket?.Connected == true)
//                    {
//                        Thread.Sleep(1000);
//                    }
//                    else
//                    {
//                        try
//                        {
//                            ClientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
//                            ClientSocket.Connect(Config.IP, Config.Port);
//                            ClientPort = ((IPEndPoint)ClientSocket.LocalEndPoint).Port;
//                            SocketAsyncEventArgs call = new();
//                            call.Completed += new EventHandler<SocketAsyncEventArgs>(Call);
//                            ClientSocket.ReceiveAsync(call);
//                        }
//                        catch (Exception e)
//                        {
//                            ServerMain.LogError(e);
//                        }
//                    }
//                }
//            });
//            ClientThread.Start();
//        }

//        private static void Call(object sender, SocketAsyncEventArgs e)
//        {
//            if (e.LastOperation == SocketAsyncOperation.Receive)
//            {
//                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
//                {
//                    var data = JsonConvert.DeserializeObject<PipeReData>(Encoding.UTF8.GetString(e.Buffer));
//                    switch (data.Type)
//                    {
//                        case PipiPackType.Http:
//                            HttpReturn obj = (HttpReturn)data.Data;
//                            if (HttpRequests.TryRemove(data.UID, out var item))
//                            {
//                                item.ContentType = obj.ContentType;
//                                item.ContentEncoding = obj.Encoding;
//                                if (obj.Head != null)
//                                    foreach (var Item in obj.Head)
//                                    {
//                                        item.AddHeader(Item.Key, Item.Value);
//                                    }
//                                if (obj.Cookie != null)
//                                    item.AppendCookie(new Cookie("cs", obj.Cookie));
//                                if (obj.Data1 == null)
//                                    item.OutputStream.Write(obj.Data);
//                                item.OutputStream.Flush();
//                                item.StatusCode = obj.ReCode;
//                                item.Close();
//                            }
//                            break;
//                        case PipiPackType.WebSocket:
//                            PipeWebSocketData obj1 = (PipeWebSocketData)data.Data;
//                            switch (obj1.State)
//                            {
//                                case SocketState.Message:
//                                    if (obj1.Base)
//                                    {
//                                        ServerWebSocket.Send(obj1.Port, Convert.FromBase64String(obj1.Data), 0);
//                                    }
//                                    else
//                                    {
//                                        ServerWebSocket.Send(obj1.Port, obj1.Data, 0);
//                                    }
//                                    break;
//                                case SocketState.Close:
//                                    ServerWebSocket.Close(obj1.Port, 0);
//                                    break;
//                            }
//                            break;
//                        case PipiPackType.IoT:
//                            PipeIoTData obj2 = (PipeIoTData)data.Data;
//                            if (obj2.IsTcp)
//                            {
//                                IoTSocketServer.TcpSendData(obj2.Port, Encoding.UTF8.GetBytes(obj2.Data), 0);
//                            }
//                            else
//                            {
//                                IoTSocketServer.UdpSendData(obj2.Port, Encoding.UTF8.GetBytes(obj2.Data), 0);
//                            }
//                            break;
//                    }
//                }
//            }
//        }

//        public static void Http(PipeHttpData data, HttpListenerResponse request)
//        {
//            if (ClientSocket?.Connected == true)
//            {
//                HttpRequests.TryAdd(data.UID, request);
//                var temp = JsonConvert.SerializeObject(data);
//                var temp1 = Encoding.UTF8.GetBytes(temp);
//                ClientSocket.Send(temp1);
//            }
//            else
//            {
//                request.OutputStream.Write(Encoding.UTF8.GetBytes($"服务器错误"));
//                request.OutputStream.Flush();
//                request.StatusCode = 400;
//                request.Close();
//            }
//        }
//        public static void WebSocket(int Port, PipeWebSocketData data)
//        {
//            if (ClientSocket?.Connected == true)
//            {
//                data.Server = ClientPort;
//                data.Port = Port;
//                var temp = JsonConvert.SerializeObject(data);
//                var temp1 = Encoding.UTF8.GetBytes(temp);
//                ClientSocket.Send(temp1);
//            }
//        }
//        public static void IoT(int Port, PipeIoTData data)
//        {
//            if (ClientSocket?.Connected == true)
//            {
//                data.Server = ClientPort;
//                data.Port = Port;
//                var temp = JsonConvert.SerializeObject(data);
//                var temp1 = Encoding.UTF8.GetBytes(temp);
//                ClientSocket.Send(temp1);
//            }
//        }

//        public static void Mqtt(int Port, PipeMqttData data)
//        {
//            if (ClientSocket?.Connected == true)
//            {
//                data.Server = ClientPort;
//                data.Port = Port;
//                var temp = JsonConvert.SerializeObject(data);
//                var temp1 = Encoding.UTF8.GetBytes(temp);
//                ClientSocket.Send(temp1);
//            }
//        }
//    }
//}
