using ColoryrServer.DllManager;
using ColoryrServer.Pipe;
using ColoryrServer.SDK;
using Lib.IoT;
using System;
using System.Threading.Tasks;

namespace ColoryrServer.IoT
{
    internal class IoTPackDo
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
                if (pack[a] != IoTPack.ReadPack[a])
                    return false;
            }
            for (; a < 5; a++)
            {
                if (pack[a + 5] != IoTPack.SendPack[a])
                    return false;
            }
            return true;
        }

        public static void SendTcpPack(int port, byte[] pack)
        {
            var data = new byte[pack.Length + IoTPack.SendPack.Length + 1];

            IoTPack.SendPack.CopyTo(data, 0);
            pack.CopyTo(data, IoTPack.SendPack.Length);
            data[^1] = (byte)'\0';
            IoTSocketServer.TcpSendData(port, data);
        }

        public static void SendUdpPack(int port, byte[] pack)
        {
            var data = new byte[pack.Length + IoTPack.SendPack.Length + 1];

            IoTPack.SendPack.CopyTo(data, 0);
            pack.CopyTo(data, IoTPack.SendPack.Length);
            data[^1] = (byte)'\0';
            IoTSocketServer.UdpSendData(port, data);
        }

        public static void ReadTcpPack(int Port, byte[] Data)
        {
            int a = 0;
            for (; a < 5; a++)
            {
                if (Data[a] != IoTPack.ReadPack[a])
                    return;
            }
            var Data1 = new byte[Data.Length - 1];
            Data.CopyTo(Data, 5);
            Task.Run(() =>
            {
                DllRun.IoTGo(new TcpIoTRequest(Port, Data));
            });
        }
        public static void PipeReadTcpPack(int Port, byte[] Data)
        {
            int a = 0;
            for (; a < 5; a++)
            {
                if (Data[a] != IoTPack.ReadPack[a])
                    return;
            }
            var Data1 = new byte[Data.Length - 1];
            Data.CopyTo(Data, 5);

            Task.Run(() =>
            {
                PipeClient.IoT(Port, new PipeIoTData
                {
                    Port = Port,
                    IsTcp = true,
                    Data = Convert.ToBase64String(Data)
                });
            });
        }
        public static void ReadUdpPack(int Port, byte[] Data)
        {
            int a = 0;
            for (; a < 5; a++)
            {
                if (Data[a] != IoTPack.ReadPack[a])
                    return;
            }
            var Data1 = new byte[Data.Length - 1];
            Data.CopyTo(Data, 5);
            Task.Run(() =>
            {
                DllRun.IoTGo(new UdpIoTRequest(Port, Data));
            });
        }

        public static void PipeReadUdpPack(int Port, byte[] Data)
        {
            int a = 0;
            for (; a < 5; a++)
            {
                if (Data[a] != IoTPack.ReadPack[a])
                    return;
            }
            var Data1 = new byte[Data.Length - 1];
            Data.CopyTo(Data, 5);

            Task.Run(() =>
            {
                PipeClient.IoT(Port, new PipeIoTData
                {
                    Port = Port,
                    Data = Convert.ToBase64String(Data)
                });
            });
        }
    }
}
