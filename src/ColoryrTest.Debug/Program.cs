using ColoryrServer.ServerDebug;
using ColoryrTest.Run;

namespace ColoryrTest.Debug;

internal class Program
{
    static void Main(string[] args)
    {
        Console.ReadLine();

        DebugIn.Init("127.0.0.1", 20000, "Key", "IV");

        Console.WriteLine("Init");

        DebugIn.Register("web/test", typeof(DllDemo));

        Console.ReadLine();
    }
}