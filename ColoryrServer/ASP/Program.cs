using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using Lib.App;
using Lib.Build;
using Lib.Build.Object;
using Lib.Server;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace ColoryrServer.ASP
{
    class ColoryrLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel is LogLevel.Warning or LogLevel.Error)
                ServerMain.LogError($"{logLevel}-{eventId.Id} {formatter(state, exception)}");
            else
                ServerMain.LogOut($"{logLevel}-{eventId.Id} {formatter(state, exception)}");
        }
    }

    class ColoryrLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ColoryrLogger> _loggers = new();

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ColoryrLogger());
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }

    internal class ASPServer
    {
        public static ASPConfig Config { get; set; }

        private static WebApplication Web;

        private const string https = "https";
        private const string http = "http";

        public static void Main()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(new ColoryrLoggerProvider());

            ServerMain.ConfigUtil = new ASPConfigUtils();
            ServerMain.Start();

            //Web.UseRouting();

            if (Config.Ssl)
            {
                builder.Services.AddCertificateForwarding(options =>
                {
                    options.CertificateHeader = "X-SSL-CERT";
                    options.HeaderConverter = (headerValue) =>
                    {
                        X509Certificate2 clientCertificate = null;

                        if (!string.IsNullOrWhiteSpace(headerValue))
                        {
                            byte[] bytes = StringToByteArray(headerValue);
                            clientCertificate = new X509Certificate2(bytes);
                        }

                        return clientCertificate;
                    };
                });
                ServerMain.LogOut("正在加载SSL证书");
                if (File.Exists(Config.SslLocal))
                {
                    builder.WebHost.UseKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(i =>
                        {
                            try
                            {
                                i.ServerCertificate = new X509Certificate2(Config.SslLocal, Config.SslPassword);
                            }
                            catch (Exception e)
                            {
                                ServerMain.LogError(e);
                            }
                        });
                    });

                }
                else
                {
                    ServerMain.LogError("SSL证书找不到");
                }
            }
            foreach (var item in ServerMain.Config.Http)
            {
                builder.WebHost.UseUrls($"{(Config.Ssl ? https : http)}://{item.IP}:{item.Port}/");
                ServerMain.LogOut($"Http服务器监听{item.IP}:{item.Port}");
            }
            bool Run = true;
            if (!Config.NoInput)
                new Thread(() =>
                {
                    while (Run)
                    {
                        ServerMain.Command(Console.ReadLine());
                    }
                }).Start();

            Web = builder.Build();

            Web.MapGet("/", GetIndex);
            Web.MapGet("/{name}", GetWeb);
            Web.MapGet("/{uuid}/{name}", GetGetWeb1);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}", GetBack);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", GetBack);

            Web.MapPost("/", PostBuild);
            Web.MapPost("/{name}", GetWeb);
            Web.MapPost("/{uuid}/{name}", GetGetWeb1);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}", POSTBack);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", POSTBack);

            Web.Run();

            Run = false;
            ServerMain.LogOut("正在关闭服务器");
            ServerMain.Stop();
        }

        private static async Task PostBuild(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            if (Request.Headers[BuildKV.BuildK] == BuildKV.BuildV)
            {
                MemoryStream memoryStream = new();
                var data = new byte[2000000];
                long la = (long)Request.ContentLength;
                string Str;
                while (la > 0)
                {
                    int a = await Request.Body.ReadAsync(data);
                    la -= a;
                    memoryStream.Write(data, 0, a);
                }
                if (Request.Headers[BuildKV.BuildK1] == "true")
                {
                    var receivedData = DeCode.AES256(memoryStream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                    Str = Encoding.UTF8.GetString(receivedData);
                }
                else
                {
                    Str = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
                JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                var Json = obj.ToObject<BuildOBJ>();
                var List = ServerMain.Config.User.Where(a => a.Username == Json.User);
                if (List.Any())
                {
                    await Response.BodyWriter.WriteAsync(DllBuild.StartBuild(Json, List.First()).Data);
                }
                else
                {
                    await Response.WriteAsJsonAsync(new ReMessage
                    {
                        Build = false,
                        Message = "账户错误"
                    });
                }
            }
            else if (Request.Headers[APPKV.APPK] == APPKV.APPV)
            {
                MemoryStream memoryStream = new();
                var data = new byte[2000000];
                long la = (long)Request.ContentLength;
                string Str;
                while (la > 0)
                {
                    int a = await Request.Body.ReadAsync(data);
                    la -= a;
                    memoryStream.Write(data, 0, a);
                }
                if (Request.Headers[BuildKV.BuildK1] == "true")
                {
                    var receivedData = DeCode.AES256(memoryStream.ToArray(), ServerMain.Config.AES.Key, ServerMain.Config.AES.IV);
                    Str = Encoding.UTF8.GetString(receivedData);
                }
                else
                {
                    Str = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
                JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                var Json = obj.ToObject<DownloadObj>();
                await Response.BodyWriter.WriteAsync(AppDownload.Download(Json).Data);
            }
            else
            {
                Response.ContentType = ServerContentType.HTML;
                await Response.BodyWriter.WriteAsync(HtmlUtils.HtmlIndex);
            }
        }

        private static async Task GetIndex(HttpContext context)
        {
            HttpResponse Response = context.Response;
            Response.ContentType = ServerContentType.HTML;
            await Response.BodyWriter.WriteAsync(HtmlUtils.HtmlIndex);
        }

        private static async Task GetBack(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;
            httpReturn = ASPHttpGet.HttpGET(Request.Path, Request.Headers, Request.QueryString, uuid, name);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            if (httpReturn.Head != null)
                foreach (var Item in httpReturn.Head)
                {
                    Response.Headers.Add(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                Response.Cookies.Append("cs", httpReturn.Cookie);
            if (httpReturn.Data1 == null)
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            else
            {
                Response.ContentLength = httpReturn.Data1.Length;
                httpReturn.Data1.Seek(httpReturn.Pos, SeekOrigin.Begin);
                Response.StatusCode = 206;
                await httpReturn.Data1.CopyToAsync(Response.Body);
            }
        }

        private static async Task GetWeb(HttpContext context)
        {
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var name = context.GetRouteValue("name") as string;
            httpReturn = HttpStatic.Get(name);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            await Response.BodyWriter.WriteAsync(httpReturn.Data);
        }

        private static async Task GetGetWeb1(HttpContext context)
        {
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;
            httpReturn = HttpStatic.Get(uuid, name);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            await Response.BodyWriter.WriteAsync(httpReturn.Data);
        }

        private static async Task POSTBack(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;

            MyContentType type = MyContentType.Other;
            if (Request.ContentType != null)
            {
                if (Request.ContentType.StartsWith(ServerContentType.POSTXFORM))
                {
                    type = MyContentType.XFormData;
                }
                else if (Request.ContentType.StartsWith(ServerContentType.POSTFORMDATA))
                {
                    type = MyContentType.MFormData;
                }
                else if (Request.ContentType.StartsWith(ServerContentType.JSON))
                {
                    type = MyContentType.Json;
                }
                else
                {
                    type = MyContentType.Other;
                }
            }

            httpReturn = await ASPHttpPOST.HttpPOST(Request.Body, (long)Request.ContentLength, Request.Path, Request.Headers, type, uuid, name);
            Response.ContentType = httpReturn.ContentType;
            Response.StatusCode = httpReturn.ReCode;
            if (httpReturn.Head != null)
                foreach (var Item in httpReturn.Head)
                {
                    Response.Headers.Add(Item.Key, Item.Value);
                }
            if (httpReturn.Cookie != null)
                Response.Cookies.Append("cs", httpReturn.Cookie);
            if (httpReturn.Data1 == null)
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            else
            {
                Response.ContentLength = httpReturn.Data1.Length;
                httpReturn.Data1.Seek(httpReturn.Pos, SeekOrigin.Begin);
                Response.StatusCode = 206;
                await httpReturn.Data1.CopyToAsync(Response.Body);
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
}

