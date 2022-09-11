using ColoryrServer.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrTest.Run;

[DllIN]
internal class DllDemo
{
    [NotesSDK("一个接口", new string[1] { "输入" }, new string[1] { "输出" })]
    public dynamic Main(HttpDllRequest http)
    {
        return "true";
    }
}
