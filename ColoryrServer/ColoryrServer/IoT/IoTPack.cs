using ColoryrServer.DllManager;
using ColoryrSDK;
using System;
using System.Threading.Tasks;

namespace ColoryrServer.IoT
{
    class IoTPack
    {
        private static readonly byte[] RePack = { 0x00, 0xff, 0x5a, 0xa5, 0xff };
        private static readonly byte[] SdPack = { 0xff, 0x56, 0x87, 0x4f, 0x3a };
        public static string CheckPack(byte[] pack)
        {
            if (pack.Length != 12)
            {
                return null;
            }
            int a = 0;
            for (; a < 5; a++)
            {
                if (pack[a] != RePack[a])
                    return null;
            }
            var MAC = new byte[6];
            for (; a < 11; a++)
            {
                MAC[a - 5] = pack[a];
            }
            if (pack[a] != 0x00)
            {
                return null;
            }
            string MAC_S = "";
            for (int b = 0; b < 6; b++)
            {
                MAC_S += Convert.ToString(MAC[b], 16);
            }
            return MAC_S.Substring(0, MAC_S.Length - 1);
        }

        public static void SendPack(string name, byte[] pack)
        {
            var data = new byte[pack.Length + SdPack.Length + 1];

            SdPack.CopyTo(data, 0);
            pack.CopyTo(data, SdPack.Length);
            data[data.Length - 1] = (byte)'\0';
            IoTSocket.SendData(name, data);
        }

        public static void ReadPack(string Name, byte[] Data)
        {
            int a = 0;
            for (; a < 5; a++)
            {
                if (Data[a] != RePack[a])
                    return;
            }
            var Data1 = new byte[Data.Length - 1];
            Data.CopyTo(Data, 5);
            Task.Run(() =>
            {
                try
                {
                    var Head = new IoTRequest(Name, Data);
                    var Dll = DllStonge.GetIoT(Name);
                    if (Dll != null)
                    {
                        DllRun.IoTGo(Dll, Head);
                    }
                }
                catch (Exception e)
                {
                    ServerMain.LogError(e);
                }
            });
        }
    }
}
