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
    public static ASPConfig Config { get; set; }

    private static IHttpClients _clients;
    private static WebApplication _web;

    private const string s_https = "https";
    private const string s_http = "http";
    private static readonly Dictionary<string, X509Certificate2> s_ssls = [];
    private static X509Certificate2 s_defaultSsl;
    private static bool s_isReboot = true;
    private static bool s_isRun = false;
    public static bool IsStarting { get; set; } = true;

    public static HttpClient GetHttpClient()
    {
        return _clients.GetOne();
    }

    private static void PageReload()
    {
        //Web.UseMiddleware<BrowserRefresh>();
    }

    public static void Main()
    {
        ServerConfig.Init(new ASPTopAPI());

        while (s_isReboot)
        {
            IsStarting = true;
            s_isReboot = false;
            ServerMain.Init();
            WebBinManager.Reload = PageReload;
            ASPConfigUtils.Start();
            ServerMain.Start();

            if (!s_isRun)
            {
                s_isRun = true;
                StartRead();
            }

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
                        var bytes = StringToByteArray(headerValue);
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
                                s_defaultSsl = ssl;
                            else
                                s_ssls.Add(item.Key, ssl);
                        }
                        catch (Exception e)
                        {
                            ServerMain.LogError($"SSL证书[{item.Value.Ssl}]加载错误", e);
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
                urls[a] = $"{(Config.UseSsl ? s_https : s_http)}://{item.IP}:{item.Port}/";
                ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
            }

            builder.WebHost.UseUrls(urls);

            builder.Services.AddHttpClient<HttpClients>();
            builder.Services.AddTransient<IHttpClients, HttpClients>();

            _web = builder.Build();
            //Web.UseHttpsRedirection();
            _clients = _web.Services.GetRequiredService<IHttpClients>();

            _web.MapGet("/", Config.RouteEnable ? HttpGet.RoteGetIndex : HttpGet.GetIndex);

            _web.MapGet("/{**name}", Config.RouteEnable ? HttpGet.RouteGet : HttpGet.Get);
            _web.MapPost("/{**name}", Config.RouteEnable ? HttpPost.RoutePost : HttpPost.Post);
            _web.MapPut("/{**name}", Config.RouteEnable ? HttpPost.RoutePost : HttpPost.Post);
            _web.MapDelete("/{**name}", Config.RouteEnable ? HttpPost.RoutePost : HttpPost.Post);

            _web.MapPost("/", Build);

            IsStarting = false;

            _web.Run();
            _web.DisposeAsync().AsTask().Wait();
            ServerMain.LogOut("正在关闭服务器");
            ServerMain.Stop();
        }

        ServerMain.LogOut("按下回车键退出");
        s_isRun = false;
        ReadThread.Interrupt();
        Console.In.Close();
    }

    private static Thread ReadThread;

    private static void StartRead()
    {
        if (!Config.NoInput)
        {
            ReadThread = new Thread(() =>
            {
                while (s_isRun)
                {
                    try
                    {
                        var command = Console.ReadLine();
                        if (command == null || IsStarting)
                        {
                            return;
                        }
                        var arg = command.Split(' ');
                        switch (arg[0])
                        {
                            case "stop":
                                _web.StopAsync().Wait();
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
        s_isReboot = true;
        ServerMain.LogOut("正在重启服务器");
        _web.StopAsync().Wait();
    }

    public static X509Certificate2? Ssl(ConnectionContext? context, string? url)
    {
        if (url != null && s_ssls.TryGetValue(url, out var item))
            return item;
        return s_defaultSsl;
    }

    private static async Task Build(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;
        if (request.Headers.TryGetValue(BuildKV.BuildK, out var value) 
            && value.Count == 1 && value.First() is { } str)
        {
            var obj1 = await PostDo.StartBuild(request.Body, str);
            await response.WriteAsync(JsonUtils.ToString(obj1));
        }
        else
        {
            response.ContentType = ServerContentType.HTML;
            await response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlIndex);
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

