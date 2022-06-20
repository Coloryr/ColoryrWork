using ColoryrServer.Core;
using ColoryrServer.Core.FileSystem;
using ColoryrServer.Core.FileSystem.Html;
using ColoryrServer.Core.Http;
using ColoryrServer.SDK;
using ColoryrWork.Lib.Build;
using ColoryrWork.Lib.Build.Object;
using ColoryrWork.Lib.Server;
using HttpMultipartParser;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP;

internal static class ASPServer
{
    internal static ASPConfig Config { get; set; }
    internal static IHttpClients Clients;

    private static WebApplication Web;

    private const string https = "https";
    private const string http = "http";
    private static Dictionary<string, X509Certificate2> Ssls = new();
    private static X509Certificate2 DefaultSsl;

    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new ColoryrLoggerProvider());

        ServerMain.ConfigUtil = new ASPConfigUtils();
        ServerMain.Start();

        if (Config.Ssl)
        {
            builder.Services.AddCertificateForwarding(options =>
            {
                options.CertificateHeader = "X-SSL-CERT";
                options.HeaderConverter = (headerValue) =>
                {
                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        byte[] bytes = StringToByteArray(headerValue);
                        var clientCertificate = new X509Certificate2(bytes);
                        return clientCertificate;
                    }
                    return null;
                };
            });
            ServerMain.LogOut("正在加载SSL证书");
            builder.WebHost.UseKestrel(options =>
                    options.ConfigureHttpsDefaults(i =>
                          i.ServerCertificateSelector = ASPServer.Ssl));
            foreach (var item in Config.Ssls)
            {
                if (File.Exists(item.Value.SslLocal))
                {
                    try
                    {
                        var ssl = new X509Certificate2(item.Value.SslLocal, item.Value.SslPassword);
                        if (item.Key == "default")
                            DefaultSsl = ssl;
                        else
                            Ssls.Add(item.Key, ssl);
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                    }
                }
                else
                {
                    ServerMain.LogError($"SSL证书找不到:{item.Value.SslLocal}");
                }
            }
        }
        string[] urls = new string[Config.Http.Count];
        for (int a = 0; a < Config.Http.Count; a++)
        {
            var item = Config.Http[a];
            urls[a] = $"{(Config.Ssl ? https : http)}://{item.IP}:{item.Port}/";
            ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
        }

        builder.WebHost.UseUrls(urls);

        builder.Services.AddHttpClient<HttpClients>();
        builder.Services.AddTransient<IHttpClients, HttpClients>();

        bool Run = true;
        if (!Config.NoInput)
            new Thread(() =>
            {
                while (Run)
                {
                    if (Run)
                        ServerMain.Command(Console.ReadLine());
                }
            }).Start();

        Web = builder.Build();
        Clients = Web.Services.GetRequiredService<IHttpClients>();

        Web.MapGet("/", Config.RoteEnable ? HttpGet.RoteGetIndex : HttpGet.GetIndex);

        Web.MapGet("/{**name}", Config.RoteEnable ? HttpGet.RouteGet : HttpGet.Get);
        Web.MapPost("/{**name}", Config.RoteEnable ? HttpPost.RoutePost : HttpPost.Post);
        Web.MapPut("/{**name}", Config.RoteEnable ? HttpPost.RoutePost : HttpPost.Post);
        Web.MapDelete("/{**name}", Config.RoteEnable ? HttpPost.RoutePost : HttpPost.Post);

        Web.MapPost("/", PostBuild);

        Web.Run();

        Run = false;
        ServerMain.LogOut("正在关闭服务器");
        ServerMain.Stop();
        ServerMain.LogOut("按下回车键退出");
    }

    public static X509Certificate2? Ssl(ConnectionContext? context, string? url)
    {
        if (url != null && Ssls.TryGetValue(url, out var item))
            return item;
        return DefaultSsl;
    }

    private static async Task PostBuild(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        if (Request.Headers[BuildKV.BuildK] == BuildKV.BuildV)
        {
            string Str;
            MemoryStream stream = new();
            await Request.Body.CopyToAsync(stream);
            if (Request.Headers[BuildKV.BuildK1] == "true")
            {
                var receivedData = DeCode.AES256(stream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                Str = Encoding.UTF8.GetString(receivedData);
            }
            else
            {
                Str = Encoding.UTF8.GetString(stream.ToArray());
            }
            JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
            var Json = obj.ToObject<BuildOBJ>();
            var List = ServerMain.Config.User.Where(a => a.Username == Json?.User);
            if (List.Any())
            {
                var obj1 = Core.DllManager.PostBuild.PostBuild.StartBuild(Json, List.First<Core.FileSystem.UserConfig>()).Data;
                await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
            }
            else
            {
                var obj1 = new ReMessage
                {
                    Build = false,
                    Message = "账户或密码错误"
                };
                await Response.WriteAsync(JsonConvert.SerializeObject(obj1));
            }
        }
        else
        {
            Response.ContentType = ServerContentType.HTML;
            await Response.BodyWriter.WriteAsync(WebBinManager.BaseDir.HtmlIndex);
        }
    }

    private static byte[] StringToByteArray(string hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];

        for (int i = 0; i < NumberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return bytes;
    }
}

