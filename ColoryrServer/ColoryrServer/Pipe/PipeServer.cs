//using ColoryrServer.DllManager;
//using ColoryrServer.FileSystem;
//using ColoryrServer.Http;
//using ColoryrServer.SDK;
//using ColoryrServer.Utils;
//using Lib.Build.Object;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Concurrent;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ColoryrServer.Pipe
//{
//    class PipeServer
//    {
//        private static ConcurrentDictionary<int, Socket> PipeClients = new();
//        private static TcpListener ServerSocket;
//        private static Thread AcceptThread;
//        private static bool IsRun;

//        private static bool IsHttp(byte[] temp)
//        {
//            if (temp.Length != 20)
//            {
//                return false;
//            }
//            for (int a = 0; a < 20; a++)
//            {
//                if (temp[a] != PipePack.HttpServer[a])
//                {
//                    return false;
//                }
//            }
//            return true;
//        }
//        private static bool IsWebSocket(byte[] temp)
//        {
//            if (temp.Length != 20)
//            {
//                return false;
//            }
//            for (int a = 0; a < 20; a++)
//            {
//                if (temp[a] != PipePack.WebSocketServer[a])
//                {
//                    return false;
//                }
//            }
//            return true;
//        }
//        private static bool IsIoT(byte[] temp)
//        {
//            if (temp.Length != 20)
//            {
//                return false;
//            }
//            for (int a = 0; a < 20; a++)
//            {
//                if (temp[a] != PipePack.IoTServer[a])
//                {
//                    return false;
//                }
//            }
//            return true;
//        }
//        private static bool IsMqtt(byte[] temp)
//        {
//            if (temp.Length != 20)
//            {
//                return false;
//            }
//            for (int a = 0; a < 20; a++)
//            {
//                if (temp[a] != PipePack.MqttServer[a])
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        public static void Start(SocketConfig Config)
//        {
//            ServerSocket = new(IPAddress.Parse(Config.IP), Config.Port);
//            ServerSocket.Start();
//            IsRun = true;
//            AcceptThread = new(async () =>
//            {
//                while (IsRun)
//                {
//                    var socket = await ServerSocket.AcceptSocketAsync();
//                    var task = Task.Run(() =>
//                    {
//                        var temp = new byte[20];
//                        while (IsRun)
//                        {
//                            if (socket.Available > 0)
//                            {
//                                var res = socket.Receive(temp);
//                                if (res != 20)
//                                    return;
//                            }
//                            var port = (IPEndPoint)socket.RemoteEndPoint;
//                            if (IsHttp(temp))
//                            {
//                                SocketAsyncEventArgs call = new();
//                                call.Completed += new EventHandler<SocketAsyncEventArgs>(HttpCall);
//                                call.UserToken = $"http:{port.Port}";
//                                socket.ReceiveAsync(call);
//                            }
//                            else if (IsWebSocket(temp))
//                            {
//                                SocketAsyncEventArgs call = new();
//                                call.Completed += new EventHandler<SocketAsyncEventArgs>(WebSocketCall);
//                                call.UserToken = $"ws:{port.Port}";
//                                socket.ReceiveAsync(call);
//                                PipeClients.TryAdd(((IPEndPoint)socket.RemoteEndPoint).Port, socket);
//                            }
//                            else if (IsIoT(temp))
//                            {
//                                SocketAsyncEventArgs call = new();
//                                call.Completed += new EventHandler<SocketAsyncEventArgs>(IoTCall);
//                                call.UserToken = $"iot:{port.Port}";
//                                socket.ReceiveAsync(call);
//                                PipeClients.TryAdd(((IPEndPoint)socket.RemoteEndPoint).Port, socket);
//                            }
//                            else if (IsMqtt(temp))
//                            {
//                                SocketAsyncEventArgs call = new();
//                                call.Completed += new EventHandler<SocketAsyncEventArgs>(MqttCall);
//                                call.UserToken = $"mqtt:{port.Port}";
//                                socket.ReceiveAsync(call);
//                                PipeClients.TryAdd(((IPEndPoint)socket.RemoteEndPoint).Port, socket);
//                            }
//                            else
//                            {
//                                return;
//                            }
//                            if (PipeClients.ContainsKey(port.Port))
//                            {
//                                PipeClients[port.Port].Close();
//                                PipeClients[port.Port].Dispose();
//                                PipeClients[port.Port] = socket;
//                            }
//                            else
//                                PipeClients.TryAdd(port.Port, socket);
//                        }
//                    });
//                }
//            });
//            AcceptThread.Start();
//        }

//        public static void Stop()
//        {
//            IsRun = false;
//            ServerSocket.Stop();
//        }

//        private static void HttpCall(object sender, SocketAsyncEventArgs e)
//        {
//            if (e.LastOperation == SocketAsyncOperation.Receive)
//            {
//                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
//                {
//                    var data = JsonConvert.DeserializeObject<PipeHttpData>(Encoding.UTF8.GetString(e.Buffer));
//                    var Dll = DllStonge.GetDll(data.UUID);
//                    byte[] Res;
//                    if (Dll != null)
//                    {
//                        var obj = DllRun.DllGo(Dll, data.Request, data.FunctionName);
//                        var Data1 = new PipeReData
//                        {
//                            Data = obj,
//                            Type = PipiPackType.Http,
//                            UID = data.UID
//                        };
//                        var temp1 = JsonConvert.SerializeObject(Data1);
//                        Res = Encoding.UTF8.GetBytes(temp1);
//                    }
//                    else
//                    {
//                        var obj = new HttpReturn
//                        {
//                            Data = StreamUtils.JsonOBJ(new GetMeesage
//                            {
//                                Res = 666,
//                                Text = "未找到DLL",
//                                Data = data.Url
//                            }),
//                            ReCode = 404
//                        };
//                        var HttpRes = new PipeReData
//                        {
//                            Data = obj,
//                            Type = PipiPackType.Http,
//                            UID = data.UID
//                        };
//                        var temp1 = JsonConvert.SerializeObject(HttpRes);
//                        Res = Encoding.UTF8.GetBytes(temp1);
//                    }
//                    e.SetBuffer(Res, 0, Res.Length);
//                    e.AcceptSocket.SendAsync(e);
//                }
//                else
//                {
//                    e.AcceptSocket.Close();
//                }
//            }
//        }

//        private static void WebSocketCall(object sender, SocketAsyncEventArgs e)
//        {
//            if (e.LastOperation == SocketAsyncOperation.Receive)
//            {
//                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
//                {
//                    var data = JsonConvert.DeserializeObject<PipeWebSocketData>(Encoding.UTF8.GetString(e.Buffer));
//                    switch (data.State)
//                    {
//                        case SocketState.Open:
//                            DllRun.WebSocketGo(new WebSocketOpen(data.IsAvailable, data.Info, data.Server));
//                            break;
//                        case SocketState.Close:
//                            DllRun.WebSocketGo(new WebSocketClose(data.Info));
//                            break;
//                        case SocketState.Message:
//                            DllRun.WebSocketGo(new WebSocketMessage(data.IsAvailable, data.Info, data.Data, data.Server));
//                            break;
//                    }
//                }
//                else
//                {
//                    e.AcceptSocket.Close();
//                }
//            }
//        }

//        public static void WebSocketSendMessage(int Port, int Server, string data, bool Base)
//        {
//            Task.Run(() =>
//            {
//                if (!PipeClients.ContainsKey(Server))
//                    return;
//                var obj = new PipeWebSocketData
//                {
//                    Port = Port,
//                    Data = data,
//                    Base = Base,
//                    State = SocketState.Message
//                };
//                var Data = new PipeReData
//                {
//                    Type = PipiPackType.WebSocket,
//                    Data = obj
//                };
//                var res = JsonConvert.SerializeObject(Data);
//                PipeClients[Server].Send(Encoding.UTF8.GetBytes(res));
//            });
//        }
//        internal static void WebSocketSendClose(int Port, int Server)
//        {
//            Task.Run(() =>
//            {
//                if (!PipeClients.ContainsKey(Server))
//                    return;
//                var obj = new PipeWebSocketData
//                {
//                    Port = Port,
//                    State = SocketState.Close
//                };
//                var Data = new PipeReData
//                {
//                    Type = PipiPackType.WebSocket,
//                    Data = obj
//                };
//                var res = JsonConvert.SerializeObject(Data);
//                PipeClients[Server].Send(Encoding.UTF8.GetBytes(res));
//            });
//        }
//        private static void IoTCall(object sender, SocketAsyncEventArgs e)
//        {
//            if (e.LastOperation == SocketAsyncOperation.Receive)
//            {
//                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
//                {
//                    var data = JsonConvert.DeserializeObject<PipeIoTData>(Encoding.UTF8.GetString(e.Buffer));
//                    var bytes = Encoding.UTF8.GetBytes(data.Data);
//                    if (data.IsTcp)
//                    {
//                        Task.Run(() =>
//                        {
//                            DllRun.IoTGo(new TcpIoTRequest(data.Port, bytes, data.Server));
//                        });
//                    }
//                    else
//                    {
//                        Task.Run(() =>
//                        {
//                            DllRun.IoTGo(new UdpIoTRequest(data.Port, bytes, data.Server));
//                        });
//                    }
//                }
//                else
//                {
//                    e.AcceptSocket.Close();
//                }
//            }
//        }
//        public static void IoTSend(int Port, int Server, byte[] data)
//        {
//            Task.Run(() =>
//            {
//                if (!PipeClients.ContainsKey(Server))
//                    return;
//                var obj = new PipeIoTData
//                {
//                    Port = Port,
//                    Data = Encoding.UTF8.GetString(data),
//                    IsTcp = true
//                };
//                var Data = new PipeReData
//                {
//                    Type = PipiPackType.IoT,
//                    Data = obj
//                };
//                var res = JsonConvert.SerializeObject(Data);
//                PipeClients[Server].Send(Encoding.UTF8.GetBytes(res));
//            });
//        }
//        public static void UdpSend(int Port, int Server, byte[] data)
//        {
//            Task.Run(() =>
//            {
//                if (!PipeClients.ContainsKey(Server))
//                    return;
//                var obj = new PipeIoTData
//                {
//                    Port = Port,
//                    Data = Encoding.UTF8.GetString(data)
//                };
//                var Data = new PipeReData
//                {
//                    Type = PipiPackType.IoT,
//                    Data = obj
//                };
//                var res = JsonConvert.SerializeObject(Data);
//                PipeClients[Server].Send(Encoding.UTF8.GetBytes(res));
//            });
//        }
//        private static void MqttCall(object sender, SocketAsyncEventArgs e)
//        {
//            if (e.LastOperation == SocketAsyncOperation.Receive)
//            {
//                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
//                {

//                }
//                else
//                {
//                    e.AcceptSocket.Close();
//                }
//            }
//        }
//    }
//}
