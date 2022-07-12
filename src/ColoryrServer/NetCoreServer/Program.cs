using System.Net.Sockets;
using System.Net;
using System.Security.AccessControl;
using NetCoreServer;
using ColoryrServer.NetCoreServer;
using ColoryrServer.Core;
using ColoryrServer.Core.Http.PostBuild;

class HttpCacheSession : HttpSession
{
    public HttpCacheSession(HttpServer server) : base(server) { }

    protected override void OnReceivedRequest(HttpRequest request)
    {
        if (request.Method == "HEAD")
            SendResponseAsync(Response.MakeHeadResponse());
        else if (request.Method == "GET")
        {
            HttpGet.Get(this, request);
        }
        else if ((request.Method == "POST") || (request.Method == "PUT"))
        {
            HttpPost.Post(this, request);
        }
        else if (request.Method == "DELETE")
        {
            string key = request.Url;

            // Decode the key value
            key = Uri.UnescapeDataString(key);

        }
        else if (request.Method == "OPTIONS")
            SendResponseAsync(Response.MakeOptionsResponse());
        else if (request.Method == "TRACE")
            SendResponseAsync(Response.MakeTraceResponse(request.Cache.Data));
        else
            SendResponseAsync(Response.MakeErrorResponse("Unsupported HTTP method: " + request.Method));
    }

    protected override void OnReceivedRequestError(HttpRequest request, string error)
    {
        Console.WriteLine($"Request error: {error}");
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTP session caught an error: {error}");
    }
}

class HttpCacheServer : HttpServer
{
    public HttpCacheServer(IPAddress address, int port) : base(address, port) { }

    protected override TcpSession CreateSession() { return new HttpCacheSession(this); }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTP session caught an error: {error}");
    }
}

class CoreServer
{
    public static CoreConfig Config;
    static void Main(string[] args)
    {
        // HTTP server port
        int port = 80;
        if (args.Length > 0)
            port = int.Parse(args[0]);
        // HTTP server content path
        string www = "./";
        if (args.Length > 1)
            www = args[1];

        ServerMain.ConfigUtil = new CoreConfigUtils();
        PostServerConfig.Init(new CoreTopAPI());

        ServerMain.Start();

        Console.WriteLine($"HTTP server port: {port}");
        Console.WriteLine($"HTTP server static content path: {www}");
        Console.WriteLine($"HTTP server website: http://localhost:{port}/index.html");

        Console.WriteLine();

        // Create a new HTTP server
        var server = new HttpCacheServer(IPAddress.Any, port);
        server.AddStaticContent(www, "./");

        // Start the server
        Console.Write("Server starting...");
        server.Start();
        Console.WriteLine("Done!");

        Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

        // Perform text input
        for (; ; )
        {
            string line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            // Restart the server
            if (line == "!")
            {
                Console.Write("Server restarting...");
                server.Restart();
                Console.WriteLine("Done!");
            }
        }

        // Stop the server
        Console.Write("Server stopping...");
        server.Stop();
        Console.WriteLine("Done!");
    }
}