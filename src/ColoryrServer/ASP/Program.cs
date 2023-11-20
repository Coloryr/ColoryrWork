using ColoryrServer.Core;
using ColoryrServer.Core.BuilderPost;
using ColoryrServer.Core.FileSystem.Managers;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using Microsoft.AspNetCore.Connections;
using System.Security.Cryptography.X509Certificates;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP;

public static class ASPServer
{
    internal static ASPConfig Config { get; set; }
    internal static IHttpClients Clients;

    private static WebApplication Web;

    private const string https = "https";
    private const string http = "http";
    private static Dictionary<string, X509Certificate2> Ssls = new();
    private static X509Certificate2 DefaultSsl;
    private static bool IsReboot = true;
    private static bool IsRun = true;

    private static void PageReload()
    {
        //Web.UseMiddleware<BrowserRefresh>();
    }

    public static void Main()
    {
        ServerConfig.Init(new ASPTopAPI());
        while (IsReboot)
        {
            IsReboot = false;
            ServerMain.Init();
            WebBinManager.Reload = PageReload;
            ASPConfigUtils.Start();
            ServerMain.Start();

            StartRead();
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new ColoryrLoggerProvider());
            if (Config.UseSsl)
            {
                builder.Services.AddCertificateForwarding(options =>
                {
                    options.CertificateHeader = "X-SSL-CERT";
                    options.HeaderConverter = (headerValue) =>
                    {
                        byte[] bytes = StringToByteArray(headerValue);
                        return new X509Certificate2(bytes);
                    };
                });
                ServerMain.LogOut("正在加载SSL证书");
                builder.WebHost.UseKestrel(options =>
                        options.ConfigureHttpsDefaults(i =>
                              i.ServerCertificateSelector = Ssl));
                foreach (var item in Config.Ssls)
                {
                    if (File.Exists(item.Value.Ssl))
                    {
                        try
                        {
                            var ssl = new X509Certificate2(item.Value.Ssl, item.Value.Password);
                            if (item.Key == "default")
                                DefaultSsl = ssl;
                            else
                                Ssls.Add(item.Key, ssl);
                        }
                        catch (Exception e)
                        {
                            ServerMain.LogError("SSL证书[{item.Value.Ssl}]加载错误", e);
                        }
                    }
                    else
                    {
                        ServerMain.LogWarn($"SSL证书找不到[{item.Value.Ssl}]");
                    }
                }
            }
            string[] urls = new string[Config.Http.Count];
            for (int a = 0; a < Config.Http.Count; a++)
            {
                var item = Config.Http[a];
                urls[a] = $"{(Config.UseSsl ? https : http)}://{item.IP}:{item.Port}/";
                ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
            }

            builder.WebHost.UseUrls(urls);

            builder.Services.AddHttpClient<HttpClients>();
            builder.Services.AddTransient<IHttpClients, HttpClients>();

            Web = builder.Build();
            //Web.UseHttpsRedirection();
            Clients = Web.Services.GetRequiredService<IHttpClients>();

            Web.MapGet("/", Config.RouteEnable ? HttpGet.RoteGetIndex : HttpGet.GetIndex);

            Web.MapGet("/{**name}", Config.RouteEnable ? HttpGet.RouteGet : HttpGet.Get);
            Web.MapPost("/{**name}", Config.RouteEnable ? HttpPost.RoutePost : HttpPost.Post);
            Web.MapPut("/{**name}", Config.RouteEnable ? HttpPost.RoutePost : HttpPost.Post);
            Web.MapDelete("/{**name}", Config.RouteEnable ? HttpPost.RoutePost : HttpPost.Post);

            Web.MapPost("/", Build);

            Web.Run();
            Web.DisposeAsync().AsTask().Wait();
            ServerMain.LogOut("正在关闭服务器");
            ServerMain.Stop();
            ReadThread.Interrupt();
        }

        IsRun = false;
    }

    private static Thread ReadThread;

    private static void StartRead()
    {
        if (!Config.NoInput)
        {
            ReadThread = new Thread(() =>
            {
                while (IsRun)
                {
                    try
                    {
                        string command = Console.ReadLine();
                        if (command == null)
                            return;
                        var arg = command.Split(' ');
                        switch (arg[0])
                        {
                            case "stop":
                                Web.StopAsync().Wait();
                                return;
                            case "reboot":
                                Reboot();
                                return;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            });
            ReadThread.Start();
        }
    }

    public static void Reboot()
    {
        IsReboot = true;
        ServerMain.LogOut("正在重启服务器");
        Web.StopAsync().Wait();
    }

    public static X509Certificate2? Ssl(ConnectionContext? context, string? url)
    {
        if (url != null && Ssls.TryGetValue(url, out var item))
            return item;
        return DefaultSsl;
    }

    private static async Task Build(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        if (Request.Headers.ContainsKey(BuildKV.BuildK))
        {
            var obj1 = await PostDo.StartBuild(Request.Body, Request.Headers[BuildKV.BuildK]);
            await Response.WriteAsync(JsonUtils.ToString(obj1));
        }
        else
        {
            Response.ContentType = ServerContentType.HTML;
            await Response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlIndex);
        }
    }

    private static byte[] StringToByteArray(string hex)
    {
        int number = hex.Length;
        byte[] bytes = new byte[number / 2];

        for (int i = 0; i < number; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return bytes;
    }
}

