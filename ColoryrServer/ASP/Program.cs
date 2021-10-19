using ColoryrServer.DllManager;
using ColoryrServer.FileSystem;
using ColoryrServer.Http;
using ColoryrServer.SDK;
using ColoryrServer.Utils;
using HttpMultipartParser;
using Lib.App;
using Lib.Build;
using Lib.Build.Object;
using Lib.Server;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections;
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
                            Exception? exception, Func<TState, Exception, string> formatter)
        {
            if (eventId.Id == 100 || eventId.Id == 101)
                return;
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
        private static IHttpClients Clients;

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
            string[] urls = new string[ServerMain.Config.Http.Count];
            for (int a = 0; a < ServerMain.Config.Http.Count; a++)
            {
                var item = ServerMain.Config.Http[a];
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

            Web.MapGet("/", Config.RoteEnable ? RoteGetIndex : GetIndex);
            Web.MapGet("/{**name}", Config.RoteEnable ? RoteGetWeb : GetWeb);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}", GetBack);
            Web.MapGet(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", GetBack);

            Web.MapPost("/", PostBuild);
            Web.MapPost("/{**name}", Config.RoteEnable ? RoteGetWeb : GetWeb);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}", POSTBack);
            Web.MapPost(ServerMain.Config.Requset.WebAPI + "/{uuid}/{name}", POSTBack);

            Web.Run();

            Run = false;
            ServerMain.LogOut("正在关闭服务器");
            ServerMain.Stop();
            ServerMain.LogOut("按下回车键退出");
        }

        public static X509Certificate2? Ssl(ConnectionContext? context, string? url)
        {
            if (Ssls.TryGetValue(url, out var item))
            {
                return item;
            }
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
                var Json = obj.ToObject<DownloadObj>();
                await Response.BodyWriter.WriteAsync(AppDownload.Download(Json).Data);
            }
            else
            {
                Response.ContentType = ServerContentType.HTML;
                await Response.BodyWriter.WriteAsync(HtmlUtils.HtmlIndex);
            }
        }

        private static string[] data1 = Array.Empty<string>();

        private static async Task RoteGetIndex(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            if (Config.UrlRotes.TryGetValue(Request.Host.Host, out var rote1))
            {
                await RoteDo(Request, data1, rote1, Response);
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
            string Url = Request.Path;
            if (Url.StartsWith("//"))
            {
                Url = Url[1..];
            }
            var Dll = DllStonge.GetDll(uuid);
            if (Dll != null)
            {
                var Temp = new Dictionary<string, dynamic>();
                if (Request.QueryString.HasValue)
                {
                    foreach (string a in Request.QueryString.Value.Split('&'))
                    {
                        var item = a.Split("=");
                        Temp.Add(item[0], item[1]);
                    }
                }
                NameValueCollection collection = new();
                foreach (var item in Request.Headers)
                {
                    collection.Add(item.Key, item.Value);
                }
                httpReturn = DllRun.DllGo(Dll, new()
                {
                    Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
                    RowRequest = collection,
                    Parameter = Temp,
                    Method = Request.Method,
                    ContentType = MyContentType.XFormData
                }, name);
            }
            else
            {
                httpReturn = new()
                {
                    Data = HtmlUtils.Html404,
                    ContentType = ServerContentType.HTML,
                    ReCode = 200
                };
            }
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

        private static async Task RoteDo(HttpRequest Request, string[] arg, Rote rote, HttpResponse Response, int start = 1)
        {
            using HttpClient ProxyRequest = Clients.GetOne();
            HttpRequestMessage message = new();
            message.Method = new HttpMethod(Request.Method);
            string url = "";
            if (arg.Length > 1)
            {
                for (int a = start; a < arg.Length; a++)
                {
                    url += $"/{arg[a]}";
                }
            }

            message.RequestUri = new Uri($"{rote.Url}{url}");
            if (Request.Method is "POST")
                message.Content = new StreamContent(Request.Body);
            else
                message.Content = new ByteArrayContent(Array.Empty<byte>());

            foreach (var item in Request.Headers)
            {
                if (item.Key.StartsWith("Content"))
                {
                    message.Content.Headers.Add(item.Key, (IEnumerable<string>)item.Value);
                }
                else
                    message.Headers.Add(item.Key, (IEnumerable<string>)item.Value);
            }
            foreach (var item in rote.Heads)
            {
                message.Headers.Add(item.Key, item.Value);
            }

            if (url.EndsWith(".php"))
            {
                var res = await ProxyRequest.SendAsync(message);
                Response.StatusCode = (int)res.StatusCode;
                Response.ContentType = res.Content.Headers.ContentType.ToString();

                foreach (var item in res.Headers)
                {
                    if (item.Key is "Transfer-Encoding")
                        continue;
                    StringValues values = new(item.Value.ToArray());
                    Response.Headers.Add(item.Key, values);
                }

                await res.Content.CopyToAsync(Response.Body);
            }
            else
            {
                var res = await ProxyRequest.SendAsync(message);

                Response.StatusCode = (int)res.StatusCode;
                Response.ContentType = res.Content.Headers.ContentType.ToString();

                foreach (var item in res.Headers)
                {

                    StringValues values = new(item.Value.ToArray());
                    Response.Headers.Add(item.Key, values);
                }
                await res.Content.CopyToAsync(Response.Body);
            }
        }

        private static async Task RoteGetWeb(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var name = context.GetRouteValue("name") as string;
            if (name == null)
                return;
            var arg = name.Split('/');
            if (Config.UrlRotes.TryGetValue(Request.Host.Host, out var rote1))
            {
                await RoteDo(Request, arg, rote1, Response, 0);
            }
            else if (Config.Rotes.TryGetValue(arg[0], out var rote))
            {
                await RoteDo(Request, arg, rote, Response);
            }
            else if (arg.Length == 2)
            {
                httpReturn = HttpStatic.Get(arg[0], arg[1]);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            }
            else
            {
                httpReturn = HttpStatic.Get(name);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            }
        }

        private static async Task GetWeb(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var name = context.GetRouteValue("name") as string;
            if (name == null)
                return;
            var arg = name.Split('/');
            if (arg.Length == 2)
            {
                httpReturn = HttpStatic.Get(arg[0], arg[1]);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            }
            else
            {
                httpReturn = HttpStatic.Get(name);
                Response.ContentType = httpReturn.ContentType;
                Response.StatusCode = httpReturn.ReCode;
                await Response.BodyWriter.WriteAsync(httpReturn.Data);
            }
        }

        private static async Task POSTBack(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            HttpReturn httpReturn;
            var uuid = context.GetRouteValue("uuid") as string;
            var name = context.GetRouteValue("name") as string;

            MyContentType type = MyContentType.XFormData;
            var temp = new Dictionary<string, dynamic>();
            if (Request.ContentType != null)
            {
                if (Request.ContentType is ServerContentType.POSTXFORM)
                {
                    foreach (var item in Request.Form)
                    {
                        temp.Add(item.Key, item.Value);
                    }
                    foreach (var item in Request.Form.Files)
                    {
                        temp.Add(item.Name, item);
                    }
                }
                else if (Request.ContentType.StartsWith(ServerContentType.POSTFORMDATA))
                {
                    try
                    {
                        //boundary??
                        //var temp1 = Request.ContentType.Remove(0, ServerContentType.POSTFORMDATA.Length);
                        //MemoryStream stream = new();
                        //await Request.Body.CopyToAsync(stream);
                        //var temp2 = Encoding.UTF8.GetString(stream.ToArray());
                        //stream.Seek(0, SeekOrigin.Begin);
                        //var parser = await MultipartFormDataParser.ParseAsync(stream);
                        //if (parser == null)
                        //{
                        //    httpReturn = new HttpReturn
                        //    {
                        //        Data = StreamUtils.JsonOBJ(new GetMeesage
                        //        {
                        //            Res = 123,
                        //            Text = "表单解析发生错误"
                        //        })
                        //    };
                        //    await Response.BodyWriter.WriteAsync(httpReturn.Data);
                        //    return;
                        //}

                        var parser = await MultipartFormDataParser.ParseAsync(Request.Body);
                        foreach (var item in parser.Parameters)
                        {
                            temp.Add(item.Name, item.Data);
                        }
                        foreach (var item in parser.Files)
                        {
                            temp.Add(item.Name, new HttpMultipartFile()
                            {
                                Data = item.Data,
                                FileName = item.FileName,
                                ContentType = item.ContentType,
                                ContentDisposition = item.ContentDisposition
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ServerMain.LogError(e);
                        httpReturn = new HttpReturn
                        {
                            Data = StreamUtils.JsonOBJ(new GetMeesage
                            {
                                Res = 123,
                                Text = "表单解析发生错误，请检查数据"
                            })
                        };
                        await Response.BodyWriter.WriteAsync(httpReturn.Data);
                        return;
                    }
                }
                else if (Request.ContentType.StartsWith(ServerContentType.JSON))
                {
                    MemoryStream stream = new();
                    await Request.Body.CopyToAsync(stream);
                    var Str = Encoding.UTF8.GetString(stream.ToArray());
                    JObject obj = JObject.Parse(Function.GetSrings(Str, "{"));
                    foreach (var item in obj)
                    {
                        temp.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    type = MyContentType.Other;
                }
            }

            var Dll = DllStonge.GetDll(uuid);
            if (Dll != null)
            {
                NameValueCollection collection = new();
                foreach (var item in Request.Headers)
                {
                    collection.Add(item.Key, item.Value);
                }

                httpReturn = DllRun.DllGo(Dll, new()
                {
                    Cookie = ASPHttpUtils.HaveCookie(Request.Headers.Cookie),
                    Parameter = temp,
                    RowRequest = collection,
                    ContentType = type,
                    Method = Request.Method,
                    Stream = type == MyContentType.Other ? Request.Body : null
                }, name);
            }
            else
            {
                httpReturn = new HttpReturn
                {
                    Data = HtmlUtils.Html404,
                    ContentType = ServerContentType.HTML
                };
            }
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

