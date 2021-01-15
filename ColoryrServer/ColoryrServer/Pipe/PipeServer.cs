using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.Utils;
using Lib.Build.Object;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColoryrServer.Pipe
{
    class PipeServer
    {
        private static TcpListener ServerSocket;
        private static Thread AcceptThread;
        private static bool IsRun;

        private static bool IsHttp(byte[] temp)
        {
            if (temp.Length != 20)
            {
                return false;
            }
            for (int a = 0; a < 20; a++)
            {
                if (temp[a] != PipePack.HttpServer[a])
                {
                    return false;
                }
            }
            return true;
        }
        private static bool IsWebSocket(byte[] temp)
        {
            if (temp.Length != 20)
            {
                return false;
            }
            for (int a = 0; a < 20; a++)
            {
                if (temp[a] != PipePack.WebSocketServer[a])
                {
                    return false;
                }
            }
            return true;
        }
        private static bool IsIoT(byte[] temp)
        {
            if (temp.Length != 20)
            {
                return false;
            }
            for (int a = 0; a < 20; a++)
            {
                if (temp[a] != PipePack.IoTServer[a])
                {
                    return false;
                }
            }
            return true;
        }

        public static void Start(SocketConfig Config)
        {
            ServerSocket = new(IPAddress.Parse(Config.IP), Config.Port);
            ServerSocket.Start();
            IsRun = true;
            AcceptThread = new(async () =>
            {
                while (IsRun)
                {
                    var socket = await ServerSocket.AcceptSocketAsync();
                    await Task.Run(() =>
                    {
                        var temp = new byte[20];
                        while (IsRun)
                        {
                            if (socket.Available > 0)
                            {
                                var res = socket.Receive(temp);
                                if (res != 20)
                                    return;
                            }
                            var port = (IPEndPoint)socket.RemoteEndPoint;
                            if (IsHttp(temp))
                            {
                                SocketAsyncEventArgs call = new();
                                call.Completed += new EventHandler<SocketAsyncEventArgs>(HttpCall);
                                call.UserToken = $"http:{port.Port}";
                                socket.ReceiveAsync(call);
                            }
                            else if (IsWebSocket(temp))
                            {
                                SocketAsyncEventArgs call = new();
                                call.Completed += new EventHandler<SocketAsyncEventArgs>(WebSocketCall);
                                call.UserToken = $"ws:{port.Port}";
                                socket.ReceiveAsync(call);
                            }
                            else if (IsIoT(temp))
                            {
                                SocketAsyncEventArgs call = new();
                                call.Completed += new EventHandler<SocketAsyncEventArgs>(IoTCall);
                                call.UserToken = $"iot:{port.Port}";
                                socket.ReceiveAsync(call);
                            }
                        }
                    });
                }
            });
            AcceptThread.Start();
        }

        public static void Stop()
        {
            IsRun = false;
            ServerSocket.Stop();
        }

        public static void HttpCall(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    var data = JsonConvert.DeserializeObject<PipeHttpData>(Encoding.UTF8.GetString(e.Buffer));
                    var Dll = DllStonge.GetDll(data.UUID);
                    byte[] Res;
                    if (Dll != null)
                    {
                        var Data1 = DllRun.DllGo(Dll, data.Request, data.FunctionName);
                        var temp1 = JsonConvert.SerializeObject(Data1);
                        Res = Encoding.UTF8.GetBytes(temp1);
                    }
                    else
                    {
                        var HttpRes = new HttpReturn
                        {
                            Data = StreamUtils.JsonOBJ(new GetMeesage
                            {
                                Res = 666,
                                Text = "未找到DLL",
                                Data = data.Url
                            }),
                            ReCode = 404
                        };
                        var temp1 = JsonConvert.SerializeObject(HttpRes);
                        Res = Encoding.UTF8.GetBytes(temp1);
                    }
                    e.SetBuffer(Res, 0, Res.Length);
                    e.AcceptSocket.SendAsync(e);
                }
                else
                {
                    e.AcceptSocket.Close();
                }
            }
        }

        public static void WebSocketCall(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {

                }
                else
                {
                    e.AcceptSocket.Close();
                }
            }
        }
        public static void IoTCall(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {

                }
                else
                {
                    e.AcceptSocket.Close();
                }
            }
        }
    }
}
