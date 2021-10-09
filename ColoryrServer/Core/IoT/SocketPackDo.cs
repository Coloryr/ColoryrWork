using ColoryrServer.DllManager;
using ColoryrServer.SDK;
using Lib.Socket;
using System.Threading.Tasks;

namespace ColoryrServer.Socket
{
    internal class SocketPackDo
    {
        public static bool CheckPack(byte[] pack)
        {
            if (pack.Length < 10)
            {
                return false;
            }
            int a = 0;
            for (; a < 5; a++)
            {
                if (pack[a] != SocketPack.ReadPack[a])
                    return false;
            }
            for (; a < 5; a++)
            {
                if (pack[a + 5] != SocketPack.SendPack[a])
                    return false;
            }
            return true;
        }

        public static void SendTcpPack(int port, byte[] pack, int Server)
        {
            var data = new byte[pack.Length + SocketPack.SendPack.Length + 1];
            SocketPack.SendPack.CopyTo(data, 0);
            pack.CopyTo(data, SocketPack.SendPack.Length);
            data[^1] = (byte)'\0';
            SocketServer.TcpSendData(port, data, Server);
        }

        public static void SendUdpPack(int port, byte[] pack, int Server)
        {
            var data = new byte[pack.Length + SocketPack.SendPack.Length + 1];

            SocketPack.SendPack.CopyTo(data, 0);
            pack.CopyTo(data, SocketPack.SendPack.Length);
            data[^1] = (byte)'\0';
            SocketServer.UdpSendData(port, data, Server);
        }

        public static void ReadTcpPack(int Port, byte[] Data)
        {
            int a = 0;
            for (; a < 5; a++)
            {
                if (Data[a] != SocketPack.ReadPack[a])
                    return;
            }
            var Data1 = new byte[Data.Length - 1];
            Data.CopyTo(Data, 5);
            Task.Run(() =>
            {
                DllRun.SocketGo(new TcpSocketRequest(Port, Data, 0));
            });
        }

        public static void ReadUdpPack(int Port, byte[] Data)
        {
            int a = 0;
            for (; a < 5; a++)
            {
                if (Data[a] != SocketPack.ReadPack[a])
                    return;
            }
            var Data1 = new byte[Data.Length - 1];
            Data.CopyTo(Data, 5);
            Task.Run(() =>
            {
                DllRun.SocketGo(new UdpSocketRequest(Port, Data, 0));
            });
        }
    }
}
