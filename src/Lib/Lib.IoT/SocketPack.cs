namespace ColoryrWork.Lib.Socket
{
    class SocketPack
    {
        public static readonly byte[] ReadPack = { 0x00, 0xff, 0x5a, 0xa5, 0xff };
        public static readonly byte[] SendPack = { 0xff, 0x56, 0x87, 0x4f, 0x3a };
    }
}
