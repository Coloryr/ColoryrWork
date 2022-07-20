﻿using System.Net.Sockets;
using System.Net;
using System.Security.AccessControl;
using NetCoreServer;
using ColoryrServer.NetCoreServer;
using ColoryrServer.Core;
using ColoryrServer.Core.Http.PostBuild;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Ubiety.Dns.Core;
using Org.BouncyCastle.Asn1.Ocsp;

public interface IHttp
{
    public HttpResponse Response { get; }
    public bool SendResponseAsync(HttpResponse response);
}

public interface IHttpServer
{
    public void AddStaticContent(string path, string prefix = "/", string filter = "*.*", TimeSpan? timeout = null);
    public bool Start();
    public bool Restart();
    public bool Stop();
}

public static class HttpServerFuntion
{
    public static void OnReceivedRequest(IHttp http, HttpRequest request)
    {
        if (request.Method == "HEAD")
            http.SendResponseAsync(http.Response.MakeHeadResponse());
        else if (request.Method == "GET")
        {
            HttpGet.Get(http, request);
        }
        else if ((request.Method == "POST") || (request.Method == "PUT"))
        {
            HttpPost.Post(http, request);
        }
        else if (request.Method == "DELETE")
        {
            string key = request.Url;

            // Decode the key value
            key = Uri.UnescapeDataString(key);

        }
        else if (request.Method == "OPTIONS")
            http.SendResponseAsync(http.Response.MakeOptionsResponse());
        else if (request.Method == "TRACE")
            http.SendResponseAsync(http.Response.MakeTraceResponse(request.Cache.Data));
        else
            http.SendResponseAsync(http.Response.MakeErrorResponse("Unsupported HTTP method: " + request.Method));
    }

    public static void OnReceivedRequestError(HttpRequest request, string error)
        => ServerMain.LogError($"Request error: {error}");

    public static void OnError(SocketError error)
        => ServerMain.LogError($"HTTP session caught an error: {error}");
}

class HttpCacheSession : HttpSession, IHttp
{
    public HttpCacheSession(HttpServer server) : base(server) { }

    protected override void OnReceivedRequest(HttpRequest request)
        => HttpServerFuntion.OnReceivedRequest(this, request);

    protected override void OnReceivedRequestError(HttpRequest request, string error)
        => HttpServerFuntion.OnReceivedRequestError(request, error);

    protected override void OnError(SocketError error)
        => HttpServerFuntion.OnError(error);
}

class HttpsCacheSession : HttpsSession, IHttp
{
    public HttpsCacheSession(HttpsServer server) : base(server) { }

    protected override void OnReceivedRequest(HttpRequest request)
        => HttpServerFuntion.OnReceivedRequest(this, request);

    protected override void OnReceivedRequestError(HttpRequest request, string error)
        => HttpServerFuntion.OnReceivedRequestError(request, error);

    protected override void OnError(SocketError error)
        => HttpServerFuntion.OnError(error);
}

class HttpCacheServer : HttpServer, IHttpServer
{
    public HttpCacheServer(IPAddress address, int port) : base(address, port) { }

    protected override TcpSession CreateSession() { return new HttpCacheSession(this); }

    protected override void OnError(SocketError error)
    {
        ServerMain.LogError($"HTTP session caught an error: {error}");
    }
}

class HttpsCacheServer : HttpsServer, IHttpServer
{
    public HttpsCacheServer(SslContext context, IPAddress address, int port) : base(context, address, port) { }

    protected override SslSession CreateSession() { return new HttpsCacheSession(this); }

    protected override void OnError(SocketError error)
    {
        ServerMain.LogError($"HTTP session caught an error: {error}");
    }
}

class CoreServer
{
    public static CoreConfig Config;
    static void Main(string[] args)
    {
        ServerMain.ConfigUtil = new CoreConfigUtils();
        PostServerConfig.Init(new CoreTopAPI());

        ServerMain.Start();
        IHttpServer server;

        if (Config.UseSsl)
        {
            var context = new SslContext(SslProtocols.Tls13, new X509Certificate2(Config.Ssl.Local, Config.Ssl.Password));
            server = new HttpsCacheServer(context, IPAddress.Parse(Config.Http.IP), Config.Http.Port);
            ServerMain.LogOut($"服务器启动与https://{Config.Http.IP}:{Config.Http.Port}");
        }
        else
        {
            server = new HttpCacheServer(IPAddress.Parse(Config.Http.IP), Config.Http.Port);
            ServerMain.LogOut($"服务器启动与http://{Config.Http.IP}:{Config.Http.Port}");
        }
        
        server.AddStaticContent("./", "./");

        // Start the server
        ServerMain.LogOut("Server starting...");
        server.Start();
        ServerMain.LogOut("Done!");

        ServerMain.LogOut("Press Enter to stop the server or '!' to restart the server...");

        // Perform text input
        for (; ; )
        {
            string line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            // Restart the server
            if (line == "!")
            {
                ServerMain.LogOut("Server restarting...");
                server.Restart();
                ServerMain.LogOut("Done!");
            }
        }

        // Stop the server
        ServerMain.LogOut("Server stopping...");
        server.Stop();
        ServerMain.LogOut("Done!");
    }
}