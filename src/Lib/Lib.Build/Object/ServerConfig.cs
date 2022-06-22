using System;
using System.Collections.Generic;
using System.Text;

namespace ColoryrWork.Lib.Build.Object;

internal class ServerConfig
{

}

public record HttpObj
{
    public string IP { get; set; }
    public int Port { get; set; }
}

public record HttpListObj
{ 
    public List<HttpObj> HttpList { get; set; }
}