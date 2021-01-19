﻿namespace ColoryrServer.Pipe
{
    internal class PipePack
    {
        public static readonly byte[] HttpServer = new byte[20] { 0x00, 0x02, 0x33, 0xf4, 0x3a, 0xbc, 0x0e, 0x76, 0x01, 0xf8, 0x0a, 0xca, 0x43, 0xff, 0x21, 0x23, 0x48, 0xfe, 0xea, 0x98 };
        public static readonly byte[] WebSocketServer = new byte[20] { 0x45, 0x98, 0xea, 0x23, 0xac, 0xbf, 0x67, 0x00, 0x01, 0xcb, 0xae, 0xf7, 0x9f, 0x7b, 0x3f, 0x23, 0x11, 0x3f, 0xaf, 0x35 };
        public static readonly byte[] IoTServer = new byte[20] { 0x67, 0x26, 0x69, 0x56, 0xf2, 0x23, 0x5c, 0x5a, 0x61, 0xc8, 0x4b, 0x39, 0x48, 0xb1, 0xb2, 0x54, 0xa6, 0x51, 0xa3, 0x57 };
        public static readonly byte[] OK = new byte[2] { 0x00, 0xAA };
    }

    internal record PipeDataPack
    {
        public string UUID { get; set; }
        public int Port { get; set; }
    }
}
