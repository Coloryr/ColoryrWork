using System.Collections.Generic;
using System.IO;

namespace ColoryrServer.Http
{
    internal record HttpReturn
    {
        public string Cookie;
        public byte[] Data;
        public Stream Data1;
        public Dictionary<string, string> Head;
        public int ReCode = 200;
    }
}
