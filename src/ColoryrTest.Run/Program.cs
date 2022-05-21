using ColoryrServer.Core.Netty;

namespace ColoryrTest.Run;

internal class Program
{
    static void Main(string[] args)
    {
        NettyWebSocket.Start().Wait();
    }
}