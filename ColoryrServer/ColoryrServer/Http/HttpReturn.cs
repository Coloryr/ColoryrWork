using ColoryrServer.SDK;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public int Port;
    }
    internal record PipeHttpData
    {
        public HttpRequest Request { get; set; }
        public string Url { get; set; }
        public string UUID { get; set; }
        public string FunctionName { get; set; }
        public int Port { get; set; }
    }
}
