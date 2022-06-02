using Fleck;

namespace ColoryrTest.Run;

internal class Program
{
    static void Main(string[] args)
    {
        FleckLog.Level = LogLevel.Error;
        var Server = new WebSocketServer("ws://127.0.0.1:23333/");
        Server.Start(Socket =>
        {
            Socket.OnOpen = () =>
            {

            };
            Socket.OnClose = () =>
            {

            };
            Socket.OnMessage = message =>
            {
                Socket.Send(message);
            };
        });
        Console.Read();
    }
}