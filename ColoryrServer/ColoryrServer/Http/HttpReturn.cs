﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using ColoryrServer.SDK;

namespace ColoryrServer.Http
{
    internal record HttpReturn
    {
        public Encoding Encoding = Encoding.UTF8;
        public string Cookie;
        public string ContentType = ServerContentType.JSON;
        public byte[] Data;
        public Stream Data1;
        public Dictionary<string, string> Head;
        public int ReCode = 200;
    }
}
