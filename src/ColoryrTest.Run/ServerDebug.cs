using ColoryrServer.ServerDebug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoryrTest.Run;

internal static class ServerDebug
{
    public static void Test()
    {
        Console.ReadLine();

        DebugIn.Init("127.0.0.1", 20000, "Key", "IV");

        Console.WriteLine("Init");

        DebugIn.Register("web/test", typeof(DllDemo));

        Console.ReadLine();
    }
}
