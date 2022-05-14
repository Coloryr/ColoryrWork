using ColoryrServer.SDK;
using System.Collections.Generic;
using System.Text;

namespace ColoryrServer.Http;

public enum ResType
{
    Json, String, Byte, Stream
}
public record HttpReturn
{
    public Encoding Encoding = Encoding.UTF8;
    public string Cookie;
    public string ContentType = ServerContentType.JSON;
    public ResType Res;
    public object Data;
    public Dictionary<string, string> Head;
    public int ReCode = 200;
    public int Pos = 0;
}
